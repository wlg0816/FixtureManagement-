﻿using FixtureManagementBLL.UtilTool;
using FixtureManagementModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementBLL.Service.Impl
{
    public class MesFrockImpl : IMesFrockService
    {
        public List<DeviceLocationEntity> getDeviceLocationList(string deviceCode)
        {
            string url = "/blade-eqp/frockLoan/getBindingFrock?eqpSn=" + deviceCode;

            JObject obj = (JObject)JsonConvert.DeserializeObject(HttpClientUtil.GetRequest(url));

            List<DeviceLocationEntity> list = new List<DeviceLocationEntity>();

            if (obj != null)
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
            string url = "/blade-eqp/frockLoan/getFrockLifeInfo?id="+ id;

            JObject obj = (JObject)JsonConvert.DeserializeObject(HttpClientUtil.GetRequest(url));

            return JsonConvert.DeserializeObject<frockLifeInfoEntity>(obj==null?"":obj["data"].ToString());
        }

        public historicalSummaryEntity getHistoricalSummaryEntity(long id)
        {
            string url = "/blade-eqp/frockLoan/getOperateHis?id="+id;

            JObject obj = (JObject)JsonConvert.DeserializeObject(HttpClientUtil.GetRequest(url));

            return JsonConvert.DeserializeObject<historicalSummaryEntity>(obj == null ? "" : obj["data"].ToString());
        }

        public List<frockBindingLocationEntity> getLocationAllList()
        {
            using(FixtureManagementDAL.devicePlcEntityContext db = new FixtureManagementDAL.devicePlcEntityContext())
            {
                return db.frockBindingLocation.Where(o=>!o.isDel).ToList();
            }
        }

    }
}
