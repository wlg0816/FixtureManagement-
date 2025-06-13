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
            try
            {
                using (FixtureManagementDAL.devicePlcEntityContext db = new FixtureManagementDAL.devicePlcEntityContext())
                {
                    return db.Children.FirstOrDefault(o => !o.isDel && o.deviceCode.Equals(deviceCode));
                }
            }catch(Exception ex)
            {
                return new devicePlcEntity();
            }
            
        }

        public bool addOrUpdateDevicePlcEntity(devicePlcEntity devicePlc)
        {
            try
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
            }catch(Exception ex) 
            {
                return false;
            }
            
        }
    }
}
