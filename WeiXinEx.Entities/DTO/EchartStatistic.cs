using System;
using System.Collections.Generic;
using System.Text;

namespace WeiXinEx.Entities
{
    public class EChartStatistic
    {
        public DateTime Date { get; set; }
        public int MessageRecv { get; set; }
        public int MessageSend { get; set; }
        public int SessionCount { get; set; }
        public int OnlineTime { get; set; }
        public int Created { get; set; }
        public int Completed { get; set; }
        public int EmployeeCount { get; set; }
    }
}
