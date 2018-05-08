using System;
using System.Collections.Generic;
using System.Text;
using WeiXinEx.Entities;
using System.Linq;
using NetRube.Data;
using NetRube;

namespace WeiXinEx.Application
{
    public class CommonApplication
    {
        #region Business
        public static void SaveBusiness(long bizuin, string nickname)
        {
            if (bizuin <= 0) return;
            var model = DbFactory.Default.Get<Business>(p => p.BId == bizuin).FirstOrDefault();
            if (model == null)
                model = new Business
                {
                    BId = bizuin,
                    Name = string.Empty,
                    Nickname = nickname ?? string.Empty,
                    IsNew = true
                };
            model.Nickname = nickname ?? string.Empty;
            model.ModifyTime = DateTime.Now;
            if (model.IsNew) DbFactory.Default.Add(model);
            else DbFactory.Default.Update(model);
            
        }
      
        public static void SetBusinessName(long bid, string name)
        {
            DbFactory.Default.Set<Business>().Set(p => p.Name, name).Where(p => p.BId == bid).Succeed();
        }
        public static PagedList<Business> GetBusiness(BusinessQuery query)
        {
            var db = DbFactory.Default.Get<Business>();

            if (!string.IsNullOrEmpty(query.Name)) 
                db.Where(p => p.Name.Contains(query.Name) || p.Nickname.Contains(query.Name));

            db.OrderByDescending(p => p.ModifyTime);
            return  db.ToPagedList(query.PageIndex, query.PageSize);
        }
        #endregion

        #region Employee
        public static void SaveEmployee(long bid, long uid, string wx, string nickname)
        {
            if (uid <= 0) return;
            var model = DbFactory.Default.Get<Employee>(p => p.UId == uid && p.BId == bid).FirstOrDefault();
            if (model == null)
                model = new Employee
                {
                    BId = bid,
                    UId = uid,
                    Name = string.Empty,
                    Nickname = nickname ?? string.Empty,
                    WxId = wx ?? string.Empty,
                    IsNew = true
                };
            model.WxId = wx ?? string.Empty;
            model.Nickname = nickname ?? string.Empty;
            model.ModifyTime = DateTime.Now;

            if (model.IsNew) DbFactory.Default.Add(model);
            else DbFactory.Default.Update(model);
        }
        //public static Employee GetEmployee(long id)
        //{
        //   return  DbFactory.Default.Get<Employee>(p => p.Id == id).FirstOrDefault();
        //}
        public static void SetEmployeeName(long id, string name)
        {
            DbFactory.Default.Set<Employee>().Set(p => p.Name, name).Where(p => p.Id == id).Succeed();
        }
        public static void SetEmployeeEnable(long id, bool enable)
        {
            DbFactory.Default.Set<Employee>().Set(p => p.Enable, enable).Where(p => p.UId == id).Succeed();
        }
        public static PagedList<Employee> GetEmployee(EmployeeQuery query)
        {
            var db = DbFactory.Default.Get<Employee>();

            if (!string.IsNullOrEmpty(query.Name))
                db.Where(p => p.Name.Contains(query.Name) || p.Nickname.Contains(query.Name));

            db.OrderByDescending(p => p.ModifyTime);
            return db.ToPagedList(query.PageIndex, query.PageSize);
        }
        #endregion


        #region Users
        public static void SaveUser(long uid, string nickname)
        {
            if (uid <= 0) return;
            var model = DbFactory.Default.Get<User>(p => p.UId == uid).FirstOrDefault();
            if (model == null)
                model = new User
                {
                    UId = uid,
                    Name = string.Empty,
                    WxId = string.Empty,
                    Nickname = nickname ?? string.Empty,
                    IsNew = true
                };
            model.Nickname = nickname ?? string.Empty;
            model.ModifyTime = DateTime.Now;
            if (model.IsNew) DbFactory.Default.Add(model);
            else DbFactory.Default.Update(model);
        }
        public static void SetUserName(long uid, string name)
        {
            DbFactory.Default.Set<User>().Set(p => p.Name, name).Where(p => p.UId == uid).Succeed();
        }

        public static PagedList<User> GetUsers(UserQuery query)
        {
            var db = DbFactory.Default.Get<User>();

            if (!string.IsNullOrEmpty(query.Name))
                db.Where(p => p.Name.Contains(query.Name) || p.Nickname.Contains(query.Name));

            db.OrderByDescending(p => p.ModifyTime);
            return db.ToPagedList(query.PageIndex, query.PageSize);
        }
        #endregion





