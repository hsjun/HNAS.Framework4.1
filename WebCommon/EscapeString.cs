using System.Text;

namespace HNAS.Framework4.WebCommon
{
    /// <summary>
    /// 转义字符串类
    /// </summary>
    /// Copyright (c) 2012 海航航空信息系统有限公司
    /// 创 建 人：王宇
    /// 创建日期：2012-2-20
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    public static class EscapeString
    {
        // Methods
        /// <summary>
        /// 根据分隔符组合字符串
        /// </summary>
        /// <param name="strCode"></param>
        /// <param name="strTag">分隔符</param>
        /// <returns></returns>
        public static string ArraySplit(string[] strCode, string strTag)
        {
            string str = "";
            foreach (string strItem in strCode)
            {
                str += strItem + strTag;
            }

            if (str.Length > strTag.Length)
            {
                str = str.Substring(0, str.Length - strTag.Length);
            }
            return str;
        }

        /// <summary>
        /// 分隔字符串
        /// </summary>
        /// <param name="strCode"></param>
        /// <param name="strTag"></param>
        /// <returns></returns>
        public static string[] StringSplit(string strCode, string strTag)
        {
            char[] separator = strTag.ToCharArray();
            return strCode.Split(separator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputString"></param>
        /// <param name="bIsToConvertSingleQuote"></param>
        /// <returns></returns>
        public static string DisplayText(string outputString, bool bIsToConvertSingleQuote = false)
        {
            outputString = outputString.Replace("&#12288;", "");
            outputString = outputString.Replace("　", "&nbsp;&nbsp;");
            outputString = outputString.Replace(" ", "&nbsp;");
            if (bIsToConvertSingleQuote)
            {
                outputString = outputString.Replace("''", "'");
            }
            return outputString;
        }

        /// <summary>
        /// 字符转义
        /// </summary>
        /// <param name="importString"></param>
        /// <param name="bIsTitle"></param>
        /// <param name="bIsToConvertSingleQuote"></param>
        /// <returns></returns>
        public static string ImportText(string importString, bool bIsTitle = true, bool bIsToConvertSingleQuote = false)
        {
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(importString.Trim()))
            {
                for (int i = 0; i < importString.Length; i++)
                {
                    switch (importString[i])
                    {
                        case '&':
                            {
                                builder.Append("&amp;");
                                continue;
                            }
                        case '\'':
                            {
                                builder.Append(bIsToConvertSingleQuote ? "''" : "'");
                                continue;
                            }
                        case '"':
                            {
                                builder.Append("&quot;");
                                continue;
                            }
                        case '<':
                            {
                                builder.Append("&lt;");
                                continue;
                            }
                        case '>':
                            {
                                builder.Append("&gt;");
                                continue;
                            }
                    }
                    builder.Append(importString[i]);
                }
                builder.Replace("\r\n", "<br>");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importString"></param>
        /// <returns></returns>
        public static string ImpText(string importString)
        {
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(importString.Trim()))
            {
                for (int i = 0; i < importString.Length; i++)
                {
                    char ch = importString[i];
                    if (ch == '\'')
                    {
                        builder.Append("''");
                    }
                    else
                    {
                        builder.Append(importString[i]);
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 还原转义字符
        /// </summary>
        /// <param name="outputString"></param>
        /// <param name="bIsToConvertSingleQuote"></param>
        /// <returns></returns>
        public static string OutputText(string outputString, bool bIsToConvertSingleQuote = false)
        {
            outputString = outputString.Replace("<br>&#12288;&#12288;", "\r\n");
            outputString = outputString.Replace("&#12288;", "");
            outputString = outputString.Replace("<br>", "\r\n");
            outputString = outputString.Replace("&nbsp;", " ");
            outputString = outputString.Replace("&quot;", "\"");
            outputString = outputString.Replace("&lt;", "<");
            outputString = outputString.Replace("&gt;", ">");
            outputString = outputString.Replace("&nbsp", " ");
            outputString = outputString.Replace("&quot", "\"");
            outputString = outputString.Replace("&lt", "<");
            outputString = outputString.Replace("&gt", ">");
            if (bIsToConvertSingleQuote)
            {
                outputString = outputString.Replace("''", "'");
            }
            outputString = outputString.Replace("&amp;", "&");
            return outputString;
        }

        /// <summary>
        /// 标题省略
        /// </summary>
        /// <param name="strTitle"></param>
        /// <param name="iMaxLen"></param>
        /// <returns></returns>
        public static string Turn(string strTitle, int iMaxLen)
        {
            if (strTitle.Length > iMaxLen)
            {
                return (strTitle.Substring(0, iMaxLen - 3) + "…");
            }
            return strTitle;
        }
    }
}
