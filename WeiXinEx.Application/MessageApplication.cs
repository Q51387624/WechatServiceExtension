using System;
using System.Collections.Generic;
using System.Text;
using WeiXinEx.Entities;
using System.Linq;
using NetRube.Data;
using NetRube;
using WeiXinEx.Entities.DTO;

namespace WeiXinEx.Application
{
    public class MessageApplication
    {
        public static int Save(List<Message> messages)
        {
            var first = messages.FirstOrDefault();
            if (first == null) return 0;
            var exist = DbFactory.Default.Get<Message>(p => p.BizUId == first.BizUId && p.UserUId == first.UserUId)
                .OrderByDescending(p => p.MessageId)
                .Select(p=>p.MessageId)
                .ToList<int>();
            var list = messages.Where(p => !exist.Contains(p.MessageId)).ToList();
            DbFactory.Default.AddRange(list);
            return list.Count;
        }

        public static PagedList<Message> GetMessages(MessageQuery query)
        {
            var db = Builder(query);
            db.OrderByDescending(p => p.CreateTime);
            return db.ToPagedList(query.PageIndex, query.PageSize);
        }
        public static List<Message> GetMessagesAll(MessageQuery query)
        {
            var db = Builder(query);
            db.OrderBy(p => p.UserUId).OrderBy(p => p.CreateTime);
            return db.ToList();
        }

        public static List<MessageDTO> ToMessages(List<Message> messages)
        {
            var business = messages.Select(p => p.BizUId).Distinct().ToList();
            var employee = messages.Select(p => p.EmployeeUId).Distinct().ToList();
            var user = messages.Select(p => p.UserUId).Distinct().ToList();

            var businessList = DbFactory.Default.Get<Business>(p => p.BId.ExIn(business)).ToList();
            var employeeList = DbFactory.Default.Get<Employee>(p => p.UId.ExIn(employee)).ToList();
            var userList = DbFactory.Default.Get<User>(p => p.UId.ExIn(user)).ToList();

            return messages.Select(item =>
            {
                var b = businessList.FirstOrDefault(p => p.BId == item.BizUId);
                var bn = b?.Name ?? string.Empty;
                if (string.IsNullOrEmpty(bn)) bn = b?.Nickname ?? string.Empty;

                var e = employeeList.FirstOrDefault(p => p.UId == item.EmployeeUId);
                var en = e?.Name ?? string.Empty;
                if (string.IsNullOrEmpty(en)) en = e?.Nickname ?? string.Empty;

                var u = userList.FirstOrDefault(p => p.UId == item.UserUId);
                var un = u?.Name ?? string.Empty;
                if (string.IsNullOrEmpty(un)) un = u?.Nickname ?? string.Empty;

                return new MessageDTO
                {
                    BizUId = item.BizUId,
                    Content = item.Content,
                    UserUId = item.UserUId,
                    CreateTime = item.CreateTime,
                    EmployeeUId = item.EmployeeUId,
                    MessageId = item.MessageId,
                    Id = item.Id,
                    MessageType = item.MessageType,
                    Type = item.Type,
                    UserName = un,
                    EmployeeName = en,
                    BusinessName = bn
                };
            }).ToList();
        }

        
        private static GetBuilder<Message> Builder(MessageQuery query) {
            var db = DbFactory.Default.Get<Message>();
            if (!string.IsNullOrEmpty(query.BusinessName)) {
                var business = DbFactory.Default.Get<Business>(p => p.Name.Contains(query.BusinessName) || p.Nickname.Contains(query.BusinessName)).Select(p => p.BId).ToList<long>();
                db.Where(p => p.BizUId.ExIn(business));
            }
            if (!string.IsNullOrEmpty(query.EmployeeName)) {
                var employee = DbFactory.Default.Get<Employee>(p => p.Name.Contains(query.EmployeeName) || p.Nickname.Contains(query.EmployeeName)).Select(p => p.UId).ToList<long>();
                db.Where(p => p.EmployeeUId.ExIn(employee));
            }
            if (!string.IsNullOrEmpty(query.UserName))
            {
                var users = DbFactory.Default.Get<User>(p => p.Name.Contains(query.UserName) || p.Nickname.Contains(query.UserName)).Select(p => p.UId).ToList<long>();
                db.Where(p => p.UserUId.ExIn(users));
            }

            if (query.Begin.HasValue) 
                db.Where(p => p.CreateTime >= query.Begin.Value);
            if (query.End.HasValue)
                db.Where(p => p.CreateTime < query.End.Value);

            return db;
        }
    }
}
