using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace FixtureManagementBLL.UtilTool
{
    public partial class HCPlcUtil
    {
        [DllImport("StandardModbusApi.dll", EntryPoint = "Init_ETH_String", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Init_ETH_String(string sIpAddr, int nNetId = 0, int IpPort = 502);

        [DllImport("StandardModbusApi.dll", EntryPoint = "Exit_ETH", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Exit_ETH(int nNetId = 0);

        [DllImport("StandardModbusApi.dll", EntryPoint = "H3u_Read_Soft_Elem", CallingConvention = CallingConvention.Cdecl)]
        public static extern int H3u_Read_Soft_Elem(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId = 0);
        [DllImport("StandardModbusApi.dll", EntryPoint = "H3u_Write_Soft_Elem", CallingConvention = CallingConvention.Cdecl)]
        public static extern int H3u_Write_Soft_Elem(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId = 0);
        [DllImport("StandardModbusApi.dll", EntryPoint = "H5u_Write_Device_Block", CallingConvention = CallingConvention.Cdecl)]
        public static extern int H5u_Write_Device_Block(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId = 0);

        /// <summary>
        /// 获取汇川PLC数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int getPlcData(FixtureManagementModel.hcPlcEntity.PlcRequest entity)
        {
            int nNetId = 1;
            Exit_ETH(nNetId);
            bool result = Init_ETH_String(entity.gatherPlcIp, nNetId, entity.gatherPlcPort);
            if (!result)
            {
                return -2;
            }
            byte[] pBuf = new byte[16000];
            int nStartAddr = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(entity.gatherPlcIo, @"[^0-9]+", ""));
            int nCount = Convert.ToInt32(1);
            bool bIsWord = true;//是否字元件
            SoftElemType ElemType = SoftElemType.REGI_H5U_Y;
            ElemType = SoftElemType.REGI_H3U_DW;
            int nRet = H3u_Read_Soft_Elem(ElemType, nStartAddr, nCount, pBuf, nNetId);
            if (nRet == 1)
            {
                string strData = "";
                int nDataType = 0;
                if (nDataType == 1 || nDataType == 2)
                {
                    nCount = nCount / 2;
                }
                for (int i = 0; i < nCount; i++)
                {
                    if (bIsWord)
                    {
                        if (nDataType == 0)//16位整形
                        {
                            byte[] databuf = new byte[2] { 0, 0 };
                            databuf[0] = pBuf[i * 2];
                            databuf[1] = pBuf[i * 2 + 1];
                            int iTemp = BitConverter.ToInt16(databuf, 0);
                            strData = strData + iTemp.ToString() + " ";
                            continue;
                        }
                        else if (nDataType == 1)//读取32位整形
                        {
                            byte[] databuf = new byte[4] { 0, 0, 0, 0 };
                            databuf[0] = pBuf[i * 4];
                            databuf[1] = pBuf[i * 4 + 1];
                            databuf[2] = pBuf[i * 4 + 2];
                            databuf[3] = pBuf[i * 4 + 3];
                            int iTemp = BitConverter.ToInt32(databuf, 0);
                            strData = strData + iTemp.ToString() + " ";
                            continue;
                        }
                        else if (nDataType == 2)//读取浮点型
                        {
                            byte[] databuf = new byte[4] { 0, 0, 0, 0 };
                            databuf[0] = pBuf[i * 4];
                            databuf[1] = pBuf[i * 4 + 1];
                            databuf[2] = pBuf[i * 4 + 2];
                            databuf[3] = pBuf[i * 4 + 3];
                            float fTemp = BitConverter.ToSingle(databuf, 0);
                            strData = strData + fTemp.ToString() + " ";
                            continue;
                        }
                    }
                    else
                    {
                        int nVal = 0;
                        nVal = pBuf[i];
                        strData = strData + nVal.ToString() + " ";
                    }
                }
                return int.Parse(strData);
            }
            return -2;
        }
        /// <summary>
        /// 发送数据给PLC
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool postPlcData(FixtureManagementModel.hcPlcEntity.PlcRequest entity)
        {
            int nNetId = 1;
            Exit_ETH(nNetId);
            bool result = Init_ETH_String(entity.gatherPlcIp, nNetId, entity.gatherPlcPort);
            if (!result)
            {
                return false;
            }
            // 判断PLC点位是否正确
            if (getPlcData(entity) < 0)
            {
                return false;
            }
            byte[] pBuf = new byte[16000];
            int nStartAddr = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(entity.gatherPlcIo, @"[^0-9]+", ""));
            int nCount = Convert.ToInt32(1);
            bool bIsWord = false;//是否字元件
            SoftElemType ElemType = SoftElemType.REGI_H5U_Y;
            bIsWord = true;
            ElemType = SoftElemType.REGI_H5U_D;
            string[] arr = entity.gatherPlcIoState.ToString().Split(' ');
            int nDataType = 0;
            GetDataFromUI(pBuf, arr, bIsWord, nDataType);
            int nRet = H5u_Write_Device_Block(ElemType, nStartAddr, nCount, pBuf, nNetId);
            return nRet == 1;
        }
        private static void GetDataFromUI(byte[] pBuf, string[] arr, bool bIsWord, int nDataType)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == string.Empty)
                {
                    break;
                }
                double nVal = Convert.ToDouble(arr[i]);
                if (bIsWord)
                {
                    if (nDataType == 0)//16位整形
                    {
                        int idata = Convert.ToInt16(arr[i]);
                        byte[] dataBuf = new byte[2] { 0, 0 };
                        dataBuf = BitConverter.GetBytes(idata);
                        pBuf[2 * i] = dataBuf[0];
                        pBuf[2 * i + 1] = dataBuf[1];
                    }
                    else if (nDataType == 1)//32位整形
                    {
                        int idata = Convert.ToInt32(arr[i]);
                        byte[] dataBuf = new byte[4] { 0, 0, 0, 0 };
                        dataBuf = BitConverter.GetBytes(idata);
                        pBuf[4 * i] = dataBuf[0];
                        pBuf[4 * i + 1] = dataBuf[1];
                        pBuf[4 * i + 2] = dataBuf[2];
                        pBuf[4 * i + 3] = dataBuf[3];
                    }
                    else if (nDataType == 2)//浮点数
                    {
                        float fdata = Convert.ToSingle(arr[i]);
                        byte[] dataBuf = new byte[4] { 0, 0, 0, 0 };
                        dataBuf = BitConverter.GetBytes(fdata);
                        pBuf[4 * i] = dataBuf[0];
                        pBuf[4 * i + 1] = dataBuf[1];
                        pBuf[4 * i + 2] = dataBuf[2];
                        pBuf[4 * i + 3] = dataBuf[3];
                    }
                }
                else
                {
                    pBuf[i] = (byte)nVal;
                }
            }
        }

        public enum SoftElemType
        {
            //AM600
            ELEM_QX = 0,     //QX元件
            ELEM_MW = 1,     //MW元件
            ELEM_X = 2,      //X元件(对应QX200~QX300)
            ELEM_Y = 3,      //Y元件(对应QX300~QX400)

            //H3U
            REGI_H3U_Y = 0x20,       //Y元件的定义	
            REGI_H3U_X = 0x21,      //X元件的定义							
            REGI_H3U_S = 0x22,      //S元件的定义				
            REGI_H3U_M = 0x23,      //M元件的定义							
            REGI_H3U_TB = 0x24,     //T位元件的定义				
            REGI_H3U_TW = 0x25,     //T字元件的定义				
            REGI_H3U_CB = 0x26,     //C位元件的定义				
            REGI_H3U_CW = 0x27,     //C字元件的定义				
            REGI_H3U_DW = 0x28,     //D字元件的定义				
            REGI_H3U_CW2 = 0x29,        //C双字元件的定义
            REGI_H3U_SM = 0x2a,     //SM
            REGI_H3U_SD = 0x2b,     //
            REGI_H3U_R = 0x2c,      //
                                    //H5u
            REGI_H5U_Y = 0x30,       //Y元件的定义	
            REGI_H5U_X = 0x31,      //X元件的定义							
            REGI_H5U_S = 0x32,      //S元件的定义				
            REGI_H5U_M = 0x33,      //M元件的定义	
            REGI_H5U_B = 0x34,       //B元件的定义
            REGI_H5U_D = 0x35,       //D字元件的定义
            REGI_H5U_R = 0x36,       //R字元件的定义

        }
    }
}
