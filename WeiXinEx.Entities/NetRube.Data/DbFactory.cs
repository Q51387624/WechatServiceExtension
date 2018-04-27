using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;

namespace NetRube.Data
{
    /// <summary>
    /// 数据上下文工厂
    /// </summary>
    public class DbFactory : IDisposable
    {
        private static string connectionString;
        private static ThreadLocal<Database> dataContext = new ThreadLocal<Database>();

        public static void SetConnectionString(string connection)
        {
            connectionString = connection;

        }
        /// <summary>
        /// 默认连接数据上下文（连接字符串：mysql）
        /// </summary>
        public static Database Default
        {
            get
            {
                if (!dataContext.IsValueCreated)
                {
                    dataContext.Value = new Database(connectionString);
                }
                return dataContext.Value;
            }
        }

     
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            dataContext.Value = null;
            dataContext.Dispose();
        }
    }
}
