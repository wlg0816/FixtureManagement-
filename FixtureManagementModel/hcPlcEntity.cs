using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementModel
{
    public class hcPlcEntity
    {
        public class PlcRequest
        {
            /// <summary>
            /// PLC  IP地址
            /// </summary>
            public string gatherPlcIp { get; set; }
            /// <summary>
            /// PLC端口号
            /// </summary>
            public int gatherPlcPort { get; set; }
            /// <summary>
            /// 点位地址码
            /// </summary>
            public string gatherPlcIo { get; set; }
            /// <summary>
            /// 工装状态对应PLC值
            /// </summary>
            public int gatherPlcIoState { get; set; }
        }
    }
}
