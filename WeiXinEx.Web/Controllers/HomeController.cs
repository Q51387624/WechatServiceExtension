using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeiXinEx.Web.Models;
using WeiXinEx.Entities;
using WeiXinEx.Application;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Serialization;

namespace WeiXinEx.Web.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            var data = CommonApplication.GetHome();
            return View(data);
        }

        [HttpPost]
        public IActionResult JsonMonthStatistic()
        {
            var data = StatisticApplication.LastMonthStatistic();
            return Json(data, new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(string username, string password) {
            var settings = CommonApplication.Settings;
            if (CheckUsername(username, password))
            {
                var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, username) }, CookieAuthenticationDefaults.AuthenticationScheme));
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromDays(180))
                });
                return Json(new { success = true });
            }
            return Json(new { success = false });
           
        }

        private bool CheckUsername(string username, string password)
        {
            var settings = CommonApplication.Settings;
            if (username == "admin" && password == settings.Password
                || username == "viewer" && password == settings.ViewerPassword)
                return true;
            return false;
        }

        [AllowAnonymous]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        [AllowAnonymous]
        public IActionResult Denied()
        {
            return Json(new { success = false, msg = "授权失败, 禁止访问" });
        }

        [Authorize(Roles = "admin")]
        public IActionResult Client() {
            return File("~/extension/client.rar", "application/octet-stream", "客户端.rar");
        }
        [Authorize(Roles ="admin")]
        public IActionResult Settings()
        {
            return View(CommonApplication.Settings);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Settings(Settings settings)
        {
            CommonApplication.SaveSettings(settings);
            return View(CommonApplication.Settings);
        }
    }
}
