using System;
using System.Web;
using System.Web.UI;

namespace HNAS.Framework4.WebCommon
{
    /// <summary>
    /// 提示窗口类
    /// </summary>
    /// Copyright (c) 2012 海航航空信息系统有限公司
    /// 创 建 人：王宇
    /// 创建日期：2012-2-20
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    public static class AlertMsn
    {
        /// <summary>
        /// 默认提示消息
        /// </summary>
        private const String strDefaultMsg = "Hi, there!  O(∩_∩)O";

        #region 弹出确认消息
        /// <summary>
        /// 弹出确认消息
        /// </summary>
        /// <param name="bIfSuccess">操作成功标识，true跳转，false后退</param>
        /// <param name="strMessage">消息体</param>
        /// <param name="strSuccessUrl">成功后跳转页面地址</param>
        /// <param name="page">所属窗体页（默认当前页）</param>
        public static void ConfirmMsg(bool bIfSuccess, String strMessage, String strSuccessUrl, Page page = null)
        {
            if (String.IsNullOrEmpty(strMessage))
            {
                strMessage = strDefaultMsg;
            }
            if (page == null)
            {
                page = ((Page)HttpContext.Current.Handler);
            }
            if (bIfSuccess)
            {
                strMessage = "if(confirm('" + strMessage + "')){ location.href='" + page.ResolveClientUrl(strSuccessUrl) + "' ;} else{ window.location.reload(); }";
            }
            else
            {
                strMessage = "alert('" + strMessage + "');history.back();";
            }

            page.ClientScript.RegisterStartupScript(page.ClientScript.GetType(), DateTime.Now.Ticks.ToString(), strMessage, true);
        }
        #endregion

        #region 弹出消息
        /// <summary>
        /// 弹出消息
        /// </summary>
        /// <param name="strMessage">消息体</param>
        /// <param name="strRedirectUrl">跳转地址</param>
        /// <param name="page">所属窗体页（默认当前页）</param>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public static void PopMsn(String strMessage, String strRedirectUrl = "", Page page = null)
        {
            if (String.IsNullOrEmpty(strMessage))
            {
                strMessage = strDefaultMsg;
            }
            if (page == null)
            {
                page = ((Page)HttpContext.Current.Handler);
            }
            if (!String.IsNullOrEmpty(strRedirectUrl))
            {
                strRedirectUrl = "location.href='" + page.ResolveClientUrl(strRedirectUrl) + "';";
            }
            
            strMessage = "alert('" + strMessage + "');";
            page.ClientScript.RegisterStartupScript(page.ClientScript.GetType(), DateTime.Now.Ticks.ToString(), strMessage, true);
        }
        #endregion

        #region 弹出消息后关闭
        /// <summary>
        /// 弹出消息后关闭
        /// </summary>
        /// <param name="strMessage">消息体</param>
        /// <param name="bIsParentReload">是否刷新父页面</param>
        /// <param name="page">所属窗体页（默认当前页）</param>
        public static void PopMsn_Close(String strMessage, bool bIsParentReload = false, Page page = null)
        {
            if (String.IsNullOrEmpty(strMessage))
            {
                strMessage = strDefaultMsg;
            }
            if (page == null)
            {
                page = ((Page)HttpContext.Current.Handler);
            }

            strMessage = "alert('" + strMessage + "');";
            if (bIsParentReload)
            {
                strMessage += "if(opener!=null) window.opener.location.href=window.opener.location.href;window.close();";
            }
            page.ClientScript.RegisterStartupScript(page.ClientScript.GetType(), DateTime.Now.Ticks.ToString(), strMessage, true);
        }
        #endregion

        #region 页面跳转
        /// <summary>
        /// 页面跳转
        /// </summary>
        /// <param name="bIfSuccess">操作成功标识</param>
        /// <param name="strMessage">消息体</param>
        /// <param name="strSuccessUrl">成功跳转地址</param>
        /// <param name="strErrorUrl">失败跳转地址</param>
        /// <param name="page">所属窗体页（默认当前页）</param>
        public static void Redirect(bool bIfSuccess, String strMessage, String strSuccessUrl, String strErrorUrl, Page page = null)
        {
            if (page == null)
            {
                page = ((Page)HttpContext.Current.Handler);
            }

            if (bIfSuccess)
            {
                page.Response.Redirect(strSuccessUrl + "?Message=" + strMessage);
            }
            else
            {
                page.Response.Redirect(strErrorUrl + "?Message=" + strMessage);
            }
        }

        /// <summary>
        /// 永久重定向
        /// </summary>
        /// <param name="strUrl">消息体</param>
        /// <param name="page">所属窗体页（默认当前页）</param>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public static void RedirectPermanent(String strUrl, Page page = null)
        {
            if (page == null)
            {
                page = ((Page)HttpContext.Current.Handler);
            }
            page.Response.Redirect(strUrl);//2.0
            //page.Response.RedirectPermanent(strUrl);
        }

        #endregion

        #region 注册脚本
        /// <summary>
        /// 注册脚本
        /// </summary>
        /// <param name="strScript">脚本</param>
        /// <param name="addScriptTags">标识（设置为true则不需添加<script></script>）</param>
        /// <param name="page">所属窗体页（默认当前页）</param>
        public static void RegisterScript(String strScript, Boolean addScriptTags = true, Page page = null)
        {
            if (page == null)
            {
                page = ((Page)HttpContext.Current.Handler);
            }

            //注册脚本
            page.ClientScript.RegisterStartupScript(page.ClientScript.GetType(), DateTime.Now.Ticks.ToString(), strScript, addScriptTags);
        }
        #endregion
    }
}