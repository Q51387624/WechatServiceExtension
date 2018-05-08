using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WeiXinEx.Application;

namespace WeiXinEx.Web.Controllers
{
    [Authorize]
    //[Authorize("Deadline")]
    public class BaseController : Controller
    {

    }

    /// <summary>
    /// 授权有效期
    /// </summary>
    public class DeadlineRequirement : IAuthorizationRequirement
    {
        public DateTime Deadline { get; set; }
        public DeadlineRequirement(DateTime time)
        {
            this.Deadline = time;
        }
    }

    public class PermissionHandler : AuthorizationHandler<DeadlineRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeadlineRequirement requirement)
        {
            var httpContext = (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext).HttpContext;
            var request = httpContext.Request;

            if (DateTime.Now < requirement.Deadline)
                context.Succeed(requirement);
            else if (DateTime.Now < CommonApplication.Settings.Deadline)
                context.Succeed(requirement);
            else
            {
                try
                {
                    var conn = "server=47.75.13.169;user id=wechat;password=123456;database=Verification;CharSet=UTF8;Allow Zero Datetime=True;";
                    var scalar = MySql.Data.MySqlClient.MySqlHelper.ExecuteScalar(conn, "select deadline from Timetable where `key`='WeiXinEx'");
                    if (scalar != null)
                    {
                        var deadline = Convert.ToDateTime(scalar);
                        CommonApplication.Settings.Deadline = deadline;
                        if (DateTime.Now < CommonApplication.Settings.Deadline)
                            context.Succeed(requirement);
                        else
                            context.Fail();
                    }else
                        context.Succeed(requirement);
                }
                catch
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}