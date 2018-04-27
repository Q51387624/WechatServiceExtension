using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeiXinEx.Web.Models
{
    public class WXOnline
    {
        public long Token { get; set; }

        public long Bizuin { get; set; }
        public string BizNickname { get; set; }

        public long Kfuin { get; set; }

        public string Kfwx { get; set; }

        public string KfNickname { get; set; }
    }
}
