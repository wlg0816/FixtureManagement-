using FixtureManagementModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementBLL.Service
{
    public interface IDevicePlcEntityService
    {
        /// <summary>
        /// 获取PLC配置信息
        /// </summary>
        /// <param name="deviceCode">设备编号</param>
        /// <param name="deviceLocation">模具位置</param>
        /// <returns></returns>
        FixtureManagementModel.devicePlcEntity getDevicePlcEntityList(string deviceCode);
        /// <summary>
        /// 更新PLC配置信息
        /// </summary>
        /// <param name="devicePlc"></param>
        /// <returns></returns>
        bool addOrUpdateDevicePlcEntity(devicePlcEntity devicePlc);
    }
}
