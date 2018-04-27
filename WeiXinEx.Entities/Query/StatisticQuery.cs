using System;
using System.Collections.Generic;
using System.Text;

namespace WeiXinEx.Entities
{
    public class StatisticQuery:BaseQuery
    {
        /// <summary>
        /// 公众号名
        /// </summary>
        public string BusinessName { get; set; }
        /// <summary>
        /// 员工名
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? Begin { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? End { get; set; }
    }
}
