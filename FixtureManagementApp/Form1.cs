using FixtureManagementBLL.UtilTool;
using FixtureManagementModel;
using FixtureManagementModel.enums;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace FixtureManagementApp
{
    public partial class Form1 : Form 
    {
        //等比例缩放的原始点
        private float X;
        private float Y;

        #region 所有控件等比例缩放
        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    setTag(con);
            }
        }
        private void setControls(float newx, float newy, Control cons)
        {
            foreach (Control con in cons.Controls)
            {

                string[] mytag = con.Tag.ToString().Split(':');
                float a = Convert.ToSingle(mytag[0]) * newx;
                con.Width = (int)a;
                a = Convert.ToSingle(mytag[1]) * newy;
                con.Height = (int)(a);
                a = Convert.ToSingle(mytag[2]) * newx;
                con.Left = (int)(a);
                a = Convert.ToSingle(mytag[3]) * newy;
                con.Top = (int)(a);
                Single currentSize = Convert.ToSingle(mytag[4]) * Math.Min(newx, newy);
                con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);
                }
            }

        }

        void Form_Resize(object sender, EventArgs e)
        {
            float newx = (this.Width) / X;
            float newy = this.Height / Y;
            setControls(newx, newy, this);
        }
        #endregion

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // 发送消息
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        // 关闭消息
        private const uint WM_CLOSE = 0x0010;
        public Form1()
        {
            InitializeComponent();
            // 多线程控件访问
            Control.CheckForIllegalCrossThreadCalls = false;

            X = this.Width;
            Y = this.Height;

            //在窗体加载时候  解决闪烁问题
            //将图像绘制到缓冲区 减少闪烁
            this.DoubleBuffered = true;//设置本窗体
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
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
        /// <summary>
        /// 当前页签数据集合
        /// </summary>
        public static List<DeviceLocationEntity> deviceLocationList;

        public delegate void OutDelegate(bool isShow);

        public delegate void ShowHistoricalSummary();
        /// <summary>
        /// 委托事件更新页面
        /// </summary>
        /// <param name="infoEntity"></param>
        public delegate void PostMainPage(FixtureManagementModel.frockLifeInfoEntity infoEntity);
        /// <summary>
        /// 委托写入日志文件
        /// </summary>
        /// <param name="message"></param>
        public delegate void sendSystemLog(string messageType, string message);

        private void Form1_Load(object sender, EventArgs e)
        {
            //按键等比例缩放，注册到窗口Resize事件中
            this.Resize += new EventHandler(Form_Resize);

            setTag(this);
            //在MDI时用
            Form_Resize(new object(), new EventArgs());
            // 获取当前数据主界面固定数据
            this.getPageDateTimeEntity();           
            // 获取当前的配置信息
            this.getLocationList();

        }

        public void uiWaitingBarDisplay(bool isShow)
        {
            //uiWaitingBar2.Visible = isShow;
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
            OutDelegate handler = uiWaitingBarDisplay;

            handler(true);

            //读取XML的数据值
            FixtureManagementBLL.Service.IAppConfigPathSetService configPathSetService = new FixtureManagementBLL.Service.Impl.AppConfigPathSetImpl();

            string deviceCode = configPathSetService.getAppConfigValue("deviceCode");

            FixtureManagementBLL.Service.IMesFrockService frockService = new FixtureManagementBLL.Service.Impl.MesFrockImpl();

            Task task1 = Task.Run(() =>
            {
                if (uiNavBar1SelectedIndexId == 0)
                {
                    this.ShowSuccessTip("获取当前设备绑定的工装列表信息失败！");

                    sendSystemErrLog("MainMessage", "设备编号:" + deviceCode + ",获取设备绑定工装数据失败!");

                    return;
                }

                var frockEntity = frockService.getFrockLifeInfo(uiNavBar1SelectedIndexId);
                // 判断当前填写的工装编号
                var deviceLocation = deviceLocationList.FirstOrDefault(x => x.id == uiNavBar1SelectedIndexId);

                if(deviceLocation != null && frockEntity != null)
                {
                    frockEntity.modelCode= deviceLocation.modelCode;
                }

                if (frockEntity == null)
                {
                    this.ShowSuccessTip("获取工装关联数据失败,请联系系统管理员！");

                    sendSystemErrLog("MainMessage", "设备编号:" + deviceCode + ",获取工装关联数据失败!");

                    return;
                }
                // 设置委托
                PostMainPage mainPage = new PostMainPage(sendMainPage);

                this.Invoke(mainPage, frockEntity);

                handler(false);

                
            });       
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

            this.ShowSuccessDialog("更新设备配置信息成功,请重启程序后生效!");
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
            // 工装编号
            uiTextBox2.Text = infoEntity != null&& infoEntity.modelCode!=null ? infoEntity.modelCode.ToString() : "";

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
            // 开启多线程
            Task task1 = Task.Run(() =>
            {
                if (!standard.sendModeus(fockState, entity))
                {
                    sendSystemErrLog("sendModeus", "更新PLC状态失败!");
                }
            });
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
                        dataTable.Rows.Add(item.date.ToString("yyyy-MM-dd"), item.type, item.eqpSn, item.eqpName, item.lifeChange);
                    }
                }                
            }
            else
            {               
                this.ShowErrorNotifier("获取当前工装历史数据失败,请联系系统管理员！");
                // 不更新数据
                return;
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

            if (uiTabControlMenu1.SelectedIndex == 1)
            {
                timer2.Enabled = true;

                timer1.Enabled = false;
            }

            if (uiTabControlMenu1.SelectedIndex == 0)
            {
                timer1.Enabled = true;

                timer2.Enabled = false;
            }
        }

        private void uiNavBar1_MenuItemClick(string itemText, int menuIndex, int pageIndex)
        {
            uiNavBar1SelectedIndexId = long.Parse(uiNavBar1.Nodes[menuIndex].Name);

            selectMainIndex = uiNavBar1.SelectedIndex;

            // 获取主界面展示数据
            this.getMainEntity();
        }

        private List<DeviceLocationEntity> getAllDeviceLocation()
        {
            FixtureManagementBLL.Service.IAppConfigPathSetService configPathSetService = new FixtureManagementBLL.Service.Impl.AppConfigPathSetImpl();

            FixtureManagementBLL.Service.IMesFrockService frockService=new FixtureManagementBLL.Service.Impl.MesFrockImpl();

            var list= frockService.getDeviceLocationList(configPathSetService.getAppConfigValue("deviceCode"));
            // 临时列表不为空
            if (deviceLocationList == null && list.Count > 0)
            {
                deviceLocationList = list;
            }

            if (list.Count<=0)
            {
                return new List<DeviceLocationEntity>();
            }

            this.uiNavBar1.Nodes.Clear();

            this.uiNavBar2.Nodes.Clear();

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

            // 设置委托
            ShowHistoricalSummary showHistorical = getHistoricalSummaryFun;

            showHistorical();
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
            var remainLifeList= new List<DeviceLocationEntity>();

            Boolean isHalt = false;

            var isWarnList = new List<DeviceLocationEntity>();

            Boolean isWarn = false;

            var isErrorList = new List<DeviceLocationEntity>();

            Boolean isError = false;
            // 判断是否达到提醒
            foreach(var item in list)
            {
                if(item.remainLifeNum <= 0)
                {
                    isHalt = true;
                    remainLifeList.Add(item);
                    break;
                }
                if((item.remainLifeNum / item.initialLifeNum) <= new decimal(0.1))
                {
                    isWarn = true;
                    isWarnList.Add(item);
                }
                
            };
            foreach(var item in list)
            {               
                var obj =deviceLocationList.FindLast(o=>o.id== item.id);

                if (obj == null)
                {
                    break;
                }

                if (obj.modelCode == null || !obj.modelCode.Equals(item.frockOaSn))
                {
                    isError = true;
                    isErrorList.Add(item);
                }
            }
            //读取XML的数据值
            FixtureManagementBLL.Service.IAppConfigPathSetService configPathSetService = new FixtureManagementBLL.Service.Impl.AppConfigPathSetImpl();

            string deviceCode = configPathSetService.getAppConfigValue("deviceCode");
            // 判断工装模具是不是不符
            if (isError)
            {
                //updatePlcState(fockStateEnum.Error.GetHashCode(), deviceCode);
                // 填写的工装编号和实际的不符
                List<string> frockSns = isErrorList.Select(p => p.frockSn).ToList();

                string message = string.Join(" , ", frockSns.ToArray()) + ",工装型号不匹配!";
                // 工装弹窗信息
                ShowMessage(message, 15);

                this.ShowSuccessTip(message);

                sendSystemErrLog("工装信息不正确", message);
            }
            else
            {
                // 根据接口数据打开或者关闭告警
                if (isHalt)
                {
                    List<string> frockSns = remainLifeList.Select(p => p.frockSn).ToList();
                    //错误信息提示框;
                    this.ShowWarningDialog();

                    updatePlcState(fockStateEnum.Already.GetHashCode(), deviceCode);

                    string message = string.Join(" , ", frockSns.ToArray()) + ",工装剩余寿命告警提示!";

                    this.ShowSuccessTip(message);

                    sendSystemErrLog("ShowWarningDialog", message);
                }
                else
                {
                    if (isWarn)
                    {
                        //关闭错误信息提示框;
                        this.HideWarningDialog();

                        updatePlcState(fockStateEnum.Soon.GetHashCode(), deviceCode);

                        List<string> frockSns = isWarnList.Select(p => p.frockSn).ToList();

                        string message = string.Join(" , ", frockSns.ToArray()) + ",工装剩余寿命小于预设值!";

                        this.ShowSuccessTip(message);

                        sendSystemErrLog("ShowWarningDialog", message);
                    }
                    else
                    {

                        //关闭错误信息提示框;
                        this.HideWarningDialog();

                        updatePlcState(fockStateEnum.Init.GetHashCode(), deviceCode);
                    }


                }
            }          
        }
        /// <summary>
        /// 获取历史的数据
        /// </summary>
        public void getHistoricalSummaryFun()
        {
            uiWaitingBar1.Visible = true;

            FixtureManagementBLL.Service.IMesFrockService frockService = new FixtureManagementBLL.Service.Impl.MesFrockImpl();

            var historicalEntity = frockService.getHistoricalSummaryEntity(uiNavBar2SelectedIndexId);
            // 获取历史的汇总数据
            this.HistoricalSummary(historicalEntity);

            uiWaitingBar1.Visible = false;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Thread threadAll = new Thread(new ThreadStart(getAllDeviceLocationTimer));//实例化一个线程

            threadAll.IsBackground = true;//将线程改为后台线程

            threadAll.Start();//开启线程
       
        }
        private void timer2_Tick(object sender, EventArgs e)
        {

            Thread thread = new Thread(new ThreadStart(getAllDeviceLocationTimer));//实例化一个线程

            thread.IsBackground = true;//将线程改为后台线程

            thread.Start();//开启线程
            // 设置委托
            ShowHistoricalSummary showHistorical = getHistoricalSummaryFun;

            showHistorical();

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            // 获取工装的加工数据明细
            Thread thread = new Thread(new ThreadStart(getMainEntity));

            thread.IsBackground = true;//将线程改为后台线程

            thread.Start();//开启线程

            thread.Join();
        }

        /// <summary>
        /// 写入日志数据
        /// </summary>
        /// <param name="message"></param>
        public void sendSystemErrLog(string messageType, string message)
        {
            sendSystemLog sendSystemLog = new sendSystemLog(sendSystemLogFun);

            this.Invoke(sendSystemLog, messageType, message);
        }

        public void sendSystemLogFun(string messageType, string message)
        {
            FixtureManagementBLL.UtilTool.NetLogUtil.WriteTextLog(messageType, message);
        }

        private void uiTextBox2_TextChanged(object sender, EventArgs e)
        {
            deviceLocationList[uiNavBar1.SelectedIndex].modelCode = uiTextBox2.Text;
        }


        private void ShowMessage(string sMsg, int nSecondCount)
        {
            // 创建一个线程来执行倒计时操作
            Thread thread = new Thread(() =>
            {
                // 倒计时3秒
                Thread.Sleep(nSecondCount * 1000);

                // 关闭MessageBox
                if (InvokeRequired)
                {
                    Invoke(new Action(() => { CloseMessageBox(); }));
                }
                else
                {
                    CloseMessageBox();
                }
            });

            // 启动线程
            thread.Start();

            // 弹出MessageBox提示框，注意：这里的标题必须与下方查找关闭MessageBox里的标题一致。
            MessageBox.Show(sMsg, "完成提示");
        }

        private void CloseMessageBox()
        {
            // 查找并关闭MessageBox窗口
            IntPtr hwnd = FindWindow(null, "完成提示");//一致
            if (hwnd != IntPtr.Zero)
            {
                SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            Task.Run(() => getPageDateTimeEntity());

            using (Ping pingSender = new Ping())
            {
                try
                {
                    PingReply reply = pingSender.Send("10.88.228.17", 120);

                    if (reply.Status != IPStatus.Success)
                    {
                        uiSignal1.OnColor = Color.Red;
                    }
                    else
                    {
                        uiSignal1.OnColor = Color.Green;
                    }
                }
                catch (Exception ex)
                {
                    NetLogUtil.WriteTextLog("网络超时：", ex.Message);
                    uiSignal1.OnColor = Color.Red;
                }
            }           
        }
    }
}
