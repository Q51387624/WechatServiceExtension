using System;
using System.Collections.Generic;
using System.Text;

namespace WeiXinEx.Entities
{
    public class HomeViewModel
    {
        public int TodayRecv { get; set; }
        public int TodaySend { get; set; }
        public int TodaySessiong { get; set;}
        public int TodayTime { get; set; }


        /// <summary>
        /// 接受消息
        /// </summary>
        public int MonthRecv { get; set; }
        /// <summary>
        /// 回复消息
        /// </summary>
        public int MonthSend { get; set; }
        /// <summary>
        /// 接待人数
        /// </summary>
        public int MonthSession { get; set; }
        /// <summary>
        /// 总在线时长
        /// </summary>
        public int MonthTime { get; set; }

        /// <summary>
        /// 在线客服
        /// </summary>
        public int OnlineEmployees { get; set; }

        /// <summary>
        /// 总员工数
        /// </summary>
        public int TotalEmployees { get; set; }

        public string PerEmployess
        {
            get
            {
                var total = (decimal)TotalEmployees;
                if (total <= 0) total = 1;
                var pre = OnlineEmployees / total*100;
                return string.Format("{0:n2}%", pre);
            }
        }
        /// <summary>
        /// 在线公众号
        /// </summary>
        public int OnlineBusiness { get; set; }

        /// <summary>
        /// 总公众号
        /// </summary>
        public int TotalBusiness { get; set; }

        public string PerBusiness
        {
            get
            {
                var total = (decimal)TotalBusiness;
                if (total <= 0) total = 1;
                var pre = OnlineBusiness / total*100;
                return string.Format("{0:n2}%", pre);
            }
        }

    }
}