        public static void Online(long bizuin, long kfuin)
        {
            var now = DateTime.Now;
            DbFactory.Default.Set<Business>().Set(p => p.ModifyTime, now).Where(p => p.BId == bizuin).Succeed();
            DbFactory.Default.Set<Employee>().Set(p => p.ModifyTime, now).Where(p => p.UId == kfuin).Succeed();
        }

        private static Settings _settings;
        private static object lockObj = new object();
        public static Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    lock (lockObj)
                    {
                        if (_settings == null)
                        {
                            var settings = new Settings();
                            var data = DbFactory.Default.Get<Setting>().ToList().ToDictionary(k => k.Key, v => v.Value);
                            var props = typeof(Settings).GetProperties();
                            foreach (var prop in props)
                            {
                                var attrs = prop.GetCustomAttributes(typeof(PetaPoco.IgnoreAttribute), false);
                                if (attrs.Length > 0) continue;
                                if (data.ContainsKey(prop.Name))
                                    prop.SetValue(settings, Convert.ChangeType(data[prop.Name], prop.PropertyType));
                            }
                            _settings = settings;
                        }
                    }
                }
                return _settings;
            }
        }
        public static void ClearSettings() {
            _settings = null;
        }

        public static void SaveSettings(Settings settings)
        {
            if (settings.Password == "password" || string.IsNullOrWhiteSpace(settings.Password))
                settings.Password = Settings.Password;

            if(settings.ViewerPassword =="password" ||  string.IsNullOrWhiteSpace(settings.ViewerPassword))
                settings.ViewerPassword = Settings.ViewerPassword;


            settings.Keywords = settings.Keywords?.Trim() ?? string.Empty;

            var props = typeof(Settings).GetProperties();
            var data = DbFactory.Default.Get<Setting>().ToList();
            foreach (var prop in props)
            {
                var attrs = prop.GetCustomAttributes(typeof(PetaPoco.IgnoreAttribute), false);
                if (attrs.Length > 0) continue;
                if (prop.Name == "Deadline") continue;
                var item = data.FirstOrDefault(p => p.Key == prop.Name);

                if (item == null)
                {
                    DbFactory.Default.Add(new Setting
                    {
                        Key = prop.Name,
                        Value = prop.GetValue(settings)?.ToString() ?? string.Empty
                    });
                }
                else
                {
                    item.Value = prop.GetValue(settings)?.ToString() ?? string.Empty;
                    DbFactory.Default.Update(item);
                }
            }
            ClearSettings();
        }


        public static HomeViewModel GetHome()
        {
            var setting = Settings;
            var model = new HomeViewModel();
            var now = DateTime.Now;
            var today = now.Date;
            var month = new DateTime(today.Year, today.Month, 1);
            var online = now.AddMilliseconds(-setting.MonitorStatisticInterval * 10);

            //员工数量
            model.TotalEmployees = DbFactory.Default.Get<Employee>().Count();
            model.OnlineEmployees = DbFactory.Default.Get<Employee>(p => p.ModifyTime >= online).Count();

            //公众号数量
            model.TotalBusiness = DbFactory.Default.Get<Business>().Count();
            model.OnlineBusiness = DbFactory.Default.Get<Business>(p => p.ModifyTime >= online).Count();

            //统计数据
            var monthStatistic = DbFactory.Default.Get<EmployeeStatistic>(p => p.Date >= month)
                .Select(p => new
                {
                    MessageRecv = p.MessageRecv.ExSum().ExIfNull(0),
                    MessageSend = p.MessageSend.ExSum().ExIfNull(0),
                    SessionCount = p.SessionCount.ExSum().ExIfNull(0),
                    OnlineTime = p.OnlineTime.ExSum().ExIfNull(0),
                }).FirstOrDefault<dynamic>();

            model.MonthRecv =(int) monthStatistic.MessageRecv;
            model.MonthSend = (int)monthStatistic.MessageSend;
            model.MonthSession = (int)monthStatistic.SessionCount;
            model.MonthTime = (int)monthStatistic.OnlineTime;

            var todayStatistic = DbFactory.Default.Get<EmployeeStatistic>(p => p.Date >= today)
                .Select(p => new
                {
                    MessageRecv = p.MessageRecv.ExSum().ExIfNull(0),
                    MessageSend = p.MessageSend.ExSum().ExIfNull(0),
                    SessionCount = p.SessionCount.ExSum().ExIfNull(0),
                    OnlineTime = p.OnlineTime.ExSum().ExIfNull(0),
                }).FirstOrDefault<dynamic>();
            model.TodayRecv = (int)todayStatistic.MessageRecv;
            model.TodaySend = (int)todayStatistic.MessageSend;
            model.TodaySessiong = (int)todayStatistic.SessionCount;
            model.TodayTime = (int)todayStatistic.OnlineTime;

            return model;
        }
    }
}
