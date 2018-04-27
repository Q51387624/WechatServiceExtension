using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;
using NetRube.Data;

namespace WeiXinEx.Entities
{
    public partial class Db : PetaPoco.Database
    {


        public partial class Record<T> where T : IModel
        {


            private Dictionary<string, bool> _modifiedColumns = new Dictionary<string, bool>();
            private void OnLoaded()
            {
                _modifiedColumns = new Dictionary<string, bool>();
                Editabled = false;
            }
            /// <summary>
            /// Modified Field Notification Func
            /// </summary>
            protected void MarkColumnModified(string column_name)
            {
                if (_modifiedColumns != null)
                    _modifiedColumns[column_name] = true;
            }
            /// <summary>
            /// Modified Fields List
            /// </summary>
            [ResultColumn]
            public IEnumerable<string> ModifiedColumns { get { return _modifiedColumns.Keys; } }
            /// <summary>
            /// Modified Complete
            /// </summary>
            public void ModifiedComplete() { _modifiedColumns = new Dictionary<string, bool>(); }
            /// <summary>
            /// Is Edit Mode
            /// </summary>
            protected bool Editabled = true;
            [ResultColumn]
            public bool IsNew { get; set; }

        }

    }

    /// <summary>
    /// Business
    /// </summary>
    [TableName("Business")]
    [PrimaryKey("BId", AutoIncrement = false)]
    [ExplicitColumns]
    public partial class Business : Db.Record<Business>, IModel
    {

        /// <summary>
        /// BId
        /// </summary>

        [Column]
        public long BId
        {
            get
            {
                return _BId;
            }
            set
            {
                _BId = value;
                MarkColumnModified("BId");
            }
        }
        long _BId;



        /// <summary>
        /// Name
        /// </summary>

        [Column]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                MarkColumnModified("Name");
            }
        }
        string _Name;



        /// <summary>
        /// Nickname
        /// </summary>

        [Column]
        public string Nickname
        {
            get
            {
                return _Nickname;
            }
            set
            {
                _Nickname = value;
                MarkColumnModified("Nickname");
            }
        }
        string _Nickname;



        /// <summary>
        /// ModifyTime
        /// </summary>

        [Column]
        public DateTime ModifyTime
        {
            get
            {
                return _ModifyTime;
            }
            set
            {
                _ModifyTime = value;
                MarkColumnModified("ModifyTime");
            }
        }
        DateTime _ModifyTime;




    }

    /// <summary>
    /// Employee
    /// </summary>

    [TableName("Employee")]



    [PrimaryKey("UId", AutoIncrement = false)]

    [ExplicitColumns]
    public partial class Employee : Db.Record<Employee>, IModel
    {

        /// <summary>
        /// UId
        /// </summary>

        [Column]
        public long UId
        {
            get
            {
                return _UId;
            }
            set
            {
                _UId = value;
                MarkColumnModified("UId");
            }
        }
        long _UId;



        /// <summary>
        /// Name
        /// </summary>

        [Column]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                MarkColumnModified("Name");
            }
        }
        string _Name;



        /// <summary>
        /// Nickname
        /// </summary>

        [Column]
        public string Nickname
        {
            get
            {
                return _Nickname;
            }
            set
            {
                _Nickname = value;
                MarkColumnModified("Nickname");
            }
        }
        string _Nickname;



        /// <summary>
        /// WxId
        /// </summary>

        [Column]
        public string WxId
        {
            get
            {
                return _WxId;
            }
            set
            {
                _WxId = value;
                MarkColumnModified("WxId");
            }
        }
        string _WxId;



        /// <summary>
        /// ModifyTime
        /// </summary>

        [Column]
        public DateTime ModifyTime
        {
            get
            {
                return _ModifyTime;
            }
            set
            {
                _ModifyTime = value;
                MarkColumnModified("ModifyTime");
            }
        }
        DateTime _ModifyTime;



        /// <summary>
        /// Enable
        /// </summary>

        [Column]
        public bool Enable
        {
            get
            {
                return _Enable;
            }
            set
            {
                _Enable = value;
                MarkColumnModified("Enable");
            }
        }
        bool _Enable;




    }

    /// <summary>
    /// EmployeeStatistics
    /// </summary>

    [TableName("EmployeeStatistics")]



    [PrimaryKey("Id")]


    [ExplicitColumns]
    public partial class EmployeeStatistic : Db.Record<EmployeeStatistic>, IModel
    {

        /// <summary>
        /// Id
        /// </summary>

        [Column]
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                MarkColumnModified("Id");
            }
        }
        int _Id;



        /// <summary>
        /// BId
        /// </summary>

        [Column]
        public long BId
        {
            get
            {
                return _BId;
            }
            set
            {
                _BId = value;
                MarkColumnModified("BId");
            }
        }
        long _BId;



        /// <summary>
        /// UId
        /// </summary>

        [Column]
        public long UId
        {
            get
            {
                return _UId;
            }
            set
            {
                _UId = value;
                MarkColumnModified("UId");
            }
        }
        long _UId;



        /// <summary>
        /// Date
        /// </summary>

        [Column]
        public DateTime Date
        {
            get
            {
                return _Date;
            }
            set
            {
                _Date = value;
                MarkColumnModified("Date");
            }
        }
        DateTime _Date;



        /// <summary>
        /// MessageRecv
        /// </summary>

        [Column]
        public int MessageRecv
        {
            get
            {
                return _MessageRecv;
            }
            set
            {
                _MessageRecv = value;
                MarkColumnModified("MessageRecv");
            }
        }
        int _MessageRecv;



        /// <summary>
        /// MessageSend
        /// </summary>

        [Column]
        public int MessageSend
        {
            get
            {
                return _MessageSend;
            }
            set
            {
                _MessageSend = value;
                MarkColumnModified("MessageSend");
            }
        }
        int _MessageSend;



        /// <summary>
        /// SessionCount
        /// </summary>

        [Column]
        public int SessionCount
        {
            get
            {
                return _SessionCount;
            }
            set
            {
                _SessionCount = value;
                MarkColumnModified("SessionCount");
            }
        }
        int _SessionCount;



        /// <summary>
        /// OnlineTime
        /// </summary>

        [Column]
        public int OnlineTime
        {
            get
            {
                return _OnlineTime;
            }
            set
            {
                _OnlineTime = value;
                MarkColumnModified("OnlineTime");
            }
        }
        int _OnlineTime;



        /// <summary>
        /// Request
        /// </summary>

        [Column]
        public int Request
        {
            get
            {
                return _Request;
            }
            set
            {
                _Request = value;
                MarkColumnModified("Request");
            }
        }
        int _Request;



        /// <summary>
        /// Created
        /// </summary>

        [Column]
        public int Created
        {
            get
            {
                return _Created;
            }
            set
            {
                _Created = value;
                MarkColumnModified("Created");
            }
        }
        int _Created;



        /// <summary>
        /// Completed
        /// </summary>

        [Column]
        public int Completed
        {
            get
            {
                return _Completed;
            }
            set
            {
                _Completed = value;
                MarkColumnModified("Completed");
            }
        }
        int _Completed;




    }

    /// <summary>
    /// Message
    /// </summary>

    [TableName("Message")]



    [PrimaryKey("Id")]


    [ExplicitColumns]
    public partial class Message : Db.Record<Message>, IModel
    {

        /// <summary>
        /// Id
        /// </summary>

        [Column]
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                MarkColumnModified("Id");
            }
        }
        int _Id;



        /// <summary>
        /// MessageId
        /// </summary>

        [Column]
        public int MessageId
        {
            get
            {
                return _MessageId;
            }
            set
            {
                _MessageId = value;
                MarkColumnModified("MessageId");
            }
        }
        int _MessageId;



        /// <summary>
        /// BizUId
        /// </summary>

        [Column]
        public long BizUId
        {
            get
            {
                return _BizUId;
            }
            set
            {
                _BizUId = value;
                MarkColumnModified("BizUId");
            }
        }
        long _BizUId;



        /// <summary>
        /// UserUId
        /// </summary>

        [Column]
        public long UserUId
        {
            get
            {
                return _UserUId;
            }
            set
            {
                _UserUId = value;
                MarkColumnModified("UserUId");
            }
        }
        long _UserUId;



        /// <summary>
        /// EmployeeUId
        /// </summary>
        [Column]
        public long EmployeeUId
        {
            get
            {
                return _EmployeeUId;
            }
            set
            {
                _EmployeeUId = value;
                MarkColumnModified("EmployeeUId");
            }
        }
        long _EmployeeUId;



        /// <summary>
        /// CreateTime
        /// </summary>
        [Column]
        public DateTime CreateTime
        {
            get
            {
                return _CreateTime;
            }
            set
            {
                _CreateTime = value;
                MarkColumnModified("CreateTime");
            }
        }
        DateTime _CreateTime;



        /// <summary>
        /// Content
        /// </summary>
        [Column]
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                _Content = value;
                MarkColumnModified("Content");
            }
        }
        string _Content;



        /// <summary>
        /// MessageType
        /// </summary>

        [Column]
        public int MessageType
        {
            get
            {
                return _MessageType;
            }
            set
            {
                _MessageType = value;
                MarkColumnModified("MessageType");
            }
        }
        int _MessageType;



        /// <summary>
        /// Type
        /// </summary>
        [Column]
        public int Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
                MarkColumnModified("Type");
            }
        }
        int _Type;
    }

    /// <summary>
    /// Setting
    /// </summary>

    [TableName("Setting")]



    [PrimaryKey("Id")]


    [ExplicitColumns]
    public partial class Setting : Db.Record<Setting>, IModel
    {

        /// <summary>
        /// Id
        /// </summary>

        [Column]
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                MarkColumnModified("Id");
            }
        }
        int _Id;



        /// <summary>
        /// Key
        /// </summary>
        [Column]
        public string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                _Key = value;
                MarkColumnModified("Key");
            }
        }
        string _Key;



        /// <summary>
        /// Value
        /// </summary>
        [Column]
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                MarkColumnModified("Value");
            }
        }
        string _Value;
    }

    /// <summary>
    /// User
    /// </summary>

    [TableName("User")]
    [PrimaryKey("UId", AutoIncrement = false)]
    [ExplicitColumns]
    public partial class User : Db.Record<User>, IModel
    {

        /// <summary>
        /// UId
        /// </summary>
        [Column]
        public long UId
        {
            get
            {
                return _UId;
            }
            set
            {
                _UId = value;
                MarkColumnModified("UId");
            }
        }
        long _UId;



        /// <summary>
        /// Name
        /// </summary>

        [Column]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                MarkColumnModified("Name");
            }
        }
        string _Name;



        /// <summary>
        /// Nickname
        /// </summary>

        [Column]
        public string Nickname
        {
            get
            {
                return _Nickname;
            }
            set
            {
                _Nickname = value;
                MarkColumnModified("Nickname");
            }
        }
        string _Nickname;



        /// <summary>
        /// WxId
        /// </summary>

        [Column]
        public string WxId
        {
            get
            {
                return _WxId;
            }
            set
            {
                _WxId = value;
                MarkColumnModified("WxId");
            }
        }
        string _WxId;



        /// <summary>
        /// ModifyTime
        /// </summary>

        [Column]
        public DateTime ModifyTime
        {
            get
            {
                return _ModifyTime;
            }
            set
            {
                _ModifyTime = value;
                MarkColumnModified("ModifyTime");
            }
        }
        DateTime _ModifyTime;
    }



    /// <summary>
    /// Sessions
    /// </summary>

    [TableName("Sessions")]
    [PrimaryKey("Id")]
    [ExplicitColumns]
    public partial class UserSession : Db.Record<UserSession>, IModel
    {

        /// <summary>
        /// Id
        /// </summary>
        [Column]
        public long Id
        {
            get
            {
                return _UId;
            }
            set
            {
                _UId = value;
                MarkColumnModified("Id");
            }
        }
        long _UId;

        [Column]
        public long Bizuin {
            get
            {
                return _Bizuin;
            }
            set
            {
                _Bizuin = value;
                MarkColumnModified("Bizuin");
            }
        }
        long _Bizuin;

        /// <summary>
        /// Useruin
        /// </summary>

        [Column]
        public long Useruin
        {
            get
            {
                return _Useruin;
            }
            set
            {
                _Useruin = value;
                MarkColumnModified("Useruin");
            }
        }
        long _Useruin;



        /// <summary>
        /// Time
        /// </summary>

        [Column]
        public DateTime Time
        {
            get
            {
                return _Time;
            }
            set
            {
                _Time = value;
                MarkColumnModified("Time");
            }
        }
        DateTime _Time;



        /// <summary>
        /// Delay
        /// </summary>

        [Column]
        public int Delay
        {
            get
            {
                return _Delay;
            }
            set
            {
                _Delay = value;
                MarkColumnModified("Delay");
            }
        }
        int _Delay;



        /// <summary>
        /// Completed
        /// </summary>

        [Column]
        public bool Completed
        {
            get
            {
                return _Completed;
            }
            set
            {
                _Completed = value;
                MarkColumnModified("Completed");
            }
        }
        bool _Completed;
    }
}