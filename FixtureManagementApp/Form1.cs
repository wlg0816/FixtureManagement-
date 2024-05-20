using FixtureManagementModel;
using FixtureManagementModel.enums;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace FixtureManagementApp
{
    public partial class Form1 : Form 
    {
        public Form1()
        {
            InitializeComponent();
        }

        WarningDialogForm dialogForm = new WarningDialogForm();
        /// <summary>
        /// 当前选择的工装
        /// </summary>
        public long uiNavBar1SelectedIndexId;
        /// <summary>
        /// 获取历史的工装数据
        /// </summary>
        public long uiNavBar2SelectedIndexId;
        /// <summary>
        /// 当前选择的工装
        /// </summary>
        public int selectMainIndex;
        /// <summary>
        /// 获取历史数据选择
        /// </summary>
        public int selectHistoricalIndex;
        private void timer1_Tick(object sender, EventArgs e)
        {
            // 获取界面年月日、星期、时分秒
            this.getPageDateTimeEntity();
            // 根据当前绑定工装获取数据
            this.getMainEntity();
            // 绑定当前设备绑定位置(页签\工装状态检测)
            this.getAllDeviceLocationTimer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 获取当前数据主界面固定数据
            this.getPageDateTimeEntity();
            // 获取主界面展示数据
            this.getMainEntity();
            // 获取当前的配置信息
            this.getLocationList();
            // 绑定当前设备绑定位置(页签)
            this.getAllDeviceLocation();
        }
        /// <summary>
        /// 获取界面年月日、星期、时分秒
        /// </summary>
        private void getPageDateTimeEntity()
        {
            uiLabel3.Text = DateTime.Now.ToString("yyyy年MM月dd日");

            uiLabel4.Text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);

            uiLabel5.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        /// <summary>
        /// 主数据界面
        /// </summary>
        private void getMainEntity()
        {
            //读取XML的数据值
            FixtureManagementBLL.Service.IAppConfigPathSetService configPathSetService = new FixtureManagementBLL.Service.Impl.AppConfigPathSetImpl();

            string deviceCode = configPathSetService.getAppConfigValue("deviceCode");        

            FixtureManagementBLL.Service.IMesFrockService frockService = new FixtureManagementBLL.Service.Impl.MesFrockImpl();

            var frockEntity = frockService.getFrockLifeInfo(uiNavBar1SelectedIndexId);

            if (uiTabControlMenu1.SelectedIndex == 1)
            {
                var historicalEntity = frockService.getHistoricalSummaryEntity(uiNavBar2SelectedIndexId);
                // 获取历史的汇总数据
                this.HistoricalSummary(historicalEntity);
            }
            
            this.sendMainPage(frockEntity);

            if (frockEntity == null)
            {
                this.ShowErrorNotifier("获取工装关联数据失败,请联系系统管理员！");

                FixtureManagementBLL.UtilTool.NetLogUtil.WriteTextLog("MainMessage", "设备编号:"+deviceCode + ",获取工装关联数据失败!");

                return;
            }           
               
        }
        /// <summary>
        /// 获取设备信息数据
        /// </summary>
        private void getLocationList()
        {
            FixtureManagementBLL.Service.IAppConfigPathSetService configPathSetService = new FixtureManagementBLL.Service.Impl.AppConfigPathSetImpl();

            uiTextBox1.Text = configPathSetService.getAppConfigValue("deviceCode");

        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            // 写入配置信息
            FixtureManagementBLL.Service.IAppConfigPathSetService configPathSetService = new FixtureManagementBLL.Service.Impl.AppConfigPathSetImpl();

            configPathSetService.updateAppConfigValue("deviceCode", uiTextBox1.Text);

            this.ShowSuccessTip("更新配置信息 - 成功");
        }

        private void sendMainPage(FixtureManagementModel.frockLifeInfoEntity infoEntity)
        {
            // 工装名称
            uiMarkLabel1.Text = infoEntity != null ? infoEntity.frockName : "";
            // 工装流水
            uiMarkLabel2.Text = infoEntity != null ? infoEntity.frockSn : "";
            // 设备名称
            uiMarkLabel3.Text = infoEntity != null ? infoEntity.eqpName : "";
            // 设备编号
            uiMarkLabel4.Text = infoEntity != null ? infoEntity.eqpSn : "";
            // 员工姓名
            uiMarkLabel5.Text = infoEntity != null ? infoEntity.userName : "";
            // 员工工号
            uiMarkLabel6.Text = infoEntity != null ? infoEntity.account : "";
            // 今日使用
            uiMarkLabel7.Text = infoEntity != null ? infoEntity.usedToday.ToString() : "";
            // 累计使用
            uiMarkLabel8.Text = infoEntity != null ? infoEntity.lifeNumConsumption.ToString() : "";
            // 剩余次数
            uiMarkLabel9.Text = infoEntity != null ? infoEntity.remainLifeNum.ToString() : "";
            // 本次使用
            uiMarkLabel10.Text = infoEntity != null ? infoEntity.usedCurrent.ToString() : "";
            // 剩余次数百分比
            uiProcessBar1.Value = infoEntity != null ? infoEntity.remainingLife : 0;

        }
        /// <summary>
        /// 控制弹窗的展示及隐藏
        /// </summary>
        private void ShowWarningDialog()
        {
            if (!dialogForm.Visible)
            {
                dialogForm.StartPosition = FormStartPosition.CenterParent;

                dialogForm.ShowDialog();
            }
        }
        /// <summary>
        /// 控制弹窗的隐藏
        /// </summary>
        private void HideWarningDialog()
        {
            if (dialogForm.Visible)
            {
                dialogForm.Hide();
            }
        }

        private void updatePlcState(int fockState, string deviceCode)
        {
            // 获取PLC配置信息（工装剩余不足）
            FixtureManagementBLL.Service.IDevicePlcEntityService plcEntityService = new FixtureManagementBLL.Service.Impl.DevicePlcEntityImpl();

            var entity = plcEntityService.getDevicePlcEntityList(deviceCode);

            if(entity == null)
            {
                this.ShowErrorNotifier("获取PLC配置数据失败,请联系设备管理员！");
            }

            FixtureManagementBLL.Service.IStandardModbusService standard = new FixtureManagementBLL.Service.Impl.StandardModbusImpl();

            if (!standard.sendModeus(fockState,entity))
            {
                FixtureManagementBLL.UtilTool.NetLogUtil.WriteTextLog("sendModeus", "更新PLC状态失败!");
            }
        }
        /// <summary>
        /// 获取历史的工装数据
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="deviceLocation"></param>
        private void HistoricalSummary(FixtureManagementModel.historicalSummaryEntity historicalSummary)
        {
            DataTable dataTable = new DataTable("DataTable");
            dataTable.Columns.Add("日期时间");
            dataTable.Columns.Add("类型");
            dataTable.Columns.Add("设备编号");
            dataTable.Columns.Add("设备名称");
            dataTable.Columns.Add("寿命变化");
            if(historicalSummary != null&& historicalSummary.records != null)
            {
                if (historicalSummary.records.Count > 0)
                {
                    foreach (var item in historicalSummary.records)
                    {
                        dataTable.Rows.Add(item.date, item.type, item.eqpSn, item.eqpName, item.lifeChange);
                    }
                }                
            }
            else
            {
                this.ShowErrorNotifier("获取当前工装历史数据失败,请联系系统管理员！");
            }
            uiDataGridView1.DataSource = dataTable;

            uiMarkLabel11.Text = historicalSummary != null && historicalSummary.frockInfo != null ? historicalSummary.frockInfo.frockSn : "";

            uiMarkLabel12.Text = historicalSummary != null && historicalSummary.frockInfo != null ? historicalSummary.frockInfo.frockName : "";

            uiMarkLabel13.Text = historicalSummary != null && historicalSummary.frockInfo != null ? historicalSummary.frockInfo.initialLifeNum.ToString() : "";

            uiMarkLabel14.Text = historicalSummary != null && historicalSummary.frockInfo != null ? historicalSummary.frockInfo.lifeNumConsumption.ToString() : "";

            uiMarkLabel15.Text = historicalSummary != null && historicalSummary.frockInfo != null ? historicalSummary.frockInfo.lifeNumAdd.ToString() : "";

            uiMarkLabel16.Text = historicalSummary != null && historicalSummary.frockInfo != null ? historicalSummary.frockInfo.remainLifeNum.ToString() : "";

        }

        private void uiImageButton2_Click(object sender, EventArgs e)
        {
            // 设置关联PLC信息
            EditPlcMain editPlcMain = new EditPlcMain();

            editPlcMain.ShowDialog();
        }

        private void uiTabControlMenu1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //触发点击事件
            timer1.Enabled = uiTabControlMenu1.SelectedIndex < 2;
        }

        private void uiNavBar1_MenuItemClick(string itemText, int menuIndex, int pageIndex)
        {            
            uiNavBar1SelectedIndexId = long.Parse(uiNavBar1.Nodes[menuIndex].Name);

            selectMainIndex = uiNavBar1.SelectedIndex;
        }

        private List<DeviceLocationEntity> getAllDeviceLocation()
        {
            FixtureManagementBLL.Service.IAppConfigPathSetService configPathSetService = new FixtureManagementBLL.Service.Impl.AppConfigPathSetImpl();

            FixtureManagementBLL.Service.IMesFrockService frockService=new FixtureManagementBLL.Service.Impl.MesFrockImpl();

            var list= frockService.getDeviceLocationList(configPathSetService.getAppConfigValue("deviceCode"));        

            uiNavBar1.Nodes.Clear();

            uiNavBar2.Nodes.Clear();

            for (int i=0; i<list.Count; i++)
            {
                if (i < 4)
                {
                    uiNavBar1.Nodes.Insert(i, list[i].id.ToString(), "工装" + (i + 1));
                    uiNavBar2.Nodes.Insert(i, list[i].id.ToString(), "工装" + (i + 1));
                }
                else
                {
                    if (i == 4)
                    {
                        uiNavBar1.Nodes.Insert(i, list[i].id.ToString(), "更多");
                        uiNavBar2.Nodes.Insert(i, list[i].id.ToString(), "更多");

                    }
                    uiNavBar1.Nodes[4].Nodes.Insert(i, list[i].id.ToString(), "工装" + (i + 1));
                    uiNavBar2.Nodes[4].Nodes.Insert(i, list[i].id.ToString(), "工装" + (i + 1));
                }                
            }
            if (list.Count > 0)
            {
                uiNavBar1.SelectedIndex= selectMainIndex;
                uiNavBar1SelectedIndexId = list[selectMainIndex].id;

                uiNavBar2.SelectedIndex = selectHistoricalIndex;
                uiNavBar2SelectedIndexId = list[selectHistoricalIndex].id;
            }
            return list;
        }

        private void uiNavBar2_MenuItemClick(string itemText, int menuIndex, int pageIndex)
        {
            uiNavBar2SelectedIndexId = long.Parse(uiNavBar2.Nodes[menuIndex].Name);

            selectHistoricalIndex = uiNavBar2.SelectedIndex;
        }
        //
        public void getAllDeviceLocationTimer()
        {
            var list = this.getAllDeviceLocation();

            //判断当前工装是否存在异常
            if (list.Count > 0)
            {
                this.remainLifeNumFun(list);
            }
        }

        public void remainLifeNumFun(List<DeviceLocationEntity> list)
        {
            var remainLifeList= list.Where(o=>o.remainLifeNum==0).ToList();

            //读取XML的数据值
            FixtureManagementBLL.Service.IAppConfigPathSetService configPathSetService = new FixtureManagementBLL.Service.Impl.AppConfigPathSetImpl();

            string deviceCode = configPathSetService.getAppConfigValue("deviceCode");

            // 根据接口数据打开或者关闭告警
            if (remainLifeList.Count > 0)
            {
                List<string> frockSns = remainLifeList.Select(p => p.frockSn).ToList();
                //错误信息提示框;
                this.ShowWarningDialog();

                updatePlcState(fockStateEnum.Already.GetHashCode(), deviceCode);

                string message = string.Join(" , ", frockSns.ToArray()) + ",工装剩余寿命告警提示!";

                this.ShowErrorNotifier(message);

                FixtureManagementBLL.UtilTool.NetLogUtil.WriteTextLog("ShowWarningDialog", message);
            }
            else
            {
                //关闭错误信息提示框;
                this.HideWarningDialog();

                updatePlcState(fockStateEnum.Soon.GetHashCode(), deviceCode);

            }
        }
    }
}
