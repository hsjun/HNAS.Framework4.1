using System;
using System.Xml;
using System.Configuration;

namespace HNAS.Framework4.Security.Cryptography
{
	/// <summary>
	/// ServiceBase 的摘要说明。
	/// </summary>
	public class ServiceBase : System.Web.Services.WebService
	{
		//获取webconfig中配置的用户名、密码、公钥路径
        private readonly string strUserName = ConfigurationManager.AppSettings["UserName"];
		private readonly string strPassWord = ConfigurationManager.AppSettings["PassWord"];
		private readonly string strPublicKeyPath = ConfigurationManager.AppSettings["PublicKeyPath"];//公钥存放路径

		//ColumnFilter需要过滤的关键字
		private readonly string strSQLFILTER_COLUMN_WITHBLANK =ConfigurationManager.AppSettings["SQLFILTER_COLUMN_WITHBLANK"];
		private readonly string[] SQLFILTER_COLUMN_WITHBLANK;

		private readonly string strSQLFILTER_COLUMN_NOBLANK =ConfigurationManager.AppSettings["SQLFILTER_COLUMN_NOBLANK"];
		private readonly string[] SQLFILTER_COLUMN_NOBLANK;

		//RowFilter需要过滤的关键字
		private readonly string strSQLFILTER_ROW_WITHBLANK =ConfigurationManager.AppSettings["SQLFILTER_ROW_WITHBLANK"];
		private readonly string[] SQLFILTER_ROW_WITHBLANK;

		private readonly string strSQLFILTER_ROW_NOBLANK =ConfigurationManager.AppSettings["SQLFILTER_ROW_NOBLANK"];
		private readonly string[] SQLFILTER_ROW_NOBLANK;


		/// <summary>
		/// 构造函数
		/// </summary>
		public ServiceBase()
		{
			SQLFILTER_COLUMN_WITHBLANK =strSQLFILTER_COLUMN_WITHBLANK.Split(',');
			SQLFILTER_COLUMN_NOBLANK = strSQLFILTER_COLUMN_NOBLANK.Split(',');
			SQLFILTER_ROW_WITHBLANK = strSQLFILTER_ROW_WITHBLANK.Split(',');
			SQLFILTER_ROW_NOBLANK =strSQLFILTER_ROW_NOBLANK.Split(',');
		}

		#region 验证SOAP头信息
		/// <summary>
		/// 验证SOAP头信息
		/// </summary>
		/// <param name="usernameToken">SOAP头</param>
        /// <param name="xmlParameter">参数</param>
		/// <returns>验证结果</returns>
		public string ValidateUsernameToken(UsernameToken usernameToken,string xmlParameter)
		{
			
			#region SOAPHeader验证
			string userName = usernameToken.Username;  //用户名
			string passWord = usernameToken.Password;  //签名信息
			string expires = usernameToken.Expires;    //过期时间
			string nonce = usernameToken.Nonce;        //随机数base64

            //if (usernameToken == null)
            //{
            //    return "SOAP头用户令牌为空";
            //}
			if (userName == null || passWord == null || nonce == null || expires == null)
			{
				return "SOAP头用户令牌字段内容不全";
			}
		    if (userName.Trim().Equals(String.Empty)
		        || passWord.Trim().Equals(String.Empty)
		        || nonce.Trim().Equals(String.Empty)
		        || expires.Trim().Equals(String.Empty))
		    {
		        return "SOAP头用户令牌所有字段内容都不能为空";
		    }

		    DateTime expiresTime;

			try
			{
				expiresTime = DateTime.Parse(expires);
			}
			catch(Exception)
			{
				return "SOAP头用户令牌字段Expires不是有效的时间值";
			}

			if (DateTime.Now > expiresTime)
			{
				return "消息已经过期失效";
			}
			#endregion

			#region 签名验证
			//需签名的数据（用户名+密码+随机数+过期时间+xmlParameter）
			string strData=strUserName+strPassWord+nonce+expires+xmlParameter;
			//已签名的数据（取usernameToken.passWord值）
			string strSignData= usernameToken.Password;
			bool bSign=new RSAEncrypt().UnsignMD5WithRSA(strSignData,strData,strPublicKeyPath);//调用签名方法
			
			if(!bSign)
			{
				return "签名验证失败";
			}

			#endregion

			return "OK";

		}
		#endregion

		#region 获取参数值函数
		/// <summary>
		/// 获取参数值
		/// </summary>
		/// <param name="xd">xml参数</param>
		/// <param name="nodeName">需要获取参数值的参数名</param>
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

		#region 行过滤的特殊字符转换
		/// <summary>
		/// 行过滤的特殊字符转换
		/// </summary>
		/// <param name="message">需转换的字符串</param>
		/// <returns>转换后的字符串</returns>
		public string GetQueryCondition(string message)
		{
			//字符替换转换
			message = message.Replace("&apos;", "'");
			message = message.Replace("&gt;", ">");
			message = message.Replace("&lt;", "<");
			message = message.Replace("&quot;", "\"");
			message = message.Replace("&amp;", "&");

			return message;
		}
		#endregion

		#region 验证公共参数（列、开始时间、结束时间）
		/// <summary>
		/// 验证公共参数（列、开始时间、结束时间）
		/// </summary>
		/// <param name="columnFilter">行</param>
		/// <param name="dStartDate">开始时间</param>
		/// <param name="dEndDate">结束时间</param>
		/// <returns>验证结果</returns>
		public string ValidatePara(string columnFilter,string dStartDate,string dEndDate)
		{
            if (string.IsNullOrEmpty(columnFilter.Trim()))//列不允许为空
			{
				return "参数columnFilter不允许为空";
			}

			dStartDate = dStartDate.Trim();
			if (dStartDate.Equals(String.Empty) || dStartDate == null)//开始时间不允许为空
			{
				return "参数dStartDate不允许为空";
			}
			//验证dStartDate格式   ^\d{4}-\d{2}-\d{2}$ 
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
					return "参数dStartDate格式错误，必须为yyyy-MM-dd格式";
				}
			}
			else
			{
				return "参数dStartDate格式错误，必须为yyyy-MM-dd格式";
			}

			dEndDate = dEndDate.Trim();
			if (dEndDate.Equals(String.Empty) || dEndDate == null)//结束时间不允许为空
			{
				return "参数dEndDate不允许为空";
			}
			//验证dEndDate格式
			bool EndDateFlag = reg.IsMatch(dEndDate); 
			if(EndDateFlag)
			{
				try
				{
					dEndDate = Convert.ToDateTime(dEndDate).ToString("yyyy-MM-dd");
				}
				catch
				{
					return "参数dEndDate格式错误，必须为yyyy-MM-dd格式";
				}
			}
			else
			{
				return "参数dEndDate格式错误，必须为yyyy-MM-dd格式";
			}
			return "OK";

		}
		#endregion

		#region ColumnFilter参数防sql注入
		/// <summary>
		/// ColumnFilter参数防sql注入
		/// </summary>
		/// <param name="strValue">需判断的字符串</param>
		/// <returns>true 含有关键字</returns>
		public bool SqlFilter_Column(string strValue)
		{
			foreach (string strkey in SQLFILTER_COLUMN_WITHBLANK)
			{
				//包含关键字
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
				//包含关键字
				if (strValue.ToLower().IndexOf(strkey) > -1)
				{
					return true;
				}
			}
			return false;
		}
		#endregion

		#region RowFilter参数防sql注入
		/// <summary>
		/// RowFilter参数防sql注入
		/// </summary>
		/// <param name="strValue">需判断的字符串</param>
		/// <returns>true 含有关键字</returns>
		public bool SqlFilter_Row(string strValue)
		{
			foreach (string strkey in SQLFILTER_ROW_WITHBLANK)
			{
				//包含关键字
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
				//包含关键字
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
