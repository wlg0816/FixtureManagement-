using FixtureManagementBLL.UtilTool;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementBLL.Service.Impl
{
    public class AppConfigPathSetImpl : IAppConfigPathSetService
    {
        public string getAppConfigValue(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            return config.AppSettings.Settings[key].Value;
        }

        public void updateAppConfigValue(string key, string value)
        {
            AppConfigPathSetUtil.AppSetValue(key, value);
        }
    }
}
