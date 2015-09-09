using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HNAS.Framework4.WebCommon
{
    /// <summary>
    /// 工具类
    /// </summary>
    /// Copyright (c) 2012 海航航空信息系统有限公司
    /// 创 建 人：王宇
    /// 创建日期：2012-2-20
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    public static class Function
    {
        /// <summary>
        /// 绑定数据到DropDownList
        /// </summary>
        /// <param name="ddl">DropDownList实例</param>
        /// <param name="dt">数据源</param>
        /// <param name="strTextField">条目</param>
        /// <param name="strValueField">值</param>
        public static void BindDataToDDL(DropDownList ddl, DataTable dt, string strTextField, string strValueField)
        {
            if (dt != null)
            {
                ddl.DataSource = dt;
                ddl.DataTextField = strTextField;
                ddl.DataValueField = strValueField;
                ddl.DataBind();
            }
        }

        #region 导出到Excel
        /// <summary>
        /// GridView导出到Excel
        /// </summary>
        /// <param name="gvBindData"></param>
        /// <param name="strExcelName">文件名</param>
        /// <param name="strCharset">编码类型</param>
        /// <param name="page">默认当前页面</param>
        public static void GridViewToExcel(GridView gvBindData, string strExcelName = "GridView", string strCharset = "GB2312", Page page = null)
        {
            gvBindData.AllowPaging = false;
            gvBindData.DataBind();
            for (int i = 0; i < gvBindData.Columns.Count; i++)
            {
                if (gvBindData.Columns[i].FooterText.IndexOf("Delete") >= 0)
                {
                    gvBindData.Columns[i].Visible = false;
                }
            }
            if (page == null)
            {
                page = ((Page)HttpContext.Current.Handler);
            }

            page.Response.Clear();
            page.Response.Buffer = false;
            page.Response.Charset = strCharset;
            page.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(strExcelName, Encoding.UTF8) + ".xls");
            page.Response.ContentEncoding = Encoding.GetEncoding(strCharset);
            page.Response.ContentType = "application/ms-excel";
            StringWriter oStringWriter = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(oStringWriter);
            gvBindData.RenderControl(writer);
            page.Response.Write(oStringWriter.ToString());
            page.Response.End();
            gvBindData.AllowPaging = true;
        }

        /// <summary>
        /// DataTable导出Excel
        /// </summary>
        /// <param name="dtSource">数据源</param>
        /// <param name="strColumnNames">列名</param>
        /// <param name="strExcelName">文件名</param>
        /// <param name="strCharset">编码类型</param>
        /// <param name="page">默认当前页</param>
        public static void DTToExcel(DataTable dtSource, string strColumnNames, string strExcelName = "GridView", string strCharset = "GB2312", Page page = null)
        {
            DataRow[] rowArray = dtSource.Select();
            StringWriter writer = new StringWriter();
            writer.WriteLine(strColumnNames);
            foreach (DataRow row in rowArray)
            {
                string str = "";
                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    if (i != (dtSource.Columns.Count - 1))
                    {
                        str = str + row[i] + "\t";
                    }
                    else
                    {
                        str = str + row[i];
                    }
                }
                writer.WriteLine(str);
            }
            writer.Close();
            if (page == null)
            {
                page = ((Page)HttpContext.Current.Handler);
            }

            page.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(strExcelName, Encoding.UTF8) + ".xls");
            page.Response.ContentType = "application/ms-excel";
            page.Response.ContentEncoding = Encoding.GetEncoding(strCharset);
            page.Response.Write(writer);
            page.Response.End();
        }
        #endregion

        /// <summary>
        /// 密码MD5加密
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static string EncryptMD5(string strValue)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(strValue, "MD5");
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string userHostAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            switch (userHostAddress)
            {
                case null:
                case "":
                    userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    break;
            }
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContext.Current.Request.UserHostAddress;
            }
            return userHostAddress;
        }

        #region 随机字符串
        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GetRandom()
        {
            string str = DateTime.Now.ToString("yyyyMMddhhmmss");
            int num = new Random().Next(0, 0xf423f);
            return (str + num.ToString("000000"));
        }

        /// <summary>
        /// 获取指定长度的随机字符串
        /// </summary>
        /// <param name="iLength"></param>
        /// <returns></returns>
        public static string GetRandomString(int iLength)
        {
            string[] strArray = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(new char[] { ',' });
            string str2 = "";
            int num = -1;
            Random random = new Random();
            for (int i = 1; i < (iLength + 1); i++)
            {
                if (num != -1)
                {
                    random = new Random((i * num) * ((int)DateTime.Now.Ticks));
                }
                int index = random.Next(0x3d);
                if ((num != -1) && (num == index))
                {
                    return GetRandomString(iLength);
                }
                num = index;
                str2 = str2 + strArray[index];
            }
            return str2;
        }
        #endregion

        #region 是否有中文字符
        /// <summary>
        /// 是否有中文字符
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static bool IsContainChinese(string strText)
        {
            foreach (char ch in strText)
            {
                int num = ch;
                if ((num >= 0x4e00) && (num <= 0x9fa5))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 发送邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailto">收件人</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="mailcc">抄送</param>
        /// <param name="strAttachFilePath">附件路径</param>
        /// <returns></returns>
        public static bool SendEMail(string mailto, string subject, string body, string mailcc, string strAttachFilePath)
        {
            try
            {
                Encoding encoding = Encoding.GetEncoding(0x3a8);
                string str = ConfigurationManager.AppSettings["MailHost"].Trim();
                string str2 = ConfigurationManager.AppSettings["MailIsNeedCheck"].Trim();
                string userName = ConfigurationManager.AppSettings["MailUserName"].Trim();
                string password = ConfigurationManager.AppSettings["MailPwd"].Trim();
                string address = ConfigurationManager.AppSettings["MailFrom"].Trim();
                string displayName = ConfigurationManager.AppSettings["MailFromCName"].Trim();
                SmtpClient client = new SmtpClient
                {
                    Host = str,
                    UseDefaultCredentials = false
                };
                if (str2.Equals("T"))
                {
                    client.Credentials = new NetworkCredential(userName, password);
                }
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                MailMessage message = new MailMessage
                {
                    From = new MailAddress(address, displayName)
                };
                if (!string.IsNullOrEmpty(mailto) && (mailto.Trim() != ""))
                {
                    string[] strArray = mailto.Split(";".ToCharArray());
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i].Trim() != "")
                        {
                            message.To.Add(strArray[i]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(mailcc) && (mailcc.Trim() != ""))
                {
                    string[] strArray2 = mailcc.Split(";".ToCharArray());
                    for (int j = 0; j < strArray2.Length; j++)
                    {
                        if (strArray2[j].Trim() != "")
                        {
                            message.CC.Add(strArray2[j].Trim());
                        }
                    }
                }
                message.Subject = subject;
                message.SubjectEncoding = encoding;
                message.Body = body;
                message.BodyEncoding = encoding;
                message.IsBodyHtml = true;
                if (strAttachFilePath.Trim().Length > 0)
                {
                    string[] strArray3 = strAttachFilePath.Split(";".ToCharArray());
                    for (int k = 0; k < strArray3.Length; k++)
                    {
                        if (strArray3[k].Trim() != "")
                        {
                            message.Attachments.Add(new Attachment(strArray3[k]));
                        }
                    }
                }
                client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region DataTable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="strColumnsName"></param>
        /// <param name="bIsDistinct"></param>
        /// <returns></returns>
        public static DataTable DistinctDataTable(DataTable dtSource, string[] strColumnsName, bool bIsDistinct)
        {
            return dtSource.DefaultView.ToTable(bIsDistinct, strColumnsName);
        }

        /// <summary>
        /// 合并DataTable
        /// </summary>
        /// <param name="dtAll"></param>
        /// <param name="dtValue"></param>
        /// <returns></returns>
        public static DataTable TableIntersection(DataTable dtAll, DataTable dtValue)
        {
            DataTable table = null;
            if ((((dtAll == null) || (dtAll.Rows.Count <= 0)) || (dtValue == null)) || (dtValue.Rows.Count <= 0))
            {
                return null;
            }
            table = dtValue.Clone();
            foreach (DataRow row in dtValue.Rows)
            {
                string key = row[0].ToString();
                if (dtAll.Rows.Find(key) != null)
                {
                    table.ImportRow(row);
                }
            }
            return table;
        }
        #endregion

        #region 返回星期几
        /// <summary>
        /// 返回星期几
        /// </summary>
        /// <param name="dtDate"></param>
        /// <returns></returns>
        public static string ToWeekDay(DateTime dtDate)
        {
            string str = "";
            switch (int.Parse(dtDate.DayOfWeek.ToString("d")))
            {
                case 0:
                    str = "日";
                    break;

                case 1:
                    str = "一";
                    break;

                case 2:
                    str = "二";
                    break;

                case 3:
                    str = "三";
                    break;

                case 4:
                    str = "四";
                    break;

                case 5:
                    str = "五";
                    break;

                case 6:
                    str = "六";
                    break;
            }
            return ("星期" + str);
        }

        /// <summary>
        /// 返回星期几
        /// </summary>
        /// <param name="iNum"></param>
        /// <param name="bInternational"></param>
        /// <returns></returns>
        public static string ToWeekDay(int iNum, bool bInternational)
        {
            string str = "";
            if (iNum > 0)
            {
                if (bInternational)
                {
                    iNum += 6;
                }
                iNum = iNum % 7;
                switch (iNum)
                {
                    case 0:
                        str = "日";
                        break;

                    case 1:
                        str = "一";
                        break;

                    case 2:
                        str = "二";
                        break;

                    case 3:
                        str = "三";
                        break;

                    case 4:
                        str = "四";
                        break;

                    case 5:
                        str = "五";
                        break;

                    case 6:
                        str = "六";
                        break;
                }
            }
            return ("星期" + str);
        }

        #endregion
    }

}
