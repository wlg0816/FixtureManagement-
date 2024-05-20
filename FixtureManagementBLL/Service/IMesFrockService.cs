using FixtureManagementModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementBLL.Service
{
    public interface IMesFrockService
    {
        /// <summary>
        /// 获取下拉选设备点位列表
        /// </summary>
        /// <returns></returns>
        List<FixtureManagementModel.frockBindingLocationEntity> getLocationAllList();

        /// <summary>
        /// 获取工装展示主界面
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="deviceLocation"></param>
        /// <returns></returns>
        FixtureManagementModel.frockLifeInfoEntity getFrockLifeInfo(long id);
        /// <summary>
        /// 获取工装历史数据
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="deviceLocation"></param>
        /// <returns></returns>
        FixtureManagementModel.historicalSummaryEntity getHistoricalSummaryEntity(long id);
        /// <summary>
        /// 获取设备对应的工装列表
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        List<DeviceLocationEntity> getDeviceLocationList(string deviceCode);
    }
}
