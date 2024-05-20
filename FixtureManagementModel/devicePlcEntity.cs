using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementModel
{
    [Table("devicePlcEntity")]
    public partial class devicePlcEntity
    {
        [Key]
        public int? id { get; set; }
        [Required]
        public string deviceCode { get; set; }
        [Required]
        public string devicePlcIp { get; set; }
        public string devicePlcPort { get; set; }
        public string devicePlcIo { get; set; }
        public DateTime? createDateTime { get; set; }
        public bool isDel { get; set; }
    }
}
