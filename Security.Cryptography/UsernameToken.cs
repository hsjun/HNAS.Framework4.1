using System.Web.Services.Protocols;

namespace HNAS.Framework4.Security.Cryptography
{
	/// <summary>
	/// SOAP��֤�ඨ��
	/// </summary>
	public class UsernameToken : SoapHeader
	{
        /// <summary>
        /// �˺�
        /// </summary>
		public string Username;
        /// <summary>
        /// ����
        /// </summary>
		public string Password;
        /// <summary>
        /// ����ʱ��
        /// </summary>
		public string Expires;
        /// <summary>
        /// �����
        /// </summary>
		public string Nonce;
	}
}
