using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using NetRube.Data;
using WeiXinEx.Entities;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace WeiXinEx.TestConsole
{
    public class Column
    {
        public string ColumnName;
        public string PropertyName;
        public string PropertyType;
        public string Remark;
        public bool IsPK;
        public bool IsNullable;
        public bool IsAutoIncrement;
        public bool Ignore;
    }
    public class Table
    {
        
        public string TableName;
        

        public string Schema;
        public bool IsView;
        public string CleanName;
        public string ClassName;
        public string SequenceName;
        public bool Ignore;

        public List<Column> Columns;
        public Column Primkey
        {
            get
            {
                return Columns.SingleOrDefault(x => x.IsPK);
            }
        }

        public Column GetColumn(string columnName)
        {
            return Columns.Single(x => string.Compare(x.ColumnName, columnName, true) == 0);
        }

        public Column this[string columnName]
        {
            get
            {
                return GetColumn(columnName);
            }
        }

    }
    public class T4Builder
    {
        public string Schema { get; set; }
        public List<Table> Tables { get; set; }
        private string connectionString;
        
        private const string Schema_SQL = "select database()";
        private const string Table_SQL = "select TABLE_NAME as Name from information_schema.TABLES WHERE TABLE_SCHEMA = '{0}'";
        private const string Column_SQL = "select COLUMN_NAME as Name,COLUMN_COMMENT as Remark,Data_Type as DataType from information_schema.COLUMNS WHERE TABLE_SCHEMA='{0}' and TABLE_NAME = '{1}'";
        public T4Builder(string connectionString)
        {
            this.connectionString = connectionString;
            InitMySQL();
        }

        private void InitMySQL() {
            //数据库名
            Schema = Scalar(Schema_SQL).ToString();
            //表名
            Tables = Query(string.Format(Table_SQL, Schema), (reader) =>
            {
                return new Table
                {
                    TableName = reader["Name"].ToString(),
                };
            });

            foreach (var table in Tables)
            {
                table.Columns = Query(string.Format(Column_SQL, Schema, table.TableName), (reader) =>
                {
                    return new Column
                    {
                        ColumnName = reader["Name"].ToString(),
                        Remark = reader["Remark"].ToString(),
                        PropertyType = GetPropertyType(reader["DataType"].ToString())
                    };
                });
            }
        }

        private List<T> Query<T>(string sql, Func<MySqlDataReader, T> func)
        {
            List<T> result = new List<T>();
            using (var reader = MySqlHelper.ExecuteReader(connectionString, sql))
                while (reader.Read())
                    result.Add(func(reader));
            return result;
        }

        private object Scalar(string sql)
        {
            return MySqlHelper.ExecuteScalar(connectionString, sql);
        }

        private string GetPropertyType(string dataType)
        {
            switch (dataType)
            {
                case "bigint":
                    return "long";
                case "int":
                    return "int";
                case "smallint":
                    return "short";
                case "guid":
                    return "Guid";
                case "smalldatetime":
                case "date":
                case "datetime":
                case "timestamp":
                    return "DateTime";
                case "time":
                    return "TimeSpan";
                case "float":
                    return "float";
                case "double":
                    return "double";
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                    return "decimal";
                case "bit":
                case "bool":
                case "boolean":
                    return "bool";
                case "tinyint":
                    return "byte";
                case "image":
                case "binary":
                case "blob":
                case "mediumblob":
                case "longblob":
                case "varbinary":
                    return "byte[]";
            }
            return "string";
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var token = 1639493424;
            var index = string.Format("https://mpkf.weixin.qq.com/cgi-bin/kfindex?token={0}", token);
            var html = NetHelper.Get(index);
            var scripts = NetHelper.GetRegexMatches(html, @"<script[^>]*>(.|\n)*?(?=<\/script>)<\/script>");
            var script = scripts.FirstOrDefault(p => p.IndexOf("MCS.cgiData") > 0);
            var start = script.IndexOf("MCS.cgiData");
            var end = script.LastIndexOf("seajs.use");
            script = script.Substring(start, end - start);
            Console.ReadLine();
        }

        
    }
    public class NetHelper
    {
        public static string Get(string url)
        {
            var request = WebRequest.Create("https://mpkf.weixin.qq.com/cgi-bin/kfindex?token=1639493424");
            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            reader.Close();
            return text;
        }

        public static List<string> GetRegexMatches(string html, string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matches = regex.Matches(html);
            return matches.Select(p => p.Value).ToList();


        }
    }
}
