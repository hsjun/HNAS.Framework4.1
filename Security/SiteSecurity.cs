using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;
using System.Configuration;
using System.Text;
using System;

namespace HNAS.Framework4.Security
{
    /// <summary>
    /// 网站安全类：防止SQL注入、获取客户端信息
    /// </summary>
    ///  创 建 人：王宇
    ///  创建日期：2011年11月25日
    ///  修 改 人：
    ///  修改日期：
    ///  Copyright (c) 2011 海南海航航空信息系统有限公司
    ///  版 本：1.0	
    public static class SiteSecurity
    {

        #region 防SQL注入相关方法

        #region 相关参数
        /// <summary>
        /// 防SQL注入提示
        /// </summary>
        public static readonly string SQLInjectionMessage = "提示：您的输入中包含违规字符，请修改！\\n请检查";

        /// <summary>
        /// 特殊字符定义
        /// </summary>
        //public static string strSQLChars = Properties.Resources.IllegalChars;
        public static string strSQLChars = @"insert，create，select，delete，script，|，;，$，%，'，\'，&lt;，&gt;，CR，LF，\，/*，*/";
        /// <summary>
        /// 关键字定义
        /// </summary>
        //public static string strSQLKeys = Properties.Resources.IllegalSQLKeys;
        public static string strSQLKeys = @"update，drop，and，exec，count，chr，mid，master，or，truncate，char，declare，join";

        /// <summary>
        /// 特殊字符字符串数组
        /// </summary>
        public static string[] SQLChars = strSQLChars.Split('，');
        /// <summary>
        /// 关键字字符串数组
        /// </summary>
        public static string[] SQLKeys = strSQLKeys.Split('，');
        #endregion

        #region 获取过滤的关键字
        /// <summary>
        /// 获取过滤的关键字
        /// </summary>
        /// <param name="bIncludeKeys">是否返回SQL关键字如select,insert等，默认返回</param>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public static string GetIllegalKeys(bool bIncludeKeys = true)
        {
            if (bIncludeKeys)
            {
                return strSQLChars + "，" + strSQLKeys;
            }
            return strSQLChars;
        }
        #endregion

        #region 判断string是否包含关键字
        /// <summary>
        ///  判断string是否包含关键字
        /// </summary> 
        /// <param name="strValue">关键字</param>
        /// <param name="strChar">含有的敏感字符</param>
        /// <param name="bCheckKeys">是否检查SQL关键字如select,insert等，默认检查</param>
        /// <param name="strIllegalChars">待检查特殊字符的配置文件节点名称，关键字直接用'，'（全角逗号）隔开，为空则检查框架默认特殊字符</param>
        /// <param name="strIllegalSQLKeys">待检查SQL关键字的配置文件节点名称，关键字直接用'，'（全角逗号）隔开，为空则检查框架默认关键字</param>
        /// <returns>true=包含;false=不包含</returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public static bool KeyValueCheck(string strValue, ref string strChar, bool bCheckKeys = true, string strIllegalChars = "", string strIllegalSQLKeys = "")
        {
            //是否有自定义特殊字符
            if (!string.IsNullOrEmpty(strIllegalChars))
            {
                SQLChars = ConfigurationManager.AppSettings[strIllegalChars].Split('，');
            }
            //检查特殊字符
            foreach (string strkey in SQLChars)
            {
                //包含关键字
                if (strValue.ToLower().IndexOf(strkey) > -1)
                {
                    strChar = strkey;
                    return true;
                }
            }
            //检查SQL关键字
            if (bCheckKeys)
            {
                //是否自定义关键字
                if (!string.IsNullOrEmpty(strIllegalSQLKeys))
                {
                    SQLKeys = ConfigurationManager.AppSettings[strIllegalSQLKeys].Split('，');
                }
                foreach (string strkey in SQLKeys)
                {
                    //包含关键字
                    if (strValue.ToLower().IndexOf(strkey) > -1)
                    {
                        if ((strValue.ToLower().IndexOf(strkey + " ") > -1) || (strValue.ToLower().IndexOf(" " + strkey) > -1))
                        {
                            strChar = strkey;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion


        #region 判断Request是否包含关键字
        /// <summary>
        /// 判断Request是否包含关键字，有则提示
        /// </summary>
        /// <param name="httpApplication">httpApplication</param>
        /// <param name="bCheckKeys">是否检查SQL关键字如select,insert等，默认检查</param>
        /// <param name="strIllegalChars">待检查特殊字符的配置文件节点名称，关键字直接用'，'（全角逗号）隔开，为空则检查框架默认特殊字符</param>
        /// <param name="strIllegalSQLKeys">待检查SQL关键字的配置文件节点名称，关键字直接用'，'（全角逗号）隔开，为空则检查框架默认关键字</param>
        /// <param name="strErrorUrl">错误提示地址（还有问题）</param>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public static void CheckSQLInjection(HttpApplication httpApplication, bool bCheckKeys = true, string strIllegalChars = "", string strIllegalSQLKeys = "", string strErrorUrl = null)
        {
            HttpRequest httpRequest = httpApplication.Request;

            //遍历Post参数，隐藏域除外
            foreach (string param in httpRequest.Form)
            {
                string strChar = null;
                //if (httpRequest.Form[param].Trim().Length > 15)
                //{
                //过滤ASP.Net的隐藏对象
                if (!(param == "__EVENTTARGET" || param == "__EVENTVALIDATION" || param == "__EVENTARGUMENT" || param == "__VIEWSTATE")
                    && KeyValueCheck(httpRequest.Form[param], ref strChar, bCheckKeys, strIllegalChars, strIllegalSQLKeys))
                {
                    string errorMsg = SQLInjectionMessage + "：\"" + HttpUtility.UrlEncode(httpRequest.Form[param]).Replace("+", "%20") + "\"含有违规字符 " + HttpUtility.UrlEncode(strChar);
                    httpApplication.Response.Write("<script type='text/javascript'>alert(decodeURIComponent('"
    + errorMsg + "'));window.location.href=window.location.href;</script>");

                    //if (string.IsNullOrEmpty(strErrorUrl))
                    //{
                    //    httpApplication.Response.Write("<script type='text/javascript'>alert(decodeURIComponent('"
                    //        + errorMsg + "'));window.location.href=window.location.href;</script>");
                    //}
                    //else
                    //{
                    //    httpApplication.Response.Write("<script type='text/javascript'>window.open('" + strErrorUrl + "?m=" + HttpUtility.UrlEncode(errorMsg) + "');</script>");
                    //}

                    httpApplication.Response.End();
                    return;
                }
                //}
            }

            //遍历Get参数。
            foreach (string param in httpRequest.QueryString)
            {
                string strChar = null;
                //if (httpRequest.QueryString[param].Trim().Length > 15)
                //{
                //输出

                if (KeyValueCheck(httpRequest.QueryString[param], ref strChar, bCheckKeys, strIllegalChars, strIllegalSQLKeys))
                {
                    string errorMsg = SQLInjectionMessage + "：\"" + HttpUtility.UrlEncode(httpRequest.QueryString[param]).Replace("+", "%20") + "\"含有违规字符 " + HttpUtility.UrlEncode(strChar);
                    httpApplication.Response.Write("<script type='text/javascript'>alert(decodeURIComponent('"
    + errorMsg + "'));window.location.href=window.location.href;</script>");
                    httpApplication.Response.End();
                    return;
                }
                //}
            }
        }
        #endregion

        #region 判断Request是否包含关键字
        /// <summary>
        /// 判断Request是否包含关键字，有则返回含过滤关键字的变量值，无则返回null
        /// </summary>
        /// <param name="httpApplication">httpApplication</param>
        /// <param name="bCheckKeys">是否检查SQL关键字如select,insert等，默认检查</param>
        /// <param name="strIllegalChars">待检查特殊字符的配置文件节点名称，关键字直接用'，'（全角逗号）隔开，为空则检查框架默认特殊字符</param>
        /// <param name="strIllegalSQLKeys">待检查SQL关键字的配置文件节点名称，关键字直接用'，'（全角逗号）隔开，为空则检查框架默认关键字</param>
        /// <returns>返回含过滤关键字的变量值</returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public static string CheckSQLInjectionReturn(HttpApplication httpApplication, bool bCheckKeys = true, string strIllegalChars = "", string strIllegalSQLKeys = "")
        {
            HttpRequest httpRequest = httpApplication.Request;

            //遍历Post参数，隐藏域除外
            foreach (string param in httpRequest.Form)
            {
                string strChar = null;
                //if (httpRequest.Form[param].Trim().Length > 15)
                //{
                //过滤ASP.Net的隐藏对象
                if (!(param == "__EVENTTARGET" || param == "__EVENTVALIDATION" || param == "__EVENTARGUMENT" || param == "__VIEWSTATE")
                    && KeyValueCheck(httpRequest.Form[param], ref strChar, bCheckKeys, strIllegalChars, strIllegalSQLKeys))
                {
                    return httpRequest.Form[param] + "^" + strChar;
                }
                //}
            }

            //遍历Get参数。
            foreach (string param in httpRequest.QueryString)
            {
                string strChar = null;
                //if (httpRequest.QueryString[param].Trim().Length > 15)
                //{
                //输出
                if (KeyValueCheck(httpRequest.QueryString[param], ref strChar, bCheckKeys, strIllegalChars, strIllegalSQLKeys))
                {
                    return httpRequest.QueryString[param] + "^" + strChar;
                }
                //}
            }
            return null;
        }
        #endregion

        #region 判断URL是否包含关键字
        /// <summary>
        ///  判断URL是否包含关键字
        /// </summary> 
        /// <param name="bCheckKeys">是否检查SQL关键字如select,insert等，默认检查</param>
        /// <param name="strIllegalChars">待检查特殊字符的配置文件节点名称，关键字直接用'，'（全角逗号）隔开，为空则检查框架默认特殊字符</param>
        /// <param name="strIllegalSQLKeys">待检查SQL关键字的配置文件节点名称，关键字直接用'，'（全角逗号）隔开，为空则检查框架默认关键字</param>
        /// <returns>true=包含;false=不包含</returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public static bool CheckUrlHack(bool bCheckKeys = true, string strIllegalChars = "", string strIllegalSQLKeys = "")
        {
            //遍历Get参数。
            foreach (string param in HttpContext.Current.Request.QueryString)
            {
                if (HttpContext.Current.Request.QueryString[param].Trim().Length > 15)
                {
                    string strChar = null;
                    //输出
                    if (KeyValueCheck(HttpContext.Current.Request.QueryString[param], ref strChar, bCheckKeys, strIllegalChars, strIllegalSQLKeys))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region 替换字符串
        /// <summary>
        /// 替换字符串中特殊字符
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public static string GetSafeString(string strSource)
        {
            if (strSource == null)
            {
                return "";
            }
            // |，&，;，$，%，@，'，"，\'，\"，<>，()，+，CR，LF，,，\，/*，*/
            //替换特殊字符
            strSource = strSource.Replace("|", "");
            strSource = strSource.Replace("&", "");
            strSource = strSource.Replace(";", "，");
            strSource = strSource.Replace("$", "");
            strSource = strSource.Replace("%", "");
            strSource = strSource.Replace("@", "at");
            strSource = strSource.Replace("'", "‘");
            strSource = strSource.Replace("\"", "”");
            strSource = strSource.Replace("\\", "/");
            strSource = strSource.Replace("<>", "[]");
            strSource = strSource.Replace("+", "plus");
            strSource = strSource.Replace("CR", "Enter");
            strSource = strSource.Replace("LF", "Line Feed");
            strSource = strSource.Replace(",", "，");
            strSource = strSource.Replace("/*", "");
            strSource = strSource.Replace("*/", "");

            //strSource = HttpContext.Current.Server.HtmlEncode(strSource).Trim();
            return strSource;
        }
        #endregion

        #endregion

        #region 获取客户端相关信息

        #region 获取客户端IP
        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public static string GetClientIP()
        {
            HttpRequest request = HttpContext.Current.Request;

            string strUserIP;
            strUserIP = request.UserHostAddress;

            if (string.IsNullOrEmpty(strUserIP))
            {
                strUserIP = request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(strUserIP))
            {
                strUserIP = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            if (string.IsNullOrEmpty(strUserIP))
            {
                return "0.0.0.0";
            }

            return strUserIP;
        }
        #endregion

        #region 获取客户端Mac地址
        /// <summary>
        /// 获取客户端Mac地址
        /// </summary>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public static string GetClientMac()
        {
            //获取客户端IP
            string strUserIP = GetClientIP();

            string dirResults = "";
            ProcessStartInfo psi = new ProcessStartInfo();
            Process proc = new Process();
            psi.FileName = "nbtstat";
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = true;
            psi.Arguments = "-A " + strUserIP;
            psi.UseShellExecute = false;
            proc = Process.Start(psi);
            dirResults = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            dirResults = dirResults.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            Regex reg = new Regex("Mac[ ]{0,}Address[ ]{0,}=[ ]{0,}(?((.)*?)) __MAC", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Match mc = reg.Match(dirResults + "__MAC");
            if (mc.Success)
            {
                return mc.Groups["key"].Value;
            }
            else
            {
                reg = new Regex("Host not found", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                mc = reg.Match(dirResults);
                if (mc.Success)
                {
                    return "找不到主机。Host not found!";
                }
                else
                {
                    return "";
                }
            }
        }
        #endregion

        #region 获取浏览器版本号
        /// <summary>
        /// 获取浏览器版本号
        /// </summary>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public static string GetClientBrowser()
        {
            string browser;

            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
            browser = bc.Browser + bc.Version;

            return browser;
        }
        #endregion

        #region 获取客户端操作系统版本号
        /// <summary>
        /// 获取客户端操作系统版本号
        /// </summary>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public static string GetClientOS()
        {
            string Agent = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];

            if (Agent.IndexOf("NT 4.0") > 0)
            {
                return "Windows NT ";
            }
            else if (Agent.IndexOf("NT 5.0") > 0)
            {
                return "Windows 2000";
            }
            else if (Agent.IndexOf("NT 5.1") > 0)
            {
                return "Windows XP";
            }
            else if (Agent.IndexOf("NT 5.2") > 0)
            {
                return "Windows 2003";
            }
            else if (Agent.IndexOf("NT 6.0") > 0)
            {
                return "Windows Vista";//Windows Server 2008
            }
            else if (Agent.IndexOf("NT 6.1") > 0)
            {
                return "Windows 7";//Windows Server 2008 R2
            }
            else if (Agent.IndexOf("NT 6.2") > 0)
            {
                return "Windows 8";//Windows Slate
            }
            else if (Agent.IndexOf("NT 6.3") > 0)
            {
                return "Windows 9";
            }
            else if (Agent.IndexOf("WindowsCE") > 0)
            {
                return "Windows CE";
            }
            else if (Agent.IndexOf("NT") > 0)
            {
                return "Windows NT ";
            }
            else if (Agent.IndexOf("9x") > 0)
            {
                return "Windows ME";
            }
            else if (Agent.IndexOf("98") > 0)
            {
                return "Windows 98";
            }
            else if (Agent.IndexOf("95") > 0)
            {
                return "Windows 95";
            }
            else if (Agent.IndexOf("Win32") > 0)
            {
                return "Win32";
            }
            else if (Agent.IndexOf("Linux") > 0)
            {
                return "Linux";
            }
            else if (Agent.IndexOf("SunOS") > 0)
            {
                return "SunOS";
            }
            else if (Agent.IndexOf("Mac") > 0)
            {
                return "Mac";
            }
            else if (Agent.IndexOf("Linux") > 0)
            {
                return "Linux";
            }
            else if (Agent.IndexOf("Windows") > 0)
            {
                return "Windows";
            }
            return "未知类型";
        }
        #endregion

        #region 获取客户端信息
        /// <summary>
        /// 获取客户端信息
        /// </summary>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public static Dictionary<string, string> GetClientInfos()
        {
            HttpRequest request = HttpContext.Current.Request;

            // 将客户端的信息添加到 Dictionary 中
            Dictionary<string, string> clientInfos = new Dictionary<string, string>();
            try
            {
                string ip = GetClientIP();
                string userAgent = request.UserAgent == null ? "无" : request.UserAgent;

                if (request.ServerVariables["HTTP_UA_CPU"] == null)
                {
                    clientInfos.Add("CPU 类型", "未知");
                }
                else
                {
                    clientInfos.Add("CPU 类型", request.ServerVariables["HTTP_UA_CPU"]);
                }
                clientInfos.Add("操作系统", GetClientOS());
                clientInfos.Add("IP 地址", ip);
                //clientInfos.Add("MAC 地址", GetClientMac());
                if (request.Browser.ClrVersion == null)
                {
                    clientInfos.Add(".NET CLR 版本", "不支持");
                }
                else
                {
                    clientInfos.Add(".NET CLR 版本", request.Browser.ClrVersion.ToString());
                }

                clientInfos.Add("浏览器", request.Browser.Browser + request.Browser.Version);
                clientInfos.Add("支持 ActiveX", request.Browser.ActiveXControls ? "支持" : "不支持");
                clientInfos.Add("支持 Cookies", request.Browser.Cookies ? "支持" : "不支持");
                clientInfos.Add("支持 CSS", request.Browser.SupportsCss ? "支持" : "不支持");
                clientInfos.Add("语言", request.UserLanguages[0]);

                string httpAccept = request.ServerVariables["HTTP_ACCEPT"];
                if (httpAccept == null)
                {
                    clientInfos.Add("计算机/手机", "未知");
                }
                else if (httpAccept.IndexOf("wap") > -1)
                {
                    clientInfos.Add("计算机/手机", "手机");
                }
                else
                {
                    clientInfos.Add("计算机/手机", "计算机");
                }

                clientInfos.Add("Platform", request.Browser.Platform);
                //clientInfos.Add("Win16", request.Browser.Win16 ? "是" : "不是");
                //clientInfos.Add("Win32", request.Browser.Win32 ? "是" : "不是");

                if (request.ServerVariables["HTTP_ACCEPT_ENCODING"] == null)
                {
                    clientInfos.Add("Http Accept Encoding", "无");
                }
                else
                {
                    clientInfos.Add("Http Accept Encoding", request.ServerVariables["HTTP_ACCEPT_ENCODING"]);
                }

                clientInfos.Add("User Agent", userAgent);

                return clientInfos;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取客户端信息（IP、浏览器版本、操作系统、User Agent）
        /// </summary>
        /// <param name="bIncludeAgent">是否包含User Agent</param>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public static string GetClientInfo(bool bIncludeAgent = false)
        {
            string userAgent = HttpContext.Current.Request.UserAgent == null ? "null" : HttpContext.Current.Request.UserAgent;
            //获取IP
            string strClientInfo = "IP: " + GetClientIP() +            //获取IP
                " <br/> Browser: " + GetClientBrowser() +         //获取浏览器版本
                " <br/> OS: " + GetClientOS();                           //获取操作系统
            if (bIncludeAgent)
            {
                strClientInfo += " <br/> User Agent: " + userAgent;  //获取User Agent
            }
            return strClientInfo;
        }
        #endregion

        #endregion

    }
}