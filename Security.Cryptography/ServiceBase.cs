using System;
using System.Xml;
using System.Configuration;

namespace HNAS.Framework4.Security.Cryptography
{
	/// <summary>
	/// ServiceBase ��ժҪ˵����
	/// </summary>
	public class ServiceBase : System.Web.Services.WebService
	{
		//��ȡwebconfig�����õ��û��������롢��Կ·��
        private readonly string strUserName = ConfigurationManager.AppSettings["UserName"];
		private readonly string strPassWord = ConfigurationManager.AppSettings["PassWord"];
		private readonly string strPublicKeyPath = ConfigurationManager.AppSettings["PublicKeyPath"];//��Կ���·��

		//ColumnFilter��Ҫ���˵Ĺؼ���
		private readonly string strSQLFILTER_COLUMN_WITHBLANK =ConfigurationManager.AppSettings["SQLFILTER_COLUMN_WITHBLANK"];
		private readonly string[] SQLFILTER_COLUMN_WITHBLANK;

		private readonly string strSQLFILTER_COLUMN_NOBLANK =ConfigurationManager.AppSettings["SQLFILTER_COLUMN_NOBLANK"];
		private readonly string[] SQLFILTER_COLUMN_NOBLANK;

		//RowFilter��Ҫ���˵Ĺؼ���
		private readonly string strSQLFILTER_ROW_WITHBLANK =ConfigurationManager.AppSettings["SQLFILTER_ROW_WITHBLANK"];
		private readonly string[] SQLFILTER_ROW_WITHBLANK;

		private readonly string strSQLFILTER_ROW_NOBLANK =ConfigurationManager.AppSettings["SQLFILTER_ROW_NOBLANK"];
		private readonly string[] SQLFILTER_ROW_NOBLANK;


		/// <summary>
		/// ���캯��
		/// </summary>
		public ServiceBase()
		{
			SQLFILTER_COLUMN_WITHBLANK =strSQLFILTER_COLUMN_WITHBLANK.Split(',');
			SQLFILTER_COLUMN_NOBLANK = strSQLFILTER_COLUMN_NOBLANK.Split(',');
			SQLFILTER_ROW_WITHBLANK = strSQLFILTER_ROW_WITHBLANK.Split(',');
			SQLFILTER_ROW_NOBLANK =strSQLFILTER_ROW_NOBLANK.Split(',');
		}

		#region ��֤SOAPͷ��Ϣ
		/// <summary>
		/// ��֤SOAPͷ��Ϣ
		/// </summary>
		/// <param name="usernameToken">SOAPͷ</param>
        /// <param name="xmlParameter">����</param>
		/// <returns>��֤���</returns>
		public string ValidateUsernameToken(UsernameToken usernameToken,string xmlParameter)
		{
			
			#region SOAPHeader��֤
			string userName = usernameToken.Username;  //�û���
			string passWord = usernameToken.Password;  //ǩ����Ϣ
			string expires = usernameToken.Expires;    //����ʱ��
			string nonce = usernameToken.Nonce;        //�����base64

            //if (usernameToken == null)
            //{
            //    return "SOAPͷ�û�����Ϊ��";
            //}
			if (userName == null || passWord == null || nonce == null || expires == null)
			{
				return "SOAPͷ�û������ֶ����ݲ�ȫ";
			}
		    if (userName.Trim().Equals(String.Empty)
		        || passWord.Trim().Equals(String.Empty)
		        || nonce.Trim().Equals(String.Empty)
		        || expires.Trim().Equals(String.Empty))
		    {
		        return "SOAPͷ�û����������ֶ����ݶ�����Ϊ��";
		    }

		    DateTime expiresTime;

			try
			{
				expiresTime = DateTime.Parse(expires);
			}
			catch(Exception)
			{
				return "SOAPͷ�û������ֶ�Expires������Ч��ʱ��ֵ";
			}

			if (DateTime.Now > expiresTime)
			{
				return "��Ϣ�Ѿ�����ʧЧ";
			}
			#endregion

			#region ǩ����֤
			//��ǩ�������ݣ��û���+����+�����+����ʱ��+xmlParameter��
			string strData=strUserName+strPassWord+nonce+expires+xmlParameter;
			//��ǩ�������ݣ�ȡusernameToken.passWordֵ��
			string strSignData= usernameToken.Password;
			bool bSign=new RSAEncrypt().UnsignMD5WithRSA(strSignData,strData,strPublicKeyPath);//����ǩ������
			
			if(!bSign)
			{
				return "ǩ����֤ʧ��";
			}

			#endregion

			return "OK";

		}
		#endregion

		#region ��ȡ����ֵ����
		/// <summary>
		/// ��ȡ����ֵ
		/// </summary>
		/// <param name="xd">xml����</param>
		/// <param name="nodeName">��Ҫ��ȡ����ֵ�Ĳ�����</param>
		/// <returns></returns>
		public string GetParameter(XmlDocument xd, string nodeName)
		{
			string ret = String.Empty;
			//			XmlDocument xd = new XmlDocument();
			//			xd.LoadXml(XmlParameter);
			XmlNode xn = xd.ChildNodes[0];

			if (xn.SelectSingleNode(nodeName) != null)
			{
				ret = xn.SelectSingleNode(nodeName).InnerText.Trim();
			}

			return ret;
		}
		#endregion

