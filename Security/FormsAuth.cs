using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace HNAS.Framework4.Security
{
    /// <summary>
    /// Forms认证类
    /// </summary>
    ///  Copyright (c) 2012 海南海航航空信息系统有限公司
    ///  创 建 人：王宇
    ///  创建日期：2012年2月16日
    ///  修 改 人：
    ///  修改日期：
    ///  版 本：1.0	
    public static class FormsAuth
    {
        #region 获取系统登录AD账号
        /// <summary>
        /// 获取登录AD账号
        /// </summary>
        /// <param name="strDomain">域</param>
        /// <returns>获取系统登录AD账号</returns>
        /// 创建人：王宇
        /// 创建时间：2012年2月16日
        /// 修 改 人：
        /// 修改日期：
        public static String GetADAccount(String strDomain = "HNANET")
        {
            String loginName = "";
            var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            if (windowsIdentity != null && windowsIdentity.IsAuthenticated)
            {
                loginName = windowsIdentity.Name;
                if (!String.IsNullOrEmpty(loginName) && loginName.Contains(strDomain))
                {
                    loginName = loginName.Split('\\')[1];
                }
            }

            return loginName;
        }

        /// <summary>
        /// Forms登录
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="username">账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="bCookiePersistent"></param>
        /// <param name="page">页面实例，写入Cookie</param>
        /// <returns></returns>
        /// 创建人：王宇
        /// 创建时间：2012年2月16日
        /// 修 改 人：
        /// 修改日期：
        public static bool FormsLogon(String domain, String username, String pwd, Page page, bool bCookiePersistent = false)
        {
            String adPath = "LDAP://" + domain; //Fully-qualified Domain Name
            LdapAuthentication adAuth = new LdapAuthentication(adPath);
            try
            {
                if (adAuth.IsAuthenticated(domain, username, pwd))
                {
                    String groups = adAuth.GetGroups();

                    //Create the ticket, and add the groups.
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, username, DateTime.Now, DateTime.Now.AddMinutes(60), bCookiePersistent, groups);

                    //Encrypt the ticket.
                    String encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    //Create a cookie, and then add the encrypted ticket to the cookie as data.
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                    if (bCookiePersistent)
                    {
                        authCookie.Expires = authTicket.Expiration;
                    }

                    //Add the cookie to the outgoing cookies collection.
                    page.Response.Cookies.Add(authCookie);

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region 获取Windows账号
        /// <summary>
        /// 获取Windows账号
        /// </summary>
        /// <param name="strDomain">域</param>
        /// <returns>Windows账号</returns>
        /// 创建人：王宇
        /// 创建时间：2012年5月28日
        /// 修 改 人：
        /// 修改日期：
        public static String GetAccount(String strDomain = "HNANET")
        {
            String loginName = "";
            var windowsIdentity = System.Web.HttpContext.Current.User.Identity;
            if (windowsIdentity != null && windowsIdentity.IsAuthenticated)
            {
                loginName = windowsIdentity.Name;
                if (!String.IsNullOrEmpty(loginName) && loginName.Contains(strDomain))
                {
                    loginName = loginName.Split('\\')[1];
                }
            }

            return loginName;
        }
        #endregion
    }
}
