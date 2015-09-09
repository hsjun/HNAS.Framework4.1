using System;
using System.Collections;
using System.Xml;

namespace HNAS.Framework4.Security.Cryptography
{
    /// <summary>
    /// RSA加解密实现类
    /// </summary>
    /// Copyright (c) 2012 海南海航航空信息系统有限公司
    /// 创 建 人：王宇（wang_yu5）
    /// 创建日期：2012年2月23日
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    public class RSAEncrypt
    {
        #region 公钥加密字符串
        /// <summary>
        /// 公钥加密字符串
        /// </summary>
        /// <param name="strSource">待加密字符串</param>
        /// <param name="strPublicKeyPath">公钥存放路径</param>
        /// <returns>加密结果base64编码</returns>
        public string EncryptPublicKey(string strSource, string strPublicKeyPath)
        {
            XmlDocument doc = new XmlDocument();
            // 装入指定的XML文档
            doc.Load(strPublicKeyPath);
            XmlNodeList node = doc.GetElementsByTagName("RSAKeyValue");
            //读取公钥参数
            XmlNode selectSingleNode = node[0].SelectSingleNode("Modulus");
            if (selectSingleNode != null)
            {
                string strModulus = selectSingleNode.InnerText.Trim();
                XmlNode singleNode = node[0].SelectSingleNode("Exponent");
                if (singleNode != null)
                {
                    string strExponent = singleNode.InnerText.Trim();

                    byte[] bEncrypt = EncryptString(strSource, strExponent, strModulus);
                    //将加密结果进行base64编码返回
                    return Convert.ToBase64String(bEncrypt);
                }
            }
            return null;
        }


        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="strSource">待加密字符串</param>
        /// <param name="strPublicKeyPath">公钥存放路径</param>
        /// <returns>加密结果</returns>
        public byte[] EncryptPublicKeyByte(string strSource, string strPublicKeyPath)
        {
            XmlDocument doc = new XmlDocument();
            // 装入指定的XML文档
            doc.Load(strPublicKeyPath);
            XmlNodeList node = doc.GetElementsByTagName("RSAKeyValue");
            //读取公钥参数
            XmlNode selectSingleNode = node[0].SelectSingleNode("Modulus");
            if (selectSingleNode != null)
            {
                string strModulus = selectSingleNode.InnerText.Trim();
                XmlNode singleNode = node[0].SelectSingleNode("Exponent");
                if (singleNode != null)
                {
                    string strExponent = singleNode.InnerText.Trim();

                    byte[] bEncrypt = EncryptString(strSource, strExponent, strModulus);
                    return bEncrypt;
                }
            }
            return null;
        }
        #endregion

        #region 私钥解密字符串
        /// <summary>
        /// 私钥解密字符串
        /// </summary>
        /// <param name="strSource">待解密字符串（进行了base64编码的）</param>
        /// <param name="strPrivateKeyPath">私钥存放路径</param>
        /// <returns>解密结果</returns>
        public string DecryptPrivateKey(string strSource, string strPrivateKeyPath)
        {
            XmlDocument doc = new XmlDocument();
            // 装入指定的XML文档
            doc.Load(strPrivateKeyPath);
            XmlNodeList node = doc.GetElementsByTagName("RSAKeyValue");
            //读取私钥参数
            XmlNode selectSingleNode = node[0].SelectSingleNode("Modulus");
            if (selectSingleNode != null)
            {
                string strModulus = selectSingleNode.InnerText.Trim();
                XmlNode singleNode = node[0].SelectSingleNode("D");
                if (singleNode != null)
                {
                    string strD = singleNode.InnerText.Trim();
                    //将加密结果转成字节数组再进行解密
                    byte[] dataBytes = Convert.FromBase64String(strSource);
                    return DecryptBytes(dataBytes, strD, strModulus);
                }
            }
            return null;
        }

        /// <summary>
        /// 私钥解密字符串
        /// </summary>
        /// <param name="dSource">待解密的字节数组</param>
        /// <param name="strPrivateKeyPath">私钥存放路径</param>
        /// <returns>解密结果</returns>
        public string DecryptPrivateKeyByte(byte[] dSource, string strPrivateKeyPath)
        {
            XmlDocument doc = new XmlDocument();
            // 装入指定的XML文档
            doc.Load(strPrivateKeyPath);
            XmlNodeList node = doc.GetElementsByTagName("RSAKeyValue");
            //读取私钥参数
            XmlNode selectSingleNode = node[0].SelectSingleNode("Modulus");
            if (selectSingleNode != null)
            {
                string strModulus = selectSingleNode.InnerText.Trim();
                var singleNode = node[0].SelectSingleNode("D");
                if (singleNode != null)
                {
                    string strD = singleNode.InnerText.Trim();
                    return DecryptBytes(dSource, strD, strModulus);
                }
            }
            return null;
        }
        #endregion

        #region 加密字符串
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="strData">待加密字符串</param>
        /// <param name="strkeyNum">密钥</param>
        /// <param name="strnNum">modulus</param>
        /// <returns>加密结果</returns>
        private byte[] EncryptString(string strData, string strkeyNum, string strnNum)
        {
            BigInteger keyNum = new BigInteger(strkeyNum, 16);
            BigInteger nNum = new BigInteger(strnNum, 16);

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strData);//与java加解密统一用UTF8编码格式  System.Text.Encoding.GetEncoding("utf-8").GetBytes(strData);
            //byte[] bytes = System.Text.Encoding.Default.GetBytes(strData);

            int len = bytes.Length;
            int len1 = 0;
            int blockLen = 0;
            if ((len % 120) == 0)
                len1 = len / 120;
            else
                len1 = len / 120 + 1;
            //List<byte> tempbytes = new List<byte>();
            ArrayList tempbytes = new ArrayList();

            for (int i = 0; i < len1; i++)
            {
                if (len >= 120)
                {
                    blockLen = 120;
                }
                else
                {
                    blockLen = len;
                }
                byte[] oText = new byte[blockLen];
                Array.Copy(bytes, i * 120, oText, 0, blockLen);
                BigInteger biText = new BigInteger(oText);
                BigInteger biEnText = biText.modPow(keyNum, nNum);
                ////补位
                //byte[] testbyte = null;
                string resultStr = biEnText.ToHexString();
                if (resultStr.Length < 256)
                {
                    while (resultStr.Length != 256)
                    {
                        resultStr = "0" + resultStr;
                    }
                }
                byte[] returnBytes = new byte[128];
                for (int j = 0; j < returnBytes.Length; j++)
                {
                    returnBytes[j] = Convert.ToByte(resultStr.Substring(j * 2, 2), 16);
                }

                tempbytes.AddRange(returnBytes);
                len -= blockLen;
            }
            //return tempbytes.ToArray();

            byte[] btemp = new byte[tempbytes.Count];
            for (int i = 0; i < tempbytes.Count; i++)
            {
                btemp[i] = (byte)tempbytes[i];
            }
            return btemp;
        }

        #endregion

        #region 解密字符数组
        /// <summary>
        /// 解密字符数组
        /// </summary>
        /// <param name="dataBytes">待解密字符数组</param>
        /// <param name="strkeyNum">密钥</param>
        /// <param name="strnNum">modulus</param>
        /// <returns>解密结果</returns>
        private string DecryptBytes(byte[] dataBytes, string strkeyNum, string strnNum)
        {
            BigInteger keyNum = new BigInteger(strkeyNum, 16);
            BigInteger nNum = new BigInteger(strnNum, 16);
            int len = dataBytes.Length;
            int len1 = 0;
            int blockLen = 0;
            if (len % 128 == 0)
            {
                len1 = len / 128;
            }
            else
            {
                len1 = len / 128 + 1;
            }
            //            List<byte> tempbytes = new List<byte>();
            ArrayList tempbytes = new ArrayList();
            for (int i = 0; i < len1; i++)
            {
                if (len >= 128)
                {
                    blockLen = 128;
                }
                else
                {
                    blockLen = len;
                }
                byte[] oText = new byte[blockLen];
                Array.Copy(dataBytes, i * 128, oText, 0, blockLen);
                BigInteger biText = new BigInteger(oText);
                BigInteger biEnText = biText.modPow(keyNum, nNum);

                byte[] testbyte = biEnText.getBytes();

                tempbytes.AddRange(testbyte);
                len -= blockLen;
            }
            //            return System.Text.Encoding.UTF8.GetString(tempbytes.ToArray());
            byte[] btemp = new byte[tempbytes.Count];
            for (int i = 0; i < tempbytes.Count; i++)
            {
                btemp[i] = (byte)tempbytes[i];
            }
            return System.Text.Encoding.UTF8.GetString(btemp);//与java加解密统一用UTF8编码格式
            //return System.Text.Encoding.Default.GetString(btemp);
        }

        #endregion

        #region 签名
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="strSource">需签名的字符串</param>
        /// <param name="strPrivateKeyPath">私钥存放路径</param>
        /// <returns>签名结果</returns>
        public string SignMD5WithRSA(string strSource, string strPrivateKeyPath)
        {
            XmlDocument doc = new XmlDocument();
            // 装入指定的XML文档
            doc.Load(strPrivateKeyPath);
            XmlNodeList node = doc.GetElementsByTagName("RSAKeyValue");
            //参数值进行base64编码
            XmlNode selectSingleNode = node[0].SelectSingleNode("Modulus");
            if (selectSingleNode != null)
                selectSingleNode.InnerText = Convert.ToBase64String(GetBytes(selectSingleNode.InnerText.Trim()));
            XmlNode singleNode = node[0].SelectSingleNode("Exponent");
            if (singleNode != null)
                singleNode.InnerText = Convert.ToBase64String(GetBytes(singleNode.InnerText.Trim()));
            XmlNode xmlNode = node[0].SelectSingleNode("P");
            if (xmlNode != null)
                xmlNode.InnerText = Convert.ToBase64String(GetBytes(xmlNode.InnerText.Trim()));
            XmlNode selectSingleNode1 = node[0].SelectSingleNode("Q");
            if (selectSingleNode1 != null)
                selectSingleNode1.InnerText = Convert.ToBase64String(GetBytes(selectSingleNode1.InnerText.Trim()));
            XmlNode singleNode1 = node[0].SelectSingleNode("DP");
            if (singleNode1 != null)
                singleNode1.InnerText = Convert.ToBase64String(GetBytes(singleNode1.InnerText.Trim()));
            XmlNode xmlNode1 = node[0].SelectSingleNode("DQ");
            if (xmlNode1 != null)
                xmlNode1.InnerText = Convert.ToBase64String(GetBytes(xmlNode1.InnerText.Trim()));
            XmlNode node1 = node[0].SelectSingleNode("InverseQ");
            if (node1 != null)
                node1.InnerText = Convert.ToBase64String(GetBytes(node1.InnerText.Trim()));
            XmlNode selectSingleNode2 = node[0].SelectSingleNode("D");
            if (selectSingleNode2 != null)
                selectSingleNode2.InnerText = Convert.ToBase64String(GetBytes(selectSingleNode2.InnerText.Trim()));
            string strKeyPrivate = doc.InnerXml;

            RSACryption rSACryption = new RSACryption();
            String HashData = "";
            rSACryption.GetHash(strSource, ref HashData);
            //签名结果
            string strEncryptedSignatureData = "";
            rSACryption.SignatureFormatter(strKeyPrivate, HashData, ref strEncryptedSignatureData);
            return strEncryptedSignatureData;
        }
        #endregion

        #region 验证签名
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="strSignData">已签名的数据</param>
        /// <param name="strSource">原数据</param>
        /// <param name="strPublicKeyPath">公钥存放路径</param>
        /// <returns>true/false</returns>
        public bool UnsignMD5WithRSA(string strSignData, string strSource, string strPublicKeyPath)
        {
            XmlDocument doc = new XmlDocument();
            // 装入指定的XML文档
            doc.Load(strPublicKeyPath);
            XmlNodeList node = doc.GetElementsByTagName("RSAKeyValue");
            //参数值进行base64编码
            XmlNode selectSingleNode = node[0].SelectSingleNode("Modulus");
            if (selectSingleNode != null)
                selectSingleNode.InnerText = Convert.ToBase64String(GetBytes(selectSingleNode.InnerText.Trim()));
            XmlNode singleNode = node[0].SelectSingleNode("Exponent");
            if (singleNode != null)
                singleNode.InnerText = Convert.ToBase64String(GetBytes(singleNode.InnerText.Trim()));
            string strKeyPublic = doc.InnerXml;

            bool bReturn = false;
            RSACryption RC = new RSACryption();
            string m_strHashbyteDeformatter = "";
            RC.GetHash(strSource, ref m_strHashbyteDeformatter);

            if (RC.SignatureDeformatter(strKeyPublic, m_strHashbyteDeformatter, strSignData) == false)
            {
                bReturn = false;
            }
            else
            {
                bReturn = true;
            }
            return bReturn;

        }
        #endregion

        #region 将字符串转成字节数组
        /// <summary>
        /// 将字符串转成字节数组
        /// </summary>
        /// <param name="num">需转换的字符串</param>
        /// <returns>转换结果</returns>
        private byte[] GetBytes(String num)
        {
            BigInteger n = new BigInteger(num, 16);
            String s = n.ToString(2);
            if (s.Length % 8 > 0)
            {
                s = new String('0', 8 - s.Length % 8) + s;
            }
            byte[] data = new byte[s.Length / 8];
            String ocetstr;
            for (int i = 0; i < data.Length; i++)
            {
                ocetstr = s.Substring(8 * i, 8);
                data[i] = Convert.ToByte(ocetstr, 2);
            }
            return data;
        }

        #endregion
    }
}