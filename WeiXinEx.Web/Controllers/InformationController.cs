using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeiXinEx.Entities;
using WeiXinEx.Application;
using Microsoft.AspNetCore.Authorization;

namespace WeiXinEx.Web.Controllers
{
    public class InformationController : BaseController
    {
        #region Business
        public IActionResult Business() {
            return View();
        }

        public IActionResult BusinessList(BusinessQuery query) {
           
            var data = CommonApplication.GetBusiness(query);
            return Json(new { success = true, data = new { rows = data, total = data.TotalRecordCount } });
        }

        public IActionResult SetBusinessName(long id, string name)
        {
            CommonApplication.SetBusinessName(id, name);
            return Json(new { success = true });
        }
        #endregion


        #region Employee

        public IActionResult Employees()
        {
            return View();
        }

        public IActionResult EmployeeList(EmployeeQuery query)
        {
            var data = CommonApplication.GetEmployee(query);
            return Json(new { success = true, data = new { rows = data, total = data.TotalRecordCount } });
        }

        public IActionResult SetEmployeeName(long id, string name)
        {
            CommonApplication.SetEmployeeName(id, name);
            return Json(new { success = true });
        }

        [Authorize(Roles = "admin")]
        public IActionResult SetEmployeeEnable(long id, bool enable)
        {
            CommonApplication.SetEmployeeEnable(id, enable);
            return Json(new { success = true });
        }
        #endregion

        #region User
        public IActionResult Users()
        {
            return View();
        }

        public IActionResult UserList(UserQuery query)
        {
            var data = CommonApplication.GetUsers(query);
            return Json(new { success = true, data = new { rows = data, total = data.TotalRecordCount } });
        }

        public IActionResult SetUserName(long id, string name)
        {
            CommonApplication.SetUserName(id, name);
            return Json(new { success = true });
        }
        #endregion
    }
}