using PetaPoco;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace WeiXinEx.Entities
{
    public class Settings
    {
        public string Password { get; set; } = "admin888";
        public string ViewerPassword { get; set; } = "888888";
        public int MonitorInterval { get; set; } = 500;
        public int MonitorStatisticInterval { get; set; } = 30000;
        public bool AutoReply { get; set; } = false;
        public int AutoMaxIndex { get; set; } = 4;

        public string Keywords { get; set; } = string.Empty;

        public DateTime Deadline { get; set; } = DateTime.MinValue;



    }
}
