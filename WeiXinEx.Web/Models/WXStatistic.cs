using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeiXinEx.Web.Models
{
    public class WXStatistic
    {
        public long Date { get; set; }
        public long Kf_uin { get; set; }
        public long Bizuin { get; set; }

        public int Online_Time { get; set; }

        public int Session_Count { get; set; }

        public int Msg_Send { get; set; }

        public int Msg_Recv { get; set; }
    }

}
