using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementModel
{
    public class frockLifeInfoEntity
    {
        /// <summary>
        /// 工装名称
        /// </summary>
        public string frockName { get; set; }
        /// <summary>
        /// 工装流水号
        /// </summary>
        public string frockSn { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string eqpName { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string eqpSn { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 员工工号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 今日使用
        /// </summary>
        public decimal usedToday { get; set; }
        /// <summary>
        ///  累计使用
        /// </summary>
        public decimal lifeNumConsumption { get; set; }
        /// <summary>
        /// 本次使用
        /// </summary>
        public decimal usedCurrent { get; set; }
        /// <summary>
        /// 剩余次数
        /// </summary>
        public decimal remainLifeNum { get; set; }
        /// <summary>
        /// 剩余次数百分比
        /// </summary>
        public int remainingLife { get; set; }
        /// <summary>
        /// 累计增加寿命
        /// </summary>
        public decimal lifeNumAdd { get; set; }
        /// <summary>
        /// 生命周期基准
        /// </summary>
        public decimal initialLifeNum { get; set; }
    }
}
