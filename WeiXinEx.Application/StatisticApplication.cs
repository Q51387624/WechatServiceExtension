using System;
using System.Collections.Generic;
using WeiXinEx.Entities;
using System.Linq;
using NetRube.Data;
using NetRube;
using System.Text.RegularExpressions;

namespace WeiXinEx.Application
{
          
    public class StatisticApplication
    {
        public static void Save(List<EmployeeStatistic> statistics,SessionStatic session)
        {
            var first = statistics.FirstOrDefault();
            if (first == null) return;//无可用数据
            var minData = statistics.Min(p => p.Date);

            var data = DbFactory.Default.Get<EmployeeStatistic>(p => p.BId == first.BId && p.UId == first.UId && p.Date >= minData).ToList();
            foreach (var item in statistics)
            {
                var model = data.FirstOrDefault(p => p.Date == item.Date);
                if (model == null)
                    model = new EmployeeStatistic();

                model.Date = item.Date;
                model.BId = item.BId;
                model.UId = item.UId;
                model.MessageRecv = item.MessageRecv;
                model.MessageSend = item.MessageSend;
                model.OnlineTime = item.OnlineTime;
                model.SessionCount = item.SessionCount;

                if (model.Date == DateTime.Now.Date)
                {
                    model.Request += session.Request;
                    model.Created += session.Created;
                    model.Completed += session.Completed;
                }

                if (model.Id > 0)
                    DbFactory.Default.Update(model);
                else
                    DbFactory.Default.Add(model);
            }
        }

        public static void SaveSessions(long bizuin, List<UserSession> created, List<UserSession> success)
        {
            var today = DateTime.Now.Date;
            var users = created.Select(p => p.Useruin).ToList();
            users.AddRange(success.Select(p => p.Useruin));
            users = users.Distinct().ToList();
            var data = DbFactory.Default.Get<UserSession>(p => p.Bizuin == bizuin && p.Time > today && p.Useruin.ExIn(users)).OrderByDescending(p => p.Time).ToList();
            foreach (var item in created)
            {
                var i = data.FirstOrDefault(p => p.Useruin == item.Useruin);
                if (i == null)
                {
                    data.Add(item);
                    DbFactory.Default.Add(item);
                }
            }

            foreach (var item in success)
            {
                var i = data.FirstOrDefault(p => p.Useruin == item.Useruin);
                if (i == null)
                    DbFactory.Default.Add(item);
                else
                {
                    i.Time = item.Time;
                    i.Delay = item.Delay;
                    i.Completed = true;
                    DbFactory.Default.Update(item);
                }
            }
        }

        public static PagedList<EmployeeStatistic> GetStatistic(StatisticQuery query) {
            var db = Builder(query);
            db.OrderByDescending(p => p.Date);
            return db.ToPagedList(query.PageIndex, query.PageSize);
        }

        public static List<EmployeeStatistic> GetStatisticAll(StatisticQuery query)
        {
            var db = Builder(query);
            db.OrderByDescending(p => p.Date);
            return db.ToList();
        }

        public static List<StatisticDTO> ToStatistic(List<EmployeeStatistic> statistic)
        {
            var business = statistic.Select(p => p.BId).Distinct().ToList();
            var employee = statistic.Select(p => p.UId).Distinct().ToList();

            var businessList = DbFactory.Default.Get<Business>(p => p.BId.ExIn(business)).ToList();
            var employeeList = DbFactory.Default.Get<Employee>(p => p.UId.ExIn(employee)).ToList();

            return statistic.Select(item =>
            {
                var b = businessList.FirstOrDefault(p => p.BId == item.BId);
                var bn = b?.Name ?? string.Empty;
                if (string.IsNullOrEmpty(bn)) bn = b?.Nickname ?? string.Empty;

                var e = employeeList.FirstOrDefault(p => p.UId == item.UId);
                var en = e?.Name ?? string.Empty;
                if (string.IsNullOrEmpty(en)) en = e?.Nickname ?? string.Empty;

                return new StatisticDTO
                {
                    BId = item.BId,
                    UId = item.UId,
                    Date = item.Date,
                    Id = item.Id,
                    MessageRecv = item.MessageRecv,
                    MessageSend = item.MessageSend,
                    OnlineTime = item.OnlineTime,
                    SessionCount = item.SessionCount,
                    EmployeeName = en,
                    BusinessName = bn,
                };
            }).ToList();
        }

