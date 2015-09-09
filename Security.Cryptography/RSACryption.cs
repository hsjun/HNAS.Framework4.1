using System;
using System.Security.Cryptography;
using System.Text;

namespace HNAS.Framework4.Security.Cryptography
{
    /// <summary>
    /// RSA加解密，RSA数字签名
    /// </summary>
    /// Copyright (c) 2012 海南海航航空信息系统有限公司
    /// 创 建 人：王宇（wang_yu5）
    /// 创建日期：2012年2月23日
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    public class RSACryption
    {

        #region RSA 加密解密

        #region RSA 的密钥产生
        //产生私钥 和公钥 
        /// <summary>
        /// 产生私钥 和公钥
        /// </summary>
        /// <param name="strXMLKeys">私钥</param>
        /// <param name="strXMLPublicKey">公钥</param>
        public void RSAKey(out string strXMLKeys, out string strXMLPublicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            strXMLKeys = rsa.ToXmlString(true);
            strXMLPublicKey = rsa.ToXmlString(false);
        }
        #endregion

        #region RSA的加密函数

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <remarks>
        /// 密钥必须是XML的行式
        /// 该加密方式有长度限制
        /// </remarks>
        /// <param name="strXMLPublicKey">公钥</param>
        /// <param name="strEncryptString">明文</param>
        /// <returns>密文</returns>
        public string RSAEncrypt(string strXMLPublicKey, string strEncryptString)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(strXMLPublicKey);
            byte[] PlainTextBArray = (new UnicodeEncoding()).GetBytes(strEncryptString);
            byte[] CypherTextBArray = rsa.Encrypt(PlainTextBArray, false);
            string Result = Convert.ToBase64String(CypherTextBArray);

            return Result;
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="strXMLPublicKey">公钥</param>
        /// <param name="EncryptString">明文</param>
        /// <returns>密文</returns>
        public string RSAEncrypt(string strXMLPublicKey, byte[] EncryptString)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(strXMLPublicKey);
            byte[] CypherTextBArray = rsa.Encrypt(EncryptString, false);
            string Result = Convert.ToBase64String(CypherTextBArray);

            return Result;
        }
        #endregion

        #region RSA的解密函数

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="strXMLPrivateKey">私钥</param>
        /// <param name="m_strDecryptString">密文</param>
        /// <returns>明文</returns>
        public string RSADecrypt(string strXMLPrivateKey, string m_strDecryptString)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(strXMLPrivateKey);
            byte[] PlainTextBArray = Convert.FromBase64String(m_strDecryptString);
            byte[] DypherTextBArray = rsa.Decrypt(PlainTextBArray, false);
            string Result = (new UnicodeEncoding()).GetString(DypherTextBArray);

            return Result;
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="strXMLPrivateKey">私钥</param>
        /// <param name="DecryptString">密文</param>
        /// <returns>明文</returns>
        public string RSADecrypt(string strXMLPrivateKey, byte[] DecryptString)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(strXMLPrivateKey);
            byte[] DypherTextBArray = rsa.Decrypt(DecryptString, false);
            string Result = (new UnicodeEncoding()).GetString(DypherTextBArray);

