using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeiXinEx.Entities;
using WeiXinEx.Application;
using NPOI.HSSF.UserModel;
using System.IO;

namespace WeiXinEx.Web.Controllers
{
    public class StatisticController : BaseController
    {
        public IActionResult Index() {
            return View();
        }

        public IActionResult List(StatisticQuery query)
        {
            if (query.End.HasValue)
                query.End = query.End.Value.AddDays(1);
            var data = StatisticApplication.GetStatistic(query);
            var statistic = StatisticApplication.ToStatistic(data);
            return Json(new { success = true, data = new { rows = statistic, total = data.TotalRecordCount } });
        }

        public IActionResult Export(StatisticQuery query)
        {
            if (query.End.HasValue)
                query.End = query.End.Value.AddDays(1);
            var data = StatisticApplication.GetStatisticAll(query);
            var statistic = StatisticApplication.ToStatistic(data);
            var workboox = new HSSFWorkbook();
            var sheet = (HSSFSheet)workboox.CreateSheet("客服统计"); //创建工作表
            sheet.CreateFreezePane(0, 1); //冻结列头行
            var header = (HSSFRow)sheet.CreateRow(0); //创建列头行

            header.CreateCell(0).SetCellValue("客服名");
            header.CreateCell(1).SetCellValue("服务人数");
            header.CreateCell(2).SetCellValue("回复消息");
            header.CreateCell(3).SetCellValue("收到消息");
            //header.CreateCell(4).SetCellValue("在线时长");
            var index = 1;
            var employees = statistic.GroupBy(p => p.UId);
            foreach (var employee in employees)
            {

                var row = sheet.CreateRow(index++);
                row.CreateCell(0).SetCellValue(employee.FirstOrDefault().EmployeeName);
                row.CreateCell(1).SetCellValue(employee.Sum(p => p.SessionCount));
                row.CreateCell(2).SetCellValue(employee.Sum(p => p.MessageSend));
                row.CreateCell(3).SetCellValue(employee.Sum(p => p.MessageRecv));
                //row.CreateCell(4).SetCellValue(employee.Sum(p => p.OnlineTime));
            }
            var filename = string.Format("{0}-{1}客服数据统计.xls", query.Begin.Value.ToString("MM月dd日"), query.End.Value.ToString("MM月dd日"));
            var stream = new MemoryStream();
            workboox.Write(stream);
            stream.Flush();
            var file = stream.ToArray();
            stream.Close();
            return File(file, "application/ms-excel", filename);
        }
    }
}