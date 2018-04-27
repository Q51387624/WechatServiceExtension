using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeiXinEx.Web.Models
{
    public class WXMessage
    {
        public int MsgId { get; set; }
        public long Bizuin { get; set; }
        public long Useruin { get; set; }
        public long Kfuin { get; set; }
        public long CreateTime { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 1:文本消息 34:语音消息
        /// </summary>
        public int MsgType { get; set; }
        /// <summary>
        /// 1:客服发送 2:客户发送
        /// </summary>
        public int Type { get; set; }
        public string Kf_openid { get; set; }
    }
}