		#region �й��˵������ַ�ת��
		/// <summary>
		/// �й��˵������ַ�ת��
		/// </summary>
		/// <param name="message">��ת�����ַ���</param>
		/// <returns>ת������ַ���</returns>
		public string GetQueryCondition(string message)
		{
			//�ַ��滻ת��
			message = message.Replace("&apos;", "'");
			message = message.Replace("&gt;", ">");
			message = message.Replace("&lt;", "<");
			message = message.Replace("&quot;", "\"");
			message = message.Replace("&amp;", "&");

			return message;
		}
		#endregion

		#region ��֤�����������С���ʼʱ�䡢����ʱ�䣩
		/// <summary>
		/// ��֤�����������С���ʼʱ�䡢����ʱ�䣩
		/// </summary>
		/// <param name="columnFilter">��</param>
		/// <param name="dStartDate">��ʼʱ��</param>
		/// <param name="dEndDate">����ʱ��</param>
		/// <returns>��֤���</returns>
		public string ValidatePara(string columnFilter,string dStartDate,string dEndDate)
		{
            if (string.IsNullOrEmpty(columnFilter.Trim()))//�в�����Ϊ��
			{
				return "����columnFilter������Ϊ��";
			}

			dStartDate = dStartDate.Trim();
			if (dStartDate.Equals(String.Empty) || dStartDate == null)//��ʼʱ�䲻����Ϊ��
			{
				return "����dStartDate������Ϊ��";
			}
			//��֤dStartDate��ʽ   ^\d{4}-\d{2}-\d{2}$ 
			System.String ex = @"^\d{4}-\d{2}-\d{2}$"; 
			System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(ex); 
			bool StartDateFlag = reg.IsMatch(dStartDate); 
			if(StartDateFlag)
			{
				try
				{
					dStartDate = Convert.ToDateTime(dStartDate).ToString("yyyy-MM-dd");
				}
				catch
				{
					return "����dStartDate��ʽ���󣬱���Ϊyyyy-MM-dd��ʽ";
				}
			}
			else
			{
				return "����dStartDate��ʽ���󣬱���Ϊyyyy-MM-dd��ʽ";
			}

			dEndDate = dEndDate.Trim();
			if (dEndDate.Equals(String.Empty) || dEndDate == null)//����ʱ�䲻����Ϊ��
			{
				return "����dEndDate������Ϊ��";
			}
			//��֤dEndDate��ʽ
			bool EndDateFlag = reg.IsMatch(dEndDate); 
			if(EndDateFlag)
			{
				try
				{
					dEndDate = Convert.ToDateTime(dEndDate).ToString("yyyy-MM-dd");
				}
				catch
				{
					return "����dEndDate��ʽ���󣬱���Ϊyyyy-MM-dd��ʽ";
				}
			}
			else
			{
				return "����dEndDate��ʽ���󣬱���Ϊyyyy-MM-dd��ʽ";
			}
			return "OK";

		}
		#endregion

		#region ColumnFilter������sqlע��
		/// <summary>
		/// ColumnFilter������sqlע��
		/// </summary>
		/// <param name="strValue">���жϵ��ַ���</param>
		/// <returns>true ���йؼ���</returns>
		public bool SqlFilter_Column(string strValue)
		{
			foreach (string strkey in SQLFILTER_COLUMN_WITHBLANK)
			{
				//�����ؼ���
				if (strValue.ToLower().IndexOf(strkey) > -1)
				{
					if ((strValue.ToLower().IndexOf(strkey + " ") > -1) || (strValue.ToLower().IndexOf(" " + strkey) > -1))
					{
						return true;
					}
				}
			}
			foreach (string strkey in SQLFILTER_COLUMN_NOBLANK)
			{
				//�����ؼ���
				if (strValue.ToLower().IndexOf(strkey) > -1)
				{
					return true;
				}
			}
			return false;
		}
		#endregion

		#region RowFilter������sqlע��
		/// <summary>
		/// RowFilter������sqlע��
		/// </summary>
		/// <param name="strValue">���жϵ��ַ���</param>
		/// <returns>true ���йؼ���</returns>
		public bool SqlFilter_Row(string strValue)
		{
			foreach (string strkey in SQLFILTER_ROW_WITHBLANK)
			{
				//�����ؼ���
				if (strValue.ToLower().IndexOf(strkey) > -1)
				{
					if ((strValue.ToLower().IndexOf(strkey + " ") > -1) || (strValue.ToLower().IndexOf(" " + strkey) > -1))
					{
						return true;
					}
				}
			}
			foreach (string strkey in SQLFILTER_ROW_NOBLANK)
			{
				//�����ؼ���
				if (strValue.ToLower().IndexOf(strkey) > -1)
				{
					return true;
				}
			}
			return false;
		}
		#endregion
	}
}
