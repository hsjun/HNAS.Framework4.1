using System;

namespace HNAS.Framework4.WebCommon
{
    /// <summary>
    /// 本类实现阿拉伯数字到大写中文的转换
    /// 该类没有对非法数字进行判别,请事先自己判断数字是否合法
    /// </summary>
    /// Copyright (c) 2012 海航航空信息系统有限公司
    /// 创 建 人：王宇
    /// 创建日期：2012-2-20
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    public static class ChineseNum
    {
        /// <summary>
        /// 小写转大写
        /// </summary>
        /// <param name="p_num"></param>
        /// <returns></returns>
        public static string GetChineseNum(string p_num)
        {
            return NumToChn(p_num);
        }

        /// <summary>
        /// 小写金额转大写金额
        /// </summary>
        /// <param name="p_Money"></param>
        /// <returns></returns>
        public static string GetUpperMoney(double p_Money)
        {
            return GetMoneyChinese(p_Money);
        }

        /// <summary>
        /// 转换数字
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static char CharToNum(char x)
        {
            string stringChnNames = "零一二三四五六七八九";
            string stringNumNames = "0123456789";
            return stringChnNames[stringNumNames.IndexOf(x)];
        }

        /// <summary>
        /// 转换万以下整数
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static string WanStrToInt(string x)
        {
            string[] stringArrayLevelNames = new string[4] {"", "十", "百", "千"};
            string ret = "";
            int i;
            for (i = x.Length - 1; i >= 0; i--)
                if (x[i] == '0')
                {
                    ret = CharToNum(x[i]) + ret;
                }
                else
                {
                    ret = CharToNum(x[i]) + stringArrayLevelNames[x.Length - 1 - i] + ret;
                }
            while ((i = ret.IndexOf("零零")) != -1)
            {
                ret = ret.Remove(i, 1);
            }
            if (ret[ret.Length - 1] == '零' && ret.Length > 1)
            {
                ret = ret.Remove(ret.Length - 1, 1);
            }
            if (ret.Length >= 2 && ret.Substring(0, 2) == "一十")
            {
                ret = ret.Remove(0, 1);
            }
            return ret;
        }

        /// <summary>
        /// 转换整数
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static string StrToInt(string x)
        {
            int len = x.Length;
            string ret, temp;
            if (len <= 4)
            {
                ret = WanStrToInt(x);
            }
            else if (len <= 8)
            {
                ret = WanStrToInt(x.Substring(0, len - 4)) + "万";
                temp = WanStrToInt(x.Substring(len - 4, 4));
                if (temp.IndexOf("千") == -1 && temp != "")
                    ret += "零" + temp;
                else
                    ret += temp;
            }
            else
            {
                ret = WanStrToInt(x.Substring(0, len - 8)) + "亿";
                temp = WanStrToInt(x.Substring(len - 8, 4));
                if (temp.IndexOf("千") == -1 && temp != "")
                {
                    ret += "零" + temp;
                }
                else
                {
                    ret += temp;
                }
                ret += "万";
                temp = WanStrToInt(x.Substring(len - 4, 4));
                if (temp.IndexOf("千") == -1 && temp != "")
                {
                    ret += "零" + temp;
                }
                else
                {
                    ret += temp;
                }
            }
            int i;
            if ((i = ret.IndexOf("零万")) != -1)
            {
                ret = ret.Remove(i + 1, 1);
            }
            while ((i = ret.IndexOf("零零")) != -1)
            {
                ret = ret.Remove(i, 1);
            }
            if (ret[ret.Length - 1] == '零' && ret.Length > 1)
            {
                ret = ret.Remove(ret.Length - 1, 1);
            }
            return ret;
        }

        /// <summary>
        /// 转换小数
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static string StrToDouble(string x)
        {
            string ret = "";
            for (int i = 0; i < x.Length; i++)
            {
                ret += CharToNum(x[i]);
            }
            return ret;
        }

        /// <summary>
        /// 小写转大写
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static string NumToChn(string x)
        {
            if (x.Length == 0)
            {
                return "";
            }
            string ret = "";
            if (x[0] == '-')
            {
                ret = "负";
                x = x.Remove(0, 1);
            }
            if (x[0].ToString() == ".")
            {
                x = "0" + x;
            }
            if (x[x.Length - 1].ToString() == ".")
            {
                x = x.Remove(x.Length - 1, 1);
            }
            if (x.IndexOf(".") > -1)
            {
                ret += StrToInt(x.Substring(0, x.IndexOf("."))) + "点" + StrToDouble(x.Substring(x.IndexOf(".") + 1));
            }
            else
            {
                ret += StrToInt(x);
            }
            return ret;
        }

        /// <summary>
        /// 金额转换
        /// </summary>
        /// <param name="Money"></param>
        /// <returns></returns>
        public static string GetMoneyChinese(Double Money)
        {
            int i;
            string mstrSource;
            if (Money == 0)
            {
                return "零圆整";
            }
            mstrSource = Money.ToString("#0.00");
            i = mstrSource.IndexOf(".");
            if (i > 0)
            {
                mstrSource = mstrSource.Replace(".", "");
            }
            if (mstrSource.Substring(0, 1) == "0")
            {
                mstrSource = mstrSource.Remove(0, 1);
            }
            mstrSource = NumstrToChinese(mstrSource);
            if (mstrSource.Length == 0)
            {
                return "";
            }
            //负
            if (Money < 0)
            {
                mstrSource = "负" + mstrSource;
            }
            mstrSource = mstrSource.Replace("0", "零");
            mstrSource = mstrSource.Replace("1", "壹");
            mstrSource = mstrSource.Replace("2", "贰");
            mstrSource = mstrSource.Replace("3", "叁");
            mstrSource = mstrSource.Replace("4", "肆");
            mstrSource = mstrSource.Replace("5", "伍");
            mstrSource = mstrSource.Replace("6", "陆");
            mstrSource = mstrSource.Replace("7", "柒");
            mstrSource = mstrSource.Replace("8", "捌");
            mstrSource = mstrSource.Replace("9", "玖");
            mstrSource = mstrSource.Replace("M", "亿");
            mstrSource = mstrSource.Replace("W", "万");
            mstrSource = mstrSource.Replace("S", "仟");
            mstrSource = mstrSource.Replace("H", "佰");
            mstrSource = mstrSource.Replace("T", "拾");
            mstrSource = mstrSource.Replace("Y", "圆");
            mstrSource = mstrSource.Replace("J", "角");
            mstrSource = mstrSource.Replace("F", "分");
            if (mstrSource.Substring(mstrSource.Length - 1, 1) != "分")
            {
                mstrSource = mstrSource + "整";
            }
            return mstrSource;
        }

        /// <summary>
        /// 金额转换
        /// </summary>
        /// <param name="numstr"></param>
        /// <returns></returns>
        private static string NumstrToChinese(string numstr)
        {
            int i;
            int j;
            string mstrChar;
            string[] mstrFlag = new string[4];
            string mstrReturn = "";
            bool mblnAddzero = false;
            mstrFlag[0] = "";
            mstrFlag[1] = "T";
            mstrFlag[2] = "H";
            mstrFlag[3] = "S";
            for (i = 1; i <= numstr.Length; i++)
            {
                j = numstr.Length - i;
                mstrChar = numstr.Substring(i - 1, 1);
                if (mstrChar != "0" && j > 1)
                {
                    mstrReturn = mstrReturn + mstrChar + mstrFlag[(j - 2)%4];
                }
                if (mstrChar == "0" && mblnAddzero == false)
                {
                    mstrReturn = mstrReturn + "0";
                    mblnAddzero = true;
                }
                if (j == 14)
                {
                    if (mstrReturn.Substring(mstrReturn.Length - 1) == "0")
                    {
                        mstrReturn = mstrReturn.Substring(0, mstrReturn.Length - 1) + "W0";
                    }
                    else
                    {
                        mstrReturn = mstrReturn + "W";
                    }
                }
                if (j == 2)
                {
                    if (mstrReturn.Substring(mstrReturn.Length - 1, 1) == "0")
                    {
                        mstrReturn = mstrReturn.Substring(0, mstrReturn.Length - 1) + "Y0";
                    }
                    else
                    {
                        mstrReturn = mstrReturn + "Y";
                    }
                    //元
                }
                if (j == 6)
                {
                    if (mstrReturn.Length > 2)
                    {
                        if (mstrReturn.Substring(mstrReturn.Length - 2) != "M0")
                        {
                            if (mstrReturn.Substring(mstrReturn.Length - 1) == "0")
                            {
                                mstrReturn = mstrReturn.Substring(0, mstrReturn.Length - 1) + "W0";
                            }
                            else
                            {
                                mstrReturn = mstrReturn + "W";
                            }
                        }
                    }
                    else
                    {
                        if (mstrReturn.Substring(mstrReturn.Length - 1) == "0")
                        {
                            mstrReturn = mstrReturn.Substring(0, mstrReturn.Length - 1) + "W0";
                        }
                        else
                        {
                            mstrReturn = mstrReturn + "W";
                        }
                    }
                }
                if (j == 10)
                {
                    if (mstrReturn.Substring(mstrReturn.Length - 1) == "0")
                    {
                        mstrReturn = mstrReturn.Substring(0, mstrReturn.Length - 1) + "M0";
                    }
                    else
                    {
                        mstrReturn = mstrReturn + "M";
                    }
                }
                if (j == 0 && mstrChar != "0")
                {
                    mstrReturn = mstrReturn + mstrChar + "F";
                }
                if (j == 1 && mstrChar != "0")
                {
                    mstrReturn = mstrReturn + mstrChar + "J";
                }
                if (mstrChar != "0")
                {
                    mblnAddzero = false;
                }
            }
            if (mstrReturn.Substring(0, 1) == "1" && mstrReturn.Substring(1, 1) == mstrFlag[1])
            {
                mstrReturn = mstrReturn.Substring(1);
            }
            if (mstrReturn.Substring(mstrReturn.Length - 1, 1) == "0")
            {
                mstrReturn = mstrReturn.Substring(0, mstrReturn.Length - 1);
            }
            if (mstrReturn.Substring(0, 1) == "0")
            {
                mstrReturn = mstrReturn.Substring(1);
            }
            if (mstrReturn.Substring(mstrReturn.Length - 1, 1) == "M" ||
                mstrReturn.Substring(mstrReturn.Length - 1, 1) == "W" ||
                mstrReturn.Substring(mstrReturn.Length - 1, 1) == "S" ||
                mstrReturn.Substring(mstrReturn.Length - 1, 1) == "H" ||
                mstrReturn.Substring(mstrReturn.Length - 1, 1) == "T")
            {
                mstrReturn = mstrReturn + "Y";
            }
            return mstrReturn;
        }
    }
}