using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeiXinEx.Application;
using WeiXinEx.Entities;
using WeiXinEx.Web.Models;

namespace WeiXinEx.Web.Controllers
{
    [Route("receive")]
    [Authorize("Deadline")]
    [Produces("application/json")]
    public class ReceiveController : Controller
    {
        /// <summary>
        /// 提交聊天记录
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        [HttpPost("messages")]
        public object PostMessages(List<WXMessage> messages)
        {
            var list = messages.Select(item => new Message
            {
                BizUId = item.Bizuin,
                Content = item.Content??string.Empty,
                CreateTime = ToDateTime(item.CreateTime),
                EmployeeUId = item.Kfuin,
                MessageId = item.MsgId,
                Type = item.Type,
                UserUId = item.Useruin,
                MessageType = item.MsgType,
            }).ToList();
            var count= MessageApplication.Save(list);
            return new { success = true, count = count };
        }
        [HttpPost("users")]
        public object PostUser(List<WXUser> users)
        {
            foreach (var user in users)
                CommonApplication.SaveUser(user.Useruin, user.Nickname);
            return new { success = true };
        }
       
        /// <summary>
        /// 提交客服统计
        /// </summary>
        /// <param name="statistics"></param>
        /// <returns></returns>
        [HttpPost("statistic")]
        public object PostStatistic(long bizuin, long kfuin,List<WXStatistic> statistics,WXSessions sessions)
        {
            if (bizuin == 0 || kfuin == 0)
                return new { success = false, msg = "无效的公众号或客服ID" };

            CommonApplication.Online(bizuin, kfuin);
            var list1 = statistics.Select(item => new EmployeeStatistic
            {
                Date = ToDateTime(item.Date),
                MessageRecv = item.Msg_Recv,
                MessageSend = item.Msg_Send,
                OnlineTime = item.Online_Time,
                SessionCount = item.Session_Count,
                UId = item.Kf_uin,
                BId = item.Bizuin,
            }).ToList();
            var session = new SessionStatic
            {
                Request = sessions.Request,
                Created = sessions.Created,
                Completed = sessions.Success
            };
            StatisticApplication.Save(list1, session);

            var createds = sessions.CreatedUsers.Select(item => new UserSession
            {
                Bizuin = bizuin,
                Useruin = item.Useruin,
                Time = ToDateTime(item.Time),
                Delay = item.Delay,
                IsNew = true,
                Completed = false,
            }).ToList();

            var success = sessions.SuccessUsers.Select(item => new UserSession
            {
                Bizuin = bizuin,
                Useruin = item.Useruin,
                Time = ToDateTime(item.Time),
                Delay = item.Delay,
                IsNew = true,
                Completed = true,
            }).ToList();
            StatisticApplication.SaveSessions(bizuin, createds, success);

            return new { success = true };
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("settings")]
        public object Settings()
        {
            var setting = CommonApplication.Settings;
            var keywords = setting.Keywords
               .Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(p => p.Trim())
               .ToList();
            return new
            {
                success = true,
                settings = new
                {
                    monitorInterval = setting.MonitorInterval,
                    enable_session= setting.EnableSession,
                    setting.MonitorStatisticInterval,
                    monitorStatisticRange = 30,
                    auto_reply = setting.AutoReply,
                    auto_count = setting.AutoMaxIndex,
                    keywords
                }
            };
        }
        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("online")]
        public object Online(WXOnline online)
        {
            CommonApplication.SaveBusiness(online.Bizuin, online.BizNickname);
            CommonApplication.SaveEmployee(online.Bizuin, online.Kfuin, online.Kfwx, online.KfNickname);
            return new { success = true };
        }

        [HttpGet("modules")]
        public object Modules(long id)
        {
            var employee = CommonApplication.GetEmployee(id);
            if (employee == null || !employee.Enable)
                return File("/extension/uauthorized.js", "application/javascript;charset=utf-8");
            else
                return File("/extension/modules.min.js", "application/javascript;charset=utf-8");
        }
        private DateTime ToDateTime(long time)
        {
            return new DateTime(1970, 1, 1, 8, 0, 0).AddSeconds(time);
        }

    }
}