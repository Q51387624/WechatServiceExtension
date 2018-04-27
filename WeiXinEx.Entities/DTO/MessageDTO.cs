using System;
using System.Collections.Generic;
using System.Text;

namespace WeiXinEx.Entities.DTO
{
    public class MessageDTO:Message
    {
        public string BusinessName { get; set;}
        public string EmployeeName { get; set; }
        public string UserName { get; set; }
    }
}