        private static GetBuilder<EmployeeStatistic> Builder(StatisticQuery query)
        {
            var db = DbFactory.Default.Get<EmployeeStatistic>();
            if (!string.IsNullOrEmpty(query.BusinessName))
            {
                var business = DbFactory.Default.Get<Business>(p => p.Name.Contains(query.BusinessName) || p.Nickname.Contains(query.BusinessName)).Select(p => p.BId).ToList<long>();
                db.Where(p => p.BId.ExIn(business));
            }
            if (!string.IsNullOrEmpty(query.EmployeeName))
            {
                var employee = DbFactory.Default.Get<Employee>(p => p.Name.Contains(query.EmployeeName) || p.Nickname.Contains(query.EmployeeName)).Select(p => p.UId).ToList<long>();
                db.Where(p => p.UId.ExIn(employee));
            }
          

            if (query.Begin.HasValue)
                db.Where(p => p.Date >= query.Begin.Value);
            if (query.End.HasValue)
                db.Where(p => p.Date < query.End.Value);

            return db;
        }
        /// <summary>
        /// 最近30天状态统计
        /// </summary>
        public static LineChartDataModel<int> LastMonthStatistic()
        {
            var begin = DateTime.Now.Date.AddDays(-30);
            var data = DbFactory.Default.Get<EmployeeStatistic>(p => p.Date >= begin)
                .GroupBy(p => p.Date)
                .Select(p => new
                {
                    p.Date,
                    MessageRecv = p.MessageRecv.ExSum(),
                    MessageSend = p.MessageSend.ExSum(),
                    SessionCount = p.SessionCount.ExSum(),
                    OnlineTime = p.OnlineTime.ExSum(),
                    Created = p.Created.ExSum(),
                    Completed = p.Completed.ExSum(),
                    EmployeeCount = p.UId.ExCount(true)
                }).ToList<EChartStatistic>();

            var chart = new LineChartDataModel<int>();
            var seriesRecv = new ChartSeries<int> { Name = "收到消息" };
            var seriesSend = new ChartSeries<int> { Name = "发送消息" };
            var seriesSession = new ChartSeries<int> { Name = "接待客户" };
            var seriesTime = new ChartSeries<int> { Name = "在线时长" };
            var seriesCreated = new ChartSeries<int> { Name = "来访客户数" };
            var seriesCompleted = new ChartSeries<int> { Name = "抢单成功" };
            var seriesEmployee = new ChartSeries<int> { Name = "上线员工" };
            chart.SeriesData.Add(seriesRecv);
            chart.SeriesData.Add(seriesSend);
            chart.SeriesData.Add(seriesSession);
            chart.SeriesData.Add(seriesTime);
            chart.SeriesData.Add(seriesCreated);
            chart.SeriesData.Add(seriesCompleted);
            chart.SeriesData.Add(seriesEmployee);
            for (var date = begin; date < DateTime.Now; date = date.AddDays(1))
            {
                var item = data.FirstOrDefault(p => p.Date == date);
                if (item == null) item = new EChartStatistic();
                chart.XAxisData.Add(date.ToString("MM月dd日"));
                seriesRecv.Data.Add(item.MessageRecv);
                seriesSend.Data.Add(item.MessageSend);
                seriesSession.Data.Add(item.SessionCount);
                seriesTime.Data.Add(item.OnlineTime);
                seriesCreated.Data.Add(item.Created);
                seriesCompleted.Data.Add(item.Completed);
                seriesEmployee.Data.Add(item.EmployeeCount);
            }
            return chart;
        }
    }
}
