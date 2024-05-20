using FixtureManagementModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementBLL.Service.Impl
{
    public class DevicePlcEntityImpl : IDevicePlcEntityService
    {
        public FixtureManagementModel.devicePlcEntity getDevicePlcEntityList(string deviceCode)
        {
            using (FixtureManagementDAL.devicePlcEntityContext db = new FixtureManagementDAL.devicePlcEntityContext())
            {
                return db.Children.FirstOrDefault(o => !o.isDel && o.deviceCode.Equals(deviceCode));
            }
        }

        public bool addOrUpdateDevicePlcEntity(devicePlcEntity devicePlc)
        {
            using (FixtureManagementDAL.devicePlcEntityContext db = new FixtureManagementDAL.devicePlcEntityContext())
            {
                if (devicePlc.id == null)
                {
                    devicePlc.createDateTime = DateTime.Now;                  
                }
                db.Children.AddOrUpdate(devicePlc);
                return db.SaveChanges() > 0;
            }
        }
    }
}
