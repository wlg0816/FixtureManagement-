using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementBLL.Service
{
    public interface IAppConfigPathSetService
    {
        /// <summary>
        /// 获取appconfig配置文件信息
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns></returns>
        string getAppConfigValue(string key);

        /// <summary>
        /// 更新配置文件参数信息
        /// </summary>
        /// <param name="key">主键</param>
        /// <param name="value">数据</param>
        /// <returns></returns>
        void updateAppConfigValue(string key,string value);
    }
}
