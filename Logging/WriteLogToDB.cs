
using System.Collections;
using System.IO;
using System.Data.SqlClient;

using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace HNAS.Framework4.Logging
{
    /// <summary>
    /// 日志写入数据库
    /// </summary>
    /// 创 建 人：胡鹏
    /// 创建日期：2011-12-5
    /// 修 改 人：
    /// 修改日期：
    public static class WriteLogToDB
    {

        /// <summary>
        /// 写日志函数,将日志信息记录到数据库中
        /// </summary>
        /// <param name="strMessage">日志信息</param>
        /// <param name="strOperatorName">操作人</param>
        /// <param name="bOpen">日志开关</param>
        /// 创 建 人：胡鹏
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public static void WriteLog(string strMessage, string strOperatorName = "", bool bOpen = true)
        {
            if (bOpen)
            {
                LogEntry logEntry = new LogEntry();
                logEntry.Title = "日志";
                logEntry.Message = strMessage + "@" + strOperatorName;
                logEntry.Categories.Add("记录");

                Logger.Write(logEntry, "General", 3);
                //Logger.Write(logEntry);
            }
        }

        /// <summary>
        /// 创建日志信息表到数据库中
        /// </summary>
        /// <param name="constr">数据库服务器</param>
        /// 创 建 人：胡鹏
        /// 创建日期：2012-11-28
        /// 修 改 人：
        /// 修改日期：
        public static void CreateLogDB(string constr)
        {
            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            ArrayList Lists = ExecuteSqlFile("WebUI/Scripts/LoggingDatabase.sql"); //调用ExecuteSqlFile()方法，反回 ArrayList对象;
            string teststr;                           //定义遍历ArrayList 的变量;
            foreach (string varcommandText in Lists)
            {
                teststr = varcommandText;             //遍历并符值;
                cmd.CommandText = teststr;            //为SqlCommand赋Sql语句;
                cmd.ExecuteNonQuery();                //执行
            }
            conn.Close();
        }

        /// <summary>
        /// 读取.sql脚本文件
        /// </summary>
        /// <param name="varFileName">文件路径</param>
        /// 创 建 人：胡鹏
        /// 创建日期：2012-11-28
        /// 修 改 人：
        /// 修改日期：
        public static ArrayList ExecuteSqlFile(string varFileName)
        {
            StreamReader sr = File.OpenText(varFileName);//传入的是文件路径及完整的文件名
            ArrayList alSql = new ArrayList();           //每读取一条语名存入ArrayList
            string commandText = "";
            string varLine = "";
            while (sr.Peek() > -1)
            {
                varLine = sr.ReadLine();
                if (varLine == "")
                {
                    continue;
                }
                if (varLine != "GO")
                {
                    commandText += varLine;
                    commandText += " ";
                }
                else
                {
                    alSql.Add(commandText);
                    commandText = "";
                }
            }

            sr.Close();
            return alSql;
        }

    }
}
