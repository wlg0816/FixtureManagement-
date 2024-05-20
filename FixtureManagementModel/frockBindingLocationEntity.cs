using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementModel
{
    [Table("frockBindingLocation")]
    public partial class frockBindingLocationEntity
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int dictKey { get; set; }
        [Required]
        public string dictValue { get; set; }
        public bool isDel { get; set; }
    }

    public class DeviceLocationEntity
    {
        /// <summary>
        /// 请求ID
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 绑定位置
        /// </summary>
        public string bindingLocationName { get; set; }
        /// <summary>
        /// 工装名字
        /// </summary>
        public string frockName { get; set; }
        /// <summary>
        /// 工装编码
        /// </summary>
        public string frockSn { get; set; }
        /// <summary>
        /// 剩余次数
        /// </summary>
        public decimal remainLifeNum { get; set; }
    }
}
