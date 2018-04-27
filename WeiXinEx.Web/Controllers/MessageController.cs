using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeiXinEx.Entities;
using WeiXinEx.Application;
using System.IO;
using NPOI.HSSF.UserModel;

namespace WeiXinEx.Web.Controllers
{
    public class MessageController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List(MessageQuery query)
        {
            if (query.End.HasValue)
                query.End = query.End.Value.AddDays(1);
            var data = MessageApplication.GetMessages(query);
            var message = MessageApplication.ToMessages(data);
            return Json(new { success = true, data = new { rows = message, total = data.TotalRecordCount } });
        }

        public IActionResult Export(MessageQuery query)
        {
            if (query.End.HasValue)
                query.End = query.End.Value.AddDays(1);
            var data = MessageApplication.GetMessagesAll(query);
            var messages = MessageApplication.ToMessages(data);
            var workboox = new HSSFWorkbook();
            var sheet = (HSSFSheet)workboox.CreateSheet("客服统计"); //创建工作表
            sheet.CreateFreezePane(0, 1); //冻结列头行
            var header = (HSSFRow)sheet.CreateRow(0); //创建列头行


            header.CreateCell(0).SetCellValue("公众号");
            header.CreateCell(1).SetCellValue("客服");
            header.CreateCell(2).SetCellValue("客户");
            header.CreateCell(3).SetCellValue("类型");
            header.CreateCell(4).SetCellValue("时间");
            header.CreateCell(5).SetCellValue("内容");

            var index = 1;
            foreach (var message in messages)
            {
                var row = sheet.CreateRow(index++);
                row.CreateCell(0).SetCellValue(message.BusinessName);
                row.CreateCell(1).SetCellValue(message.EmployeeName);
                row.CreateCell(2).SetCellValue(message.UserName);
                row.CreateCell(3).SetCellValue(message.Type == 1 ? "回复" : "收到");
                row.CreateCell(4).SetCellValue(message.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                row.CreateCell(5).SetCellValue(message.Content);
            }

            var stream = new MemoryStream();
            workboox.Write(stream);
            stream.Flush();
            var file = stream.ToArray();
            stream.Close();
            return File(file, "application/ms-excel", "聊天记录.xls");
        }
    }
}