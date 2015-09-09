
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace HNAS.Framework4.Security
{
    #region IP归属地类
    /// <summary>
    /// IP归属地类
    /// </summary>
    ///  创 建 人：王宇
    ///  创建日期：2011年11月25日
    ///  修 改 人：
    ///  修改日期：
    ///  Copyright (c) 2011 海南海航航空信息系统有限公司
    ///  版 本：1.0	
    public class IPLocation
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP
        {
            get;
            set;
        }
        /// <summary>
        /// 归属地
        /// </summary>
        public string Country
        {
            get;
            set;
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Local
        {
            get;
            set;
        }
    }
    #endregion

    #region IP归属地查询类
    /// <summary>
    /// IP归属地查询类（支持纯真IP数据库）
    /// </summary>
    ///  创 建 人：王宇
    ///  创建日期：2011年11月25日
    ///  修 改 人：
    ///  修改日期：
    ///  Copyright (c) 2011 海南海航航空信息系统有限公司
    ///  版 本：1.0	
    public class IPLocator
    {
        private byte[] data;
        Regex regex = new Regex(@"(((\d{1,2})|(1\d{2})|(2[0-4]\d)|(25[0-5]))\.){3}((\d{1,2})|(1\d{2})|(2[0-4]\d)|(25[0-5]))");
        long firstStartIpOffset;
        long lastStartIpOffset;
        /// <summary>
        /// IPCount
        /// </summary>
        public long Count 
        { 
            get;
            private set;
        }

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataPath">纯真IP数据库路径</param>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public IPLocator(string dataPath)
        {
            using (FileStream fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
            }
            byte[] buffer = new byte[8];
            Array.Copy(data, 0, buffer, 0, 8);
            firstStartIpOffset = ((buffer[0] + (buffer[1] * 0x100)) + ((buffer[2] * 0x100) * 0x100)) + (((buffer[3] * 0x100) * 0x100) * 0x100);
            lastStartIpOffset = ((buffer[4] + (buffer[5] * 0x100)) + ((buffer[6] * 0x100) * 0x100)) + (((buffer[7] * 0x100) * 0x100) * 0x100);
            Count = Convert.ToInt64((double)(((double)(lastStartIpOffset - firstStartIpOffset)) / 7.0));

            if (Count <= 1L)
            {
                throw new ArgumentException("ip FileDataError");
            }
        }
        #endregion

        #region 查询IP归属地
        /// <summary>
        /// 查询IP归属地
        /// </summary>
        /// <param name="ip"></param>
        /// <returns>IPLocation IP,Country,Local</returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        public IPLocation Query(string ip)
        {
            if (!regex.Match(ip).Success)
            {
                throw new ArgumentException("IP格式错误");
            }
            IPLocation ipLocation = new IPLocation();
            ipLocation.IP = ip;
            long intIP = IpToInt(ip);
            if ((intIP >= IpToInt("127.0.0.1") && (intIP <= IpToInt("127.255.255.255"))))
            {
                ipLocation.Country = "本机内部环回地址";
                ipLocation.Local = "";
            }
            else
            {
                if ((((intIP >= IpToInt("0.0.0.0")) && (intIP <= IpToInt("2.255.255.255"))) || ((intIP >= IpToInt("64.0.0.0")) && (intIP <= IpToInt("126.255.255.255")))) ||
                ((intIP >= IpToInt("58.0.0.0")) && (intIP <= IpToInt("60.255.255.255"))))
                {
                    ipLocation.Country = "网络保留地址";
                    ipLocation.Local = "";
                }
            }
            long right = Count;
            long left = 0L;
            long middle = 0L;
            long startIp = 0L;
            long endIpOff = 0L;
            long endIp = 0L;
            int countryFlag = 0;
            while (left < (right - 1L))
            {
                middle = (right + left) / 2L;
                startIp = GetStartIp(middle, out endIpOff);
                if (intIP == startIp)
                {
                    left = middle;
                    break;
                }
                if (intIP > startIp)
                {
                    left = middle;
                }
                else
                {
                    right = middle;
                }
            }
            startIp = GetStartIp(left, out endIpOff);
            endIp = GetEndIp(endIpOff, out countryFlag);
            if ((startIp <= intIP) && (endIp >= intIP))
            {
                string local;
                ipLocation.Country = GetCountry(endIpOff, countryFlag, out local);
                ipLocation.Local = local;
            }
            else
            {
                ipLocation.Country = "未知";
                ipLocation.Local = "";
            }
            return ipLocation;
        }
        #endregion

        #region 相关私有方法

        /// <summary>
        /// IP转为long型
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        private long IpToInt(string ip)
        {
            char[] separator = new char[] { '.' };
            if (ip.Split(separator).Length == 3)
            {
                ip = ip + ".0";
            }
            string[] strArray = ip.Split(separator);
            long num2 = ((long.Parse(strArray[0]) * 0x100L) * 0x100L) * 0x100L;
            long num3 = (long.Parse(strArray[1]) * 0x100L) * 0x100L;
            long num4 = long.Parse(strArray[2]) * 0x100L;
            long num5 = long.Parse(strArray[3]);
            return (((num2 + num3) + num4) + num5);
        }

        /// <summary>
        /// long型转为IP
        /// </summary>
        /// <param name="ip_Int"></param>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        private string IntToIP(long ip_Int)
        {
            long num = (long)((ip_Int & 0xff000000L) >> 0x18);
            if (num < 0L)
            {
                num += 0x100L;
            }
            long num2 = (ip_Int & 0xff0000L) >> 0x10;
            if (num2 < 0L)
            {
                num2 += 0x100L;
            }
            long num3 = (ip_Int & 0xff00L) >> 8;
            if (num3 < 0L)
            {
                num3 += 0x100L;
            }
            long num4 = ip_Int & 0xffL;
            if (num4 < 0L)
            {
                num4 += 0x100L;
            }
            return (num.ToString() + "." + num2.ToString() + "." + num3.ToString() + "." + num4.ToString());
        }

        /// <summary>
        /// 获取起始IP
        /// </summary>
        /// <param name="left"></param>
        /// <param name="endIpOff"></param>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        private long GetStartIp(long left, out long endIpOff)
        {
            long leftOffset = firstStartIpOffset + (left * 7L);
            byte[] buffer = new byte[7];
            Array.Copy(data, leftOffset, buffer, 0, 7);
            endIpOff = (Convert.ToInt64(buffer[4].ToString()) + (Convert.ToInt64(buffer[5].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[6].ToString()) * 0x100L) * 0x100L);
            return ((Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L)) + (((Convert.ToInt64(buffer[3].ToString()) * 0x100L) * 0x100L) * 0x100L);
        }

        /// <summary>
        /// 获取结束IP
        /// </summary>
        /// <param name="endIpOff"></param>
        /// <param name="countryFlag"></param>
        /// <returns></returns>
        private long GetEndIp(long endIpOff, out int countryFlag)
        {
            byte[] buffer = new byte[5];
            Array.Copy(data, endIpOff, buffer, 0, 5);
            countryFlag = buffer[4];
            return ((Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L)) + (((Convert.ToInt64(buffer[3].ToString()) * 0x100L) * 0x100L) * 0x100L);
        }

        /// <summary> 
        /// 查询所属国家。Gets the country. 
        /// </summary> 
        /// <param name="endIpOff">The end ip off.</param> 
        /// <param name="countryFlag">The country flag.</param> 
        /// <param name="local">The local.</param> 
        /// <returns>country</returns> 
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        private string GetCountry(long endIpOff, int countryFlag, out string local)
        {
            string country = "";
            long offset = endIpOff + 4L;
            switch (countryFlag)
            {
                case 1:
                case 2:
                    country = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    offset = endIpOff + 8L;
                    local = (1 == countryFlag) ? "" : GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    break;
                default:
                    country = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    local = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    break;
            }
            return country;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="countryFlag"></param>
        /// <param name="endIpOff"></param>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        private string GetFlagStr(ref long offset, ref int countryFlag, ref long endIpOff)
        {
            int flag = 0;
            byte[] buffer = new byte[3];

            while (true)
            {
                //用于向前累加偏移量 
                long forwardOffset = offset;
                flag = data[forwardOffset++];
                //没有重定向 
                if (flag != 1 && flag != 2)
                {
                    break;
                }
                Array.Copy(data, forwardOffset, buffer, 0, 3);
                forwardOffset += 3;
                if (flag == 2)
                {
                    countryFlag = 2;
                    endIpOff = offset - 4L;
                }
                offset = (Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L);
            }
            if (offset < 12L)
            {
                return "";
            }
            return GetStr(ref offset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        ///  创 建 人：王宇
        ///  创建日期：2011年11月25日
        ///  修 改 人：
        ///  修改日期：
        private string GetStr(ref long offset)
        {
            byte lowByte = 0;
            byte highByte = 0;
            StringBuilder stringBuilder = new StringBuilder();
            byte[] bytes = new byte[2];
            Encoding encoding = Encoding.GetEncoding("GB2312");
            while (true)
            {
                lowByte = data[offset++];
                if (lowByte == 0)
                {
                    return stringBuilder.ToString();
                }
                if (lowByte > 0x7f)
                {
                    highByte = data[offset++];
                    bytes[0] = lowByte;
                    bytes[1] = highByte;
                    if (highByte == 0)
                    {
                        return stringBuilder.ToString();
                    }
                    stringBuilder.Append(encoding.GetString(bytes));
                }
                else
                {
                    stringBuilder.Append((char)lowByte);
                }
            }
        }

        #endregion

    }
    #endregion

} 
