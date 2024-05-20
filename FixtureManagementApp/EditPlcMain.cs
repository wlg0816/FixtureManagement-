using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FixtureManagementApp
{
    public partial class EditPlcMain : UIEditForm
    {
        public EditPlcMain()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 设备PLC对象
        /// </summary>
        private static FixtureManagementModel.devicePlcEntity devicePlcEntity=new FixtureManagementModel.devicePlcEntity();

        private void btnOK_Click(object sender, EventArgs e)
        {
            devicePlcEntity.devicePlcIp = uiipTextBox1.Text;

            devicePlcEntity.devicePlcPort = uiIntegerUpDown1.Value.ToString();

            devicePlcEntity.devicePlcIo = uiTextBox1.Text;

            FixtureManagementBLL.Service.IDevicePlcEntityService plcEntityService = new FixtureManagementBLL.Service.Impl.DevicePlcEntityImpl();

            if (plcEntityService.addOrUpdateDevicePlcEntity(devicePlcEntity))
            {
                this.ShowSuccessTip("更新设备PLC配置信息 - 成功");
            }      
        }

        private void EditPlcMain_Load(object sender, EventArgs e)
        {
            //读取XML的数据值
            FixtureManagementBLL.Service.IAppConfigPathSetService configPathSetService = new FixtureManagementBLL.Service.Impl.AppConfigPathSetImpl();

            devicePlcEntity.deviceCode = configPathSetService.getAppConfigValue("deviceCode");

            uiTextBox2.Text = devicePlcEntity.deviceCode;
            // 获取PLC配置信息（工装剩余不足）
            FixtureManagementBLL.Service.IDevicePlcEntityService plcEntityService = new FixtureManagementBLL.Service.Impl.DevicePlcEntityImpl();

            var entity = plcEntityService.getDevicePlcEntityList(devicePlcEntity.deviceCode);

            if(entity != null)
            {
                uiipTextBox1.Text = entity.devicePlcIp;

                uiIntegerUpDown1.Value = int.Parse(entity.devicePlcPort!=null? entity.devicePlcPort:"0");

                uiTextBox1.Text = entity.devicePlcIo;

                devicePlcEntity=entity;
            }
            
        }
    }
}