            return Result;
        }
        #endregion

        #endregion

        #region RSA数字签名

        #region 获取Hash描述表

        /// <summary>
        /// 获取MD5值
        /// </summary>
        /// <param name="strSource">明文</param>
        /// <param name="HashData">MD5值</param>
        /// <returns>bool</returns>
        public bool GetHash(string strSource, ref byte[] HashData)
        {
            //从字符串中取得Hash描述 
            HashAlgorithm MD5 = HashAlgorithm.Create("MD5");
            byte[] Buffer = Encoding.GetEncoding("UTF-8").GetBytes(strSource);
            HashData = MD5.ComputeHash(Buffer);

            return true;
        }

        /// <summary>
        /// 获取MD5值
        /// </summary>
        /// <param name="strSource">明文</param>
        /// <param name="strHashData">MD5值</param>
        /// <returns>bool</returns>
        public bool GetHash(string strSource, ref string strHashData)
        {
            //从字符串中取得Hash描述 
            HashAlgorithm MD5 = HashAlgorithm.Create("MD5");
            byte[] Buffer = Encoding.GetEncoding("UTF-8").GetBytes(strSource);
            byte[] HashData = MD5.ComputeHash(Buffer);

            strHashData = Convert.ToBase64String(HashData);

            return true;
        }

        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        /// <param name="objFile">文件</param>
        /// <param name="HashData">MD5值</param>
        /// <returns>bool</returns>
        public bool GetHash(System.IO.FileStream objFile, ref byte[] HashData)
        {
            //从文件中取得Hash描述 
            HashAlgorithm MD5 = HashAlgorithm.Create("MD5");
            HashData = MD5.ComputeHash(objFile);
            objFile.Close();

            return true;
        }

        /// <summary>
        /// 获取MD5值
        /// </summary>
        /// <param name="objFile">文件</param>
        /// <param name="strHashData">MD5值</param>
        /// <returns>bool</returns>
        public bool GetHash(System.IO.FileStream objFile, ref string strHashData)
        {
            //从文件中取得Hash描述 
            HashAlgorithm MD5 = HashAlgorithm.Create("MD5");
            byte[] HashData = MD5.ComputeHash(objFile);
            objFile.Close();

            strHashData = Convert.ToBase64String(HashData);

            return true;
        }
        #endregion

        #region RSA签名
        /// <summary>
        /// RSA签名 
        /// </summary>
        /// <param name="strPrivateKey">私钥</param>
        /// <param name="HashbyteSignature">签名</param>
        /// <param name="EncryptedSignatureData">密文</param>
        /// <returns>bool</returns>
        public bool SignatureFormatter(string strPrivateKey, byte[] HashbyteSignature, ref byte[] EncryptedSignatureData)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            RSA.FromXmlString(strPrivateKey);
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(RSA);
            //设置签名的算法为MD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //执行签名 
            EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);

            return true;
        }

        /// <summary>
        /// RSA签名 
        /// </summary>
        /// <param name="strPrivateKey">私钥</param>
        /// <param name="HashbyteSignature">签名</param>
        /// <param name="strEncryptedSignatureData">密文</param>
        /// <returns></returns>
        public bool SignatureFormatter(string strPrivateKey, byte[] HashbyteSignature, ref string strEncryptedSignatureData)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            RSA.FromXmlString(strPrivateKey);
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(RSA);
            //设置签名的算法为MD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //执行签名 
            byte[] EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);
            strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);

            return true;
        }

        /// <summary>
        /// RSA签名 
        /// </summary>
        /// <param name="strPrivateKey">私钥</param>
        /// <param name="strHashbyteSignature">签名</param>
        /// <param name="EncryptedSignatureData">密文</param>
        /// <returns>bool</returns>
        public bool SignatureFormatter(string strPrivateKey, string strHashbyteSignature, ref byte[] EncryptedSignatureData)
        {
            byte[] HashbyteSignature = Convert.FromBase64String(strHashbyteSignature);
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            RSA.FromXmlString(strPrivateKey);
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(RSA);
            //设置签名的算法为MD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //执行签名 
            EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);

            return true;
        }

        /// <summary>
        /// RSA签名 
        /// </summary>
        /// <param name="strPrivateKey">私钥</param>
        /// <param name="strHashbyteSignature">签名</param>
        /// <param name="strEncryptedSignatureData">密文</param>
        /// <returns>bool</returns>
        public bool SignatureFormatter(string strPrivateKey, string strHashbyteSignature, ref string strEncryptedSignatureData)
        {
            byte[] HashbyteSignature = Convert.FromBase64String(strHashbyteSignature);
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            RSA.FromXmlString(strPrivateKey);
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(RSA);
            //设置签名的算法为MD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //执行签名 
            byte[] EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);

            strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);

            return true;
        }
        #endregion

        #region RSA 签名验证
        /// <summary>
        /// RSA 签名验证
        /// </summary>
        /// <param name="strPublicKey">公钥</param>
        /// <param name="HashbyteDeformatter">签名</param>
        /// <param name="DeformatterData">明文</param>
        /// <returns></returns>
        public bool SignatureDeformatter(string strPublicKey, byte[] HashbyteDeformatter, byte[] DeformatterData)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            RSA.FromXmlString(strPublicKey);
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSA);
            //指定解密的时候HASH算法为MD5 
            RSADeformatter.SetHashAlgorithm("MD5");

            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// RSA 签名验证
        /// </summary>
        /// <param name="strPublicKey">公钥</param>
        /// <param name="strHashbyteDeformatter">签名</param>
        /// <param name="DeformatterData">明文</param>
        /// <returns>bool</returns>
        public bool SignatureDeformatter(string strPublicKey, string strHashbyteDeformatter, byte[] DeformatterData)
        {
            byte[] HashbyteDeformatter;

            HashbyteDeformatter = Convert.FromBase64String(strHashbyteDeformatter);

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            RSA.FromXmlString(strPublicKey);
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSA);
            //指定解密的时候HASH算法为MD5 
            RSADeformatter.SetHashAlgorithm("MD5");

            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// RSA 签名验证
        /// </summary>
        /// <param name="strPublicKey">公钥</param>
        /// <param name="HashbyteDeformatter">签名</param>
        /// <param name="strDeformatterData">明文</param>
        /// <returns>bool</returns>
        public bool SignatureDeformatter(string strPublicKey, byte[] HashbyteDeformatter, string strDeformatterData)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            RSA.FromXmlString(strPublicKey);
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSA);
            //指定解密的时候HASH算法为MD5 
            RSADeformatter.SetHashAlgorithm("MD5");

            byte[] DeformatterData = Convert.FromBase64String(strDeformatterData);

            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// RSA 签名验证
        /// </summary>
        /// <param name="strPublicKey">公钥</param>
        /// <param name="strHashbyteDeformatter">签名</param>
        /// <param name="strDeformatterData">明文</param>
        /// <returns>bool</returns>
        public bool SignatureDeformatter(string strPublicKey, string strHashbyteDeformatter, string strDeformatterData)
        {
            byte[] HashbyteDeformatter = Convert.FromBase64String(strHashbyteDeformatter); RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            RSA.FromXmlString(strPublicKey);
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSA);
            //指定解密的时候HASH算法为MD5 
            RSADeformatter.SetHashAlgorithm("MD5");

            byte[] DeformatterData = Convert.FromBase64String(strDeformatterData);

            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #endregion

    }
}
