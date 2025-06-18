using FixtureManagementModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.NetworkInformation;
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
                using (Ping pingSender = new Ping())
                {
                    PingReply reply = pingSender.Send("10.88.228.17", 120);

                    if (reply.Status != IPStatus.Success)
                    {
                        return new devicePlcEntity();
                    }

                    using (FixtureManagementDAL.devicePlcEntityContext db = new FixtureManagementDAL.devicePlcEntityContext())
                    {
                        return db.Children.FirstOrDefault(o => !o.isDel && o.deviceCode.Equals(deviceCode));
                    }
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
                using (Ping pingSender = new Ping())
                {
                    PingReply reply = pingSender.Send("10.88.228.17", 120);

                    if (reply.Status != IPStatus.Success)
                    {
                        return false;
                    }
                }
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
