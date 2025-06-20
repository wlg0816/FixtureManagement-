using FixtureManagementBLL.UtilTool;
using FixtureManagementModel;
using FixtureManagementModel.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FixtureManagementBLL.Service.Impl
{
    public class StandardModbusImpl : IStandardModbusService
    {
        public int obtainModbus(devicePlcEntity plcEntity)
        {
            using (Ping pingSender = new Ping())
            {
                try
                {
                    PingReply reply = pingSender.Send(plcEntity.devicePlcIp, 120);

                    if (reply.Status != IPStatus.Success)
                    {
                        return 0;
                    }
                    hcPlcEntity.PlcRequest entity = new hcPlcEntity.PlcRequest();

                    entity.gatherPlcIp = plcEntity.devicePlcIp;

                    entity.gatherPlcPort = int.Parse(plcEntity.devicePlcPort);

                    entity.gatherPlcIo = plcEntity.devicePlcIo;

                    return HCPlcUtil.getPlcData(entity);
                }
                catch
                {
                    return 0;
                }
            }

            
        }

        public bool sendModeus(int fockState,devicePlcEntity plcEntity)
        {
            using (Ping pingSender = new Ping())
            {
                try
                {
                    PingReply reply = pingSender.Send(plcEntity.devicePlcIp, 120);

                    if (reply.Status != IPStatus.Success)
                    {
                        return false;
                    }
                    hcPlcEntity.PlcRequest entity = new hcPlcEntity.PlcRequest();

                    entity.gatherPlcIp = plcEntity.devicePlcIp;

                    entity.gatherPlcPort = int.Parse(plcEntity.devicePlcPort);

                    entity.gatherPlcIo = plcEntity.devicePlcIo;

                    entity.gatherPlcIoState = fockState;

                    return HCPlcUtil.postPlcData(entity);
                }
                catch
                {
                    return false;
                }
            }

             
        }
    }
}
