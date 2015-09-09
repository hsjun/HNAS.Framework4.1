using System;
using System.Web;

namespace HNAS.Framework4.WebCommon
{

    /// <summary>
    /// 用于方便使用Cookie的扩展工具类
    /// </summary>
    /// Copyright (c) 2011 海航航空信息系统有限公司
    /// 创 建 人：HNAS .Net Framework 4.0 项目组
    /// 创建日期：2012年2月22日
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    public static class CookieExtension
    {
        /// <summary>
        /// 从一个Cookie中读取字符串值。
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string GetString(HttpCookie cookie)
        {
            if (cookie == null)
                return null;

            return cookie.Value;
        }

        /// <summary>
        /// 从一个Cookie中读取 Int 值。
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static int ToInt(HttpCookie cookie, int defaultVal = 0)
        {
            if (cookie == null)
                return defaultVal;

            int.TryParse(cookie.Value, out defaultVal);
            return defaultVal;
        }

        /// <summary>
        /// 从一个Cookie中读取值并转成指定的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static T ConverTo<T>(HttpCookie cookie)
        {
            if (cookie == null)
                return default(T);

            return (T)Convert.ChangeType(cookie.Value, typeof(T));
        }


        /// <summary>
        /// 将一个对象写入到Cookie
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="strName"></param>
        /// <param name="expries"></param>
        public static void WriteCookie(object obj, string strName, DateTime? expries)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (string.IsNullOrEmpty(strName))
                throw new ArgumentNullException("strName");


            HttpCookie cookie = new HttpCookie(strName, obj.ToString());

            if (expries.HasValue)
                cookie.Expires = expries.Value;

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 删除指定的Cookie
        /// </summary>
        /// <param name="strName"></param>
        public static void ClearCookie(string strName)
        {
            if (string.IsNullOrEmpty(strName))
                throw new ArgumentNullException("strName");

            HttpCookie cookie = new HttpCookie(strName);
            cookie.Values.Clear();
            // 删除Cookie，其实就是设置一个【过期的日期】
            cookie.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}
