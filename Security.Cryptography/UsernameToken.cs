using System.Web.Services.Protocols;

namespace HNAS.Framework4.Security.Cryptography
{
	/// <summary>
	/// SOAP验证类定义
	/// </summary>
	public class UsernameToken : SoapHeader
	{
        /// <summary>
        /// 账号
        /// </summary>
		public string Username;
        /// <summary>
        /// 密码
        /// </summary>
		public string Password;
        /// <summary>
        /// 过期时间
        /// </summary>
		public string Expires;
        /// <summary>
        /// 随机数
        /// </summary>
		public string Nonce;
	}
}
