using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementModel
{
    public class historicalSummaryEntity
    {
        public List<HistoricalTable> records { get; set; }

        public frockLifeInfoEntity frockInfo { get; set; }
    }

    public class HistoricalTable
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string eqpSn { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string eqpName { get; set; }
        /// <summary>
        /// 寿命变化
        /// </summary>
        public string lifeChange { get; set; }
    }
}
