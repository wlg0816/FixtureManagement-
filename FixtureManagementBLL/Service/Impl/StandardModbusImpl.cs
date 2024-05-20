using FixtureManagementBLL.UtilTool;
using FixtureManagementModel;
using FixtureManagementModel.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FixtureManagementBLL.Service.Impl
{
    public class StandardModbusImpl : IStandardModbusService
    {
        public int obtainModbus(devicePlcEntity plcEntity)
        {
            hcPlcEntity.PlcRequest entity=new hcPlcEntity.PlcRequest();

            entity.gatherPlcIp = plcEntity.devicePlcIp;

            entity.gatherPlcPort=int.Parse(plcEntity.devicePlcPort);

            entity.gatherPlcIo = plcEntity.devicePlcIo;

            return HCPlcUtil.getPlcData(entity);
        }

        public bool sendModeus(int fockState,devicePlcEntity plcEntity)
        {
            hcPlcEntity.PlcRequest entity = new hcPlcEntity.PlcRequest();

            entity.gatherPlcIp = plcEntity.devicePlcIp;

            entity.gatherPlcPort = int.Parse(plcEntity.devicePlcPort);

            entity.gatherPlcIo = plcEntity.devicePlcIo;

            entity.gatherPlcIoState = fockState;

            return HCPlcUtil.postPlcData(entity); 
        }
    }
}
