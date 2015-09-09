using System.Text.RegularExpressions;

namespace HNAS.Framework4.WebCommon
{
    /// <summary>
    /// 验证类
    /// </summary>
    /// Copyright (c) 2012 海航航空信息系统有限公司
    /// 创 建 人：王宇
    /// 创建日期：2012-2-20
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    public static class Validation
    {
        /// <summary>
        /// 账号验证
        /// </summary>
        /// <param name="strValue">账号</param>
        /// <param name="strAlertMessage">提示信息</param>
        /// <returns></returns>
        public static bool CheckAccount(string strValue, string strAlertMessage = null)
        {
            Regex regex = new Regex(@"^[a-zA-Z](\w*)$");
            if (!regex.IsMatch(strValue.Trim()))
            {
                if (!string.IsNullOrEmpty(strAlertMessage))
                {
                    strAlertMessage = "请填写格式正确的账号！";
                }
                AlertMsn.PopMsn(strAlertMessage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Email验证
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool CheckEmail(string strValue)
        {
            Regex regex = new Regex(@"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$");
            return regex.Match(strValue).Success;
        }

        /// <summary>
        /// 整数验证
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool CheckInteger(string strValue)
        {
            Regex regex = new Regex("^[0-9]+$");
            return regex.Match(strValue).Success;
        }

        /// <summary>
        /// 数字字母验证
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool CheckIntegerAndLetter(string strValue)
        {
            Regex regex = new Regex("^[a-zA-Z0-9_]+$");
            return regex.Match(strValue).Success;
        }

        /// <summary>
        /// 实数验证
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="strAlertMessage">提示信息，可为空</param>
        /// <returns></returns>
        public static bool CheckIsFloat(string strValue, string strAlertMessage = null)
        {
            strValue = strValue.Replace(@"/^(\s)*|(\s)*$/g", "");
            if (!Regex.IsMatch(strValue, @"^(-|\+)?\d*\.?\d+$"))
            {
                if (!string.IsNullOrEmpty(strAlertMessage))
                {
                    strAlertMessage = "请填写实数！";
                }
                AlertMsn.PopMsn(strAlertMessage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 正整数
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool CheckIsInteger(string strValue)
        {
            try
            {
                return (int.Parse(strValue) >= 0);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 数字验证
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="strAlertMessage">提示信息，可为空</param>
        /// <returns></returns>
        public static bool CheckIsNumber(string strValue, string strAlertMessage = null)
        {
            Regex regex = new Regex("^[0-9]*$");
            if (!regex.IsMatch(strValue.Trim()))
            {
                if (string.IsNullOrEmpty(strAlertMessage))
                {
                    strAlertMessage = "请填写数字！";
                }
                AlertMsn.PopMsn(strAlertMessage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 非负实数验证
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="strAlertMessage">提示信息，可为空</param>
        /// <returns></returns>
        public static bool CheckIsUnsignedFloat(string strValue, string strAlertMessage = null)
        {
            strValue = strValue.Replace(@"/^(\s)*|(\s)*$/g", "");
            bool flag = Regex.IsMatch(strValue, @"^(\+)?\d*\.?\d+$");
            if (!flag)
            {
                if (string.IsNullOrEmpty(strAlertMessage))
                {
                    strAlertMessage = "请填写非负实数！";
                }
                AlertMsn.PopMsn(strAlertMessage);
            }
            return flag;
        }

        /// <summary>
        /// 正整数验证
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="strAlertMessage">提示信息，可为空</param>
        /// <returns></returns>
        public static bool CheckIsUnsignedInteger(string strValue, string strAlertMessage = null)
        {
            Regex regex = new Regex(@"^\d*[123456789]\d*$");
            if (!regex.IsMatch(strValue.Trim()))
            {
                if (string.IsNullOrEmpty(strAlertMessage))
                {
                    strAlertMessage = "请填写正整数！";
                }
                AlertMsn.PopMsn(strAlertMessage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 长度验证
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="iMax">最大长度</param>
        /// <param name="strAlertMessage">提示信息，可为空</param>
        /// <returns></returns>
        public static bool CheckLength(string strValue, int iMax, string strAlertMessage = null)
        {
            string str = strValue.Trim();
            int num = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] >= '\0') && (str[i] <= '\x00ff'))
                {
                    num++;
                }
                else
                {
                    num += 2;
                }
            }
            if (num > iMax)
            {
                if (string.IsNullOrEmpty(strAlertMessage))
                {
                    strAlertMessage = string.Concat(new object[] { "输入的字符串不要超过", iMax, "个字符或者", iMax / 2, "个汉字！" });
                }
                AlertMsn.PopMsn(strAlertMessage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 手机号验证
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool CheckMobile(string strValue)
        {
            Regex regex = new Regex(@"^(13[0-9]|15[0-9]|18[0-9])\d{8}$");
            return regex.Match(strValue).Success;
        }

        /// <summary>
        /// 非空验证
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="strAlertMessage">提示信息，可为空</param>
        /// <returns></returns>
        public static bool CheckNull(string strValue, string strAlertMessage = null)
        {
            if (strValue.Trim().Length < 1)
            {
                if (string.IsNullOrEmpty(strAlertMessage))
                {
                    strAlertMessage = "请填写信息！";
                }
                AlertMsn.PopMsn(strAlertMessage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 电话号码验证
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool CheckPhone(string strValue)
        {
            Regex regex = new Regex(@"^(0[0-9]{2,3}\-)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?$");
            return regex.Match(strValue).Success;
        }

        /// <summary>
        /// 邮政编码验证
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool CheckPostCode(string strValue)
        {
            Regex regex = new Regex(@"^\d{6}$");
            return regex.Match(strValue).Success;
        }
    }
}
