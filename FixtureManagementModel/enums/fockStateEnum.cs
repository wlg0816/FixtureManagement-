using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixtureManagementModel.enums
{
    public enum fockStateEnum:int
    {
        /// <summary>
        /// 初始化
        /// </summary>
        Init=0,
        /// <summary>
        /// 即将
        /// </summary>
        Soon=1,
        /// <summary>
        /// 到达
        /// </summary>
        Already=2
    }
}
