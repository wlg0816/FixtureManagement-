using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementBLL.Service
{
    public interface IStandardModbusService
    {
        /// <summary>
        /// 获取当前PLC数据（状态）
        /// </summary>
        /// <param name="plcEntity"></param>
        /// <returns></returns>
        int obtainModbus(FixtureManagementModel.devicePlcEntity plcEntity);

        /// <summary>
        /// 发送数据到PLC
        /// </summary>
        /// <param name="plcEntity"></param>
        /// <returns></returns>
        bool sendModeus(int fockState,FixtureManagementModel.devicePlcEntity plcEntity);
    }
}
