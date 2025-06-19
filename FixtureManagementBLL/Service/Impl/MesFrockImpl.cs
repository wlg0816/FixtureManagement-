using FixtureManagementBLL.UtilTool;
using FixtureManagementModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementBLL.Service.Impl
{
    public class MesFrockImpl : IMesFrockService
    {
        public List<DeviceLocationEntity> getDeviceLocationList(string deviceCode)
        {
            using (Ping pingSender = new Ping())
            {
                try
                {
                    PingReply reply = pingSender.Send("10.88.228.17", 120);

                    if (reply.Status != IPStatus.Success)
                    {
                        return new List<DeviceLocationEntity>();
                    }
                }
                catch (Exception ex)
                {
                    return new List<DeviceLocationEntity>();
                }
            }
            string url = "/blade-eqp/frockLoan/getBindingFrock?eqpSn=" + deviceCode;

            JObject obj = (JObject)JsonConvert.DeserializeObject(HttpClientUtil.GetRequest(url));

            List<DeviceLocationEntity> list = new List<DeviceLocationEntity>();

            if (obj != null && obj.ToString() !="")
            {
                foreach (var item in obj["data"])
                {
                    list.Add(JsonConvert.DeserializeObject<DeviceLocationEntity>(item.ToString()));
                }
            }
           
            return list;
        }

        public frockLifeInfoEntity getFrockLifeInfo(long id)
        {
            using (Ping pingSender = new Ping())
            {
                try
                {
                    PingReply reply = pingSender.Send("10.88.228.17", 120);

                    if (reply.Status != IPStatus.Success)
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            string url = "/blade-eqp/frockLoan/getFrockLifeInfo?id="+ id;

            JObject obj = (JObject)JsonConvert.DeserializeObject(HttpClientUtil.GetRequest(url));

            return JsonConvert.DeserializeObject<frockLifeInfoEntity>(obj==null || obj.ToString() == "" ? "":obj["data"].ToString());
        }

        public historicalSummaryEntity getHistoricalSummaryEntity(long id)
        {
            using (Ping pingSender = new Ping())
            {
                try
                {
                    PingReply reply = pingSender.Send("10.88.228.17", 120);

                    if (reply.Status != IPStatus.Success)
                    {
                        return new historicalSummaryEntity();
                    }
                }
                catch (Exception ex)
                {
                    return new historicalSummaryEntity();
                }
            }

            string url = "/blade-eqp/frockLoan/getOperateHis?id="+id;

            JObject obj = (JObject)JsonConvert.DeserializeObject(HttpClientUtil.GetRequest(url));

            return JsonConvert.DeserializeObject<historicalSummaryEntity>(obj == null || obj.ToString() == "" ? "" : obj["data"].ToString());
        }

        public List<frockBindingLocationEntity> getLocationAllList()
        {
            using (Ping pingSender = new Ping())
            {
                try
                {
                    PingReply reply = pingSender.Send("10.88.228.17", 120);

                    if (reply.Status != IPStatus.Success)
                    {
                        return new List<frockBindingLocationEntity>();
                    }

                    using (FixtureManagementDAL.devicePlcEntityContext db = new FixtureManagementDAL.devicePlcEntityContext())
                    {
                        return db.frockBindingLocation.Where(o => !o.isDel).ToList();
                    }
                }
                catch (Exception ex)
                {
                    return new List<frockBindingLocationEntity>();
                }
            }
  
        }

    }
}
