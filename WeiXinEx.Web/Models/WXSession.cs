using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeiXinEx.Web.Models
{
     public class WXSessions
    {
        public int Request { get; set; }
        public int Created { get; set; }
        public int Success { get; set; }
        public List<WXSessionItem> CreatedUsers { get; set; } = new List<WXSessionItem>();
        public List<WXSessionItem> SuccessUsers { get; set; } = new List<WXSessionItem>();
    }
    public class WXSessionItem
    {
        public int Useruin { get; set; }
        public long Time { get; set; }
        public int Delay { get; set; }
    }
}
