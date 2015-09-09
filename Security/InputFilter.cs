using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace HNAS.Framework4.Security
{

    /// <summary>
    /// 用于统一安全验证
    /// </summary>
    /// Copyright (c) 2011 海航航空信息系统有限公司
    /// 创 建 人：谭强
    /// 创建日期：2012-9-20
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：
    public static class InputFilter
    {
        //敏感词路径
        private const string path = "~/sensitiveword.txt";
        private const string cachekey = "sensitiveword";

        #region 防止sql注入（数字类型）

        /// <summary>
        /// 防止sql注入（数字类型）。
        /// 当要把参数拼接到sql语句中，且这个参数是sting类型，但在sql语句中必须是数字型，符合
        /// "*** where a = "+参数 或类似结构时，可以采用此方法检测。用来验证字符串是否是数字型
        /// 在拼接sql语句之前使用此方法检测即可,无论小数，整数，正负数都是true
        /// </summary>
        /// 创 建 人：谭强
        /// 创建日期：2011-12-14
        /// 修 改 人：
        /// 修改日期：
        /// 版 本：
        /// <param name="Num">要嵌入sql的字符串参数</param>
        /// <returns>是否是数字</returns>
        public static bool isNum(string Num)
        {
            Regex regex = new Regex(@"^(\+?|\-?)?(0|[1-9]+[0-9]*)+(\.[0-9]+)?$");
            return regex.IsMatch(Num.Trim());
        }
        #endregion

        #region 验证邮箱验证邮箱
        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsEmail(string source)
        {
            return Regex.IsMatch(source, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.IgnoreCase);
        }
        //public static bool HasEmail(string source)
        //{
        //    return Regex.IsMatch(source, @"[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})", RegexOptions.IgnoreCase);
        //}
        #endregion

        #region 验证网址
        /// <summary>
        /// 验证网址
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsUrl(string source)
        {
            return Regex.IsMatch(source, @"^(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\.))+(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&amp;%_\./-~-]*)?$", RegexOptions.IgnoreCase);
        }
        //public static bool HasUrl(string source)
        //{
        //    return Regex.IsMatch(source, @"(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\.))+(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&amp;%_\./-~-]*)?", RegexOptions.IgnoreCase);
        //}
        #endregion

        #region 验证日期
        /**/
        /// <summary>
        /// 验证日期
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDateTime(string source)
        {
            try
            {
                DateTime time = Convert.ToDateTime(source);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 验证手机号，验证手机号,目前只验证它是纯数字，并且在11-20位之间
        /// <summary>
        /// 验证手机号,目前只验证它是纯数字，并且在11-20位之间
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsMobile(string source)
        {
            return Regex.IsMatch(source, @"^\d{11,20}$", RegexOptions.IgnoreCase);
        }
        ////public static bool HasMobile(string source)
        ////{
        ////    return Regex.IsMatch(source, @"1[35]\d{9}", RegexOptions.IgnoreCase);
        ////}
        #endregion

        #region 验证电话号码，支持分机号
        /// <summary>
        /// 验证电话号码，支持分机号
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is phone; otherwise, <c>false</c>.
        /// </returns>
        /// 创 建 人：刘鹏（liu-peng33）
        /// 创建日期：2012/9/29
        /// 修 改 人：
        /// 修改日期：
        public static bool IsPhone(string value)
        {
            return Regex.IsMatch(value, @"^(0[0-9]{2,3}\-)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?$", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 验证IP
        /**/
        /// <summary>
        /// 验证IP
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsIP(string source)
        {
            return Regex.IsMatch(source, @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$", RegexOptions.IgnoreCase);
        }
        //public static bool HasIP(string source)
        //{
        //    return Regex.IsMatch(source, @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])", RegexOptions.IgnoreCase);
        //}
        #endregion

        #region 验证身份证是否有效
        /**/
        /// <summary>
        /// 验证身份证是否有效
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool IsIDCard(string Id)
        {
            if (Id.Length == 18)
            {
                bool check = IsIDCard18(Id);
                return check;
            }
            else if (Id.Length == 15)
            {
                bool check = IsIDCard15(Id);
                return check;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// IsIDCard18
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool IsIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// IsIDCard15
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool IsIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }
        #endregion

        #region 是不是Int型的
        /// <summary>
        /// 是不是Int型的
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsInt(string source)
        {
            Regex regex = new Regex(@"^(-){0,1}\d+$");
            if (regex.Match(source).Success)
            {
                if ((long.Parse(source) > 0x7fffffffL) || (long.Parse(source) < -2147483648L))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        #endregion

        #region 看字符串的长度是不是在限定数之间 一个中文为两个字符
        /// <summary>
        /// 看字符串的长度是不是在限定数之间 一个中文为两个字符
        /// </summary>
        /// <param name="source">字符串</param>
        /// <param name="begin">大于等于</param>
        /// <param name="end">小于等于</param>
        /// <returns></returns>
        public static bool IsLengthStr(string source, int begin, int end)
        {
            int length = Regex.Replace(source, @"[^\x00-\xff]", "OK").Length;
            if ((length <= begin) && (length >= end))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 是不是中国电话，格式010-85849685
        /**/
        /// <summary>
        /// 是不是中国电话，格式010-85849685
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsTel(string source)
        {
            return Regex.IsMatch(source, @"^\d{3,4}-?\d{6,8}$", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 邮政编码 6个数字
        /**/
        /// <summary>
        /// 邮政编码 6个数字
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsPostCode(string source)
        {
            return Regex.IsMatch(source, @"^\d{6}$", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 中文
        /**/
        /// <summary>
        /// 中文
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsChinese(string source)
        {
            return Regex.IsMatch(source, @"^[\u4e00-\u9fa5]+$", RegexOptions.IgnoreCase);
        }
        //public static bool hasChinese(string source)
        //{
        //    return Regex.IsMatch(source, @"[\u4e00-\u9fa5]+", RegexOptions.IgnoreCase);
        //}
        #endregion

        #region 验证是不是正常字符 字母，数字，下划线的组合
        /**/
        /// <summary>
        /// 验证是不是正常字符 字母，数字，下划线的组合
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNormalChar(string source)
        {
            return Regex.IsMatch(source, @"[\w\d_]+", RegexOptions.IgnoreCase);
        }
        #endregion


        #region 防SQL注入，过滤XSS攻击脚本，敏感词过滤
        /// <summary>
        /// 防SQL注入，过滤XSS攻击脚本，敏感词过滤
        /// </summary>   
        /// <param name="html">传入字符串</param>
        /// <param name="bFilterSensitiveWord">是否过滤敏感词，默认不过滤</param>
        /// <param name="wordPath">敏感词文件路径，默认"~/sensitiveword.txt"</param>
        /// <param name="replaceWord">替换后的字符，默认"***"</param>
        /// <returns>过滤后的字符串</returns>   
        public static string Filter(string html, bool bFilterSensitiveWord = false, string wordPath = path, string replaceWord = "***")
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;

            // CR(0a) ，LF(0b) ，TAB(9) 除外，过滤掉所有的不打印出来字符.   
            // 目的防止这样形式的入侵 ＜java\0script＞   
            // 注意：\n, \r,   \t 可能需要单独处理，因为可能会要用到   
            string ret = System.Text.RegularExpressions.Regex.Replace(html, "([\x00-\x08][\x0b-\x0c][\x0e-\x20])", string.Empty);

            //替换所有可能的16进制构建的恶意代码   
            //<IMG SRC=&#X40&#X61&#X76&#X61&#X73&#X63&#X72&#X69&#X70&#X74&#X3A&#X61&_#X6C&#X65&#X72&#X74&#X28&#X27&#X58&#X53&#X53&#X27&#X29>   
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()~`;:?+/={}[]-_|'\"\\";
            for (int i = 0; i < chars.Length; i++)
            {
                ret = System.Text.RegularExpressions.Regex.Replace(ret, string.Concat("(&#[x|X]0{0,}", Convert.ToString((int)chars[i], 16).ToLower(), ";?)"),
                    chars[i].ToString(), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            //过滤\t, \n, \r构建的恶意代码
            string[] keywords = {"javascript", "vbscript", "expression", "applet", "meta", "xml", "blink", "link", "style", "script", "embed", "object", "iframe", "frame", "frameset", "ilayer", "layer", "bgsound", "title", "base"
             ,"onabort", "onactivate", "onafterprint", "onafterupdate","alert", "onbeforeactivate", "onbeforecopy", "onbeforecut", "onbeforedeactivate", "onbeforeeditfocus", "onbeforepaste", "onbeforeprint", "onbeforeunload", "onbeforeupdate", "onblur", "onbounce", "oncellchange", "onchange", "onclick", "oncontextmenu", "oncontrolselect", "oncopy", "oncut", "ondataavailable", "ondatasetchanged", "ondatasetcomplete", "ondblclick", "ondeactivate", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "onerror", "onerrorupdate", "onfilterchange", "onfinish", "onfocus", "onfocusin", "onfocusout", "onhelp", "onkeydown", "onkeypress", "onkeyup", "onlayoutcomplete", "onload", "onlosecapture", "onmousedown", "onmouseenter", "onmouseleave", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onmove", "onmoveend", "onmovestart", "onpaste", "onpropertychange", "onreadystatechange", "onreset", "onresize", "onresizeend", "onresizestart", "onrowenter", "onrowexit", "onrowsdelete", "onrowsinserted", "onscroll", "onselect", "onselectionchange", "onselectstart", "onstart", "onstop", "onsubmit", "onunload"};
            bool found = true;
            while (found)
            {
                var retBefore = ret;
                for (int i = 0; i < keywords.Length; i++)
                {
                    string pattern = "/?";
                    for (int j = 0; j < keywords[i].Length; j++)
                    {
                        if (j > 0)
                            pattern = string.Concat(pattern, '(', "(&#[x|X]0{0,8}([9][a][b]);?)?", "|(&#0{0,8}([9][10][13]);?)?",
                                ")?");
                        pattern = string.Concat(pattern, keywords[i][j]);
                    }
                    //string replacement = string.Concat(keywords[i].Substring(0, 2), "＜x＞", keywords[i].Substring(2));
                    string replacement = "*";
                    ret = System.Text.RegularExpressions.Regex.Replace(ret, pattern, replacement, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (ret == retBefore)
                        found = false;
                }
            }

            ret = ret.Replace("'", "\"").Replace("/", "").Replace("<", "").Replace(">", "");

            //敏感词过滤
            if (bFilterSensitiveWord)
            {
                ret = ReplaceKeyword(ret, wordPath, replaceWord);
            }
            return ret;
        }

        #endregion

        #region 屏蔽敏感词
        /// <summary>
        /// 屏蔽敏感词
        /// </summary>
        /// <param name="s">待过滤字符串</param>
        /// <param name="wordPath">敏感词文件路径，默认"~/sensitiveword.txt"</param>
        /// <param name="replaceWord">替换后的字符，默认"***"</param>
        /// <returns></returns>
        public static string ReplaceKeyword(string s, string wordPath = path, string replaceWord = "***")
        {
            //s = s.Replace(" ", "");//替换类似"共 产 党"的敏感词
            return Regex.Replace(s, GetReg(wordPath), replaceWord, RegexOptions.IgnoreCase);
        }
        #endregion

        #region 获取正则表达式
        /// <summary>
        /// 获取正则表达式
        /// </summary>
        /// <param name="wordPath">敏感词文件路径，默认"~/sensitiveword.txt"</param>
        /// <returns></returns>
        private static string GetReg(string wordPath = path)
        {
            object o = GetCache(cachekey);
            string s = "";
            if (o == null)
            {
                string p = HttpContext.Current.Server.MapPath(wordPath);

                using (StreamReader sr = new StreamReader(p, Encoding.UTF8))
                {
                    s = sr.ReadToEnd();
                }
                s = s.Trim();
                s = s.Replace("\r\n", "|");
                s = Regex.Replace(s, @"(\|)\1{1,}", "|", RegexOptions.IgnoreCase);
                s = string.Format("({0})", s);

                SetCache(cachekey, s);
            }
            else
            {
                s = o.ToString();
            }
            return s;
        }
        #endregion

        #region 获取当前应用程序指定CacheKey的Cache值
        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2012-1-5
        /// 修 改 人：
        /// 修改日期：
        private static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }
        #endregion

        #region 设置当前应用程序指定CacheKey的Cache值
        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2012-1-5
        /// 修 改 人：
        /// 修改日期：
        private static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }
        #endregion

    }
}