using System;
using System.Data.Linq.Mapping;
using System.Web.UI.WebControls;
using System.Text;
using System.Reflection;

namespace HNAS.Framework4.WebCommon
{
    /// <summary>
    /// 页面基类
    /// </summary>
    /// Copyright (c) 2011 海航航空信息系统有限公司
    /// 创 建 人：HNAS .Net Framework 4.0 项目组
    /// 创建日期：2011-12-5
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    //[Microsoft.Security.Application.SecurityRuntimeEngine.SupressAntiXssEncoding()] //防XSS跨站脚本攻击
    public class UIBase : System.Web.UI.Page
    {

        #region 页面属性
        /// <summary>
        /// 登录状态
        /// </summary>
        public bool IsLogin
        {
            get;
            private set;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否为管理员
        /// </summary>
        public bool IsAdmin
        {
            get;
            private set;
        }
        #endregion

        #region gridview显示title的方法

        /// <summary>
        ///  gridview显示title的方法
        /// </summary>
        /// Copyright (c) 
        /// 创 建 人：王好(hao-wang@hnair.net)
        /// 创建日期：2012年2月1日
        /// 修 改 人：王好
        /// 修改日期：
        /// 版 本: 
        /// <param name="gv"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static string CreateTitle(GridView gv, bool flag = false)
        {
            if (!flag)
            {
                return "没有找到相关的信息";
            }
            else
            {
                StringBuilder strHtmlStream = new StringBuilder("");

                //根据gv创建title
                string tb = string.Format(@"<table class='{0}' cellspacing='{1}'  border='0'  style='border-collapse:collapse;'>"
                                                     , "gridviewEmptyTableStyle", gv.CellSpacing);
                strHtmlStream.Append(tb + "<tr class='" + gv.HeaderStyle.CssClass + "'>");
                for (int index = 0; index < gv.Columns.Count; index++)
                {
                    strHtmlStream.Append("<td >");
                    strHtmlStream.Append(gv.Columns[index].HeaderText.ToString());//文字
                    strHtmlStream.Append("</td>");
                }
                //结尾
                strHtmlStream.Append(string.Format("</tr><tr><td colspan='{0}'>没有找到相关的信息</td></tr></table>", gv.Columns.Count));
                // 返回字符串脚本



                return strHtmlStream.ToString();
            }






        }
        #endregion

        #region 获取对象的新值
        /// <summary>
        /// 获取对象的新值
        /// </summary>
        /// Copyright (c) 
        /// 创 建 人：王好(hao-wang@hnair.net)
        /// 创建日期：2012年2月1日
        /// 修 改 人：王好
        /// 修改日期：
        /// 版 本:
        /// <param name="obj"></param>
        /// <param name="controlsPrefix"></param>
        /// <returns></returns>
        public object GetObjValueByTextBox(object obj, string controlsPrefix)
        {
            Type entityType = obj.GetType();
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                TextBox txb = this.FindControl(controlsPrefix + info.Name) as TextBox;
                if (txb != null && txb.Text.Trim() == "")
                {
                    object value = null;
                    value = info.GetValue(obj, null);
                    if (value != null)
                    {
                        info.SetValue(obj, "", null);
                    }
                }

                else if (txb != null && txb.Text.Trim() != "")//is textbox and is not empty
                {
                    object value = GetPropertyInfoTypeValue(info, txb.Text);//根据不同类型调用转换

                    //给对象赋值
                    if (value != null)//防止默认值不为null的，给置为null
                    {
                        info.SetValue(obj, value, null);
                    }
                }
            }
            return obj;
        }
        #endregion

        #region 根据PropertyInfoType 动态转换strValue
        /// <summary>
        /// 根据PropertyInfoType 动态转换strValue
        /// </summary>
        /// <param name="info"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public object GetPropertyInfoTypeValue(PropertyInfo info, string strValue)
        {
            object value = null;

            //start 判断对象类型
            if (info.PropertyType.Equals(typeof(string)))
            {
                value = ToType(typeof(string), strValue);
            }
            else if (info.PropertyType.Equals(typeof(int)) || info.PropertyType.Equals(typeof(int?)))
            {
                value = ToType(typeof(int), strValue);
            }
            else if (info.PropertyType.Equals(typeof(decimal)) || info.PropertyType.Equals(typeof(decimal?)))
            {
                value = ToType(typeof(decimal), strValue);
            }
            else if (info.PropertyType.Equals(typeof(DateTime)) || info.PropertyType.Equals(typeof(DateTime?)))
            {
                value = ToType(typeof(DateTime), strValue);
            }
            else if (info.PropertyType.Equals(typeof(double)) || info.PropertyType.Equals(typeof(double?)))
            {
                value = ToType(typeof(double), strValue);
            }
            else if (info.PropertyType.Equals(typeof(bool)) || info.PropertyType.Equals(typeof(bool?)))
            {
                value = ToType(typeof(bool), strValue);
            }
            else if (info.PropertyType.Equals(typeof(Guid)) || info.PropertyType.Equals(typeof(Guid?)))
            {
                value = ToType(typeof(Guid), strValue);
            }
            return value;

        }
        #endregion

        #region 将字符串动态转换为指定的值类型
        /// <summary>
        /// 将字符串动态转换为指定的值类型 
        /// </summary>
        /// Copyright (c) 
        /// 创 建 人：王好(hao-wang@hnair.net)
        /// 创建日期：2012年2月1日
        /// 修 改 人：王好
        /// 修改日期：
        /// 版 本:
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object ToType(Type type, string value)
        {
            if (type == typeof(string))
            {
                return value;
            }

            MethodInfo parseMethod = null;


            // 查找带2个参数的TryParse函数
            foreach (MethodInfo mi in type.GetMethods(BindingFlags.Static
                | BindingFlags.Public))
            {
                if (mi.Name == "TryParse" && mi.GetParameters().Length == 2)
                {
                    parseMethod = mi;
                    break;
                }
            }

            if (parseMethod == null)
            {
                throw new ArgumentException(string.Format(
                    "Type: {0} has not Parse static method!", type));
            }

            //反射需要用到的参数
            object[] parameters = new object[] { value, Activator.CreateInstance(type) };

            object result = parseMethod.Invoke(null, parameters);
            if (Convert.ToBoolean(result))
            {
                return parameters[1];//第二个参数是返回的结果
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region 给textbox赋值
        /// <summary>
        /// 给textbox赋值
        /// </summary>
        /// Copyright (c) 
        /// 创 建 人：王好(hao-wang@hnair.net)
        /// 创建日期：2012年2月1日
        /// 修 改 人：王好
        /// 修改日期：
        /// 版 本: 
        /// <param name="obj"></param>
        /// <param name="controlsPrefix"></param>
        /// <returns></returns>
        public object SetTxbValue(object obj, string controlsPrefix)
        {
            Type entityType = obj.GetType();
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                TextBox txb = this.FindControl(controlsPrefix + info.Name) as TextBox;
                if (txb != null)
                {
                    txb.Text = "";
                }
                if (txb != null && info.GetValue(obj, null) != null)
                {
                    //时间类型特殊处理
                    if (info.PropertyType.Equals(typeof(DateTime)) || info.PropertyType.Equals(typeof(DateTime?)))
                    {
                        txb.Text = ((DateTime)info.GetValue(obj, null)).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        //txb.Text = Convert.ToDateTime(info.GetValue(obj, null).ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else
                    {
                        txb.Text = info.GetValue(obj, null).ToString();
                    }
                }
            }
            return obj;
        }
        #endregion

        #region 根据gridview所选行给主键赋初值
        /// <summary>
        /// 根据gridview所选行给主键赋初值
        /// </summary>
        /// Copyright (c) 
        /// 创 建 人：王好(hao-wang@hnair.net)
        /// 创建日期：2012年2月1日
        /// 修 改 人：王好
        /// 修改日期：
        /// 版 本: 
        /// <param name="obj">对象</param>
        /// <param name="gv">gridvew</param>
        /// <param name="rowIndex">所选行</param>
        /// <param name="enabledctlPrefix">是否需要对主键不可更改，默认为空不可更改</param>
        /// <returns></returns>
        public object GetPrimaryKeyValue(object obj, GridView gv, int rowIndex, string enabledctlPrefix = "")
        {
            Type entityType = obj.GetType();
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(typeof(ColumnAttribute), true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;
                    //是主键，并且gv里设置了DataKeys
                    if (objectAttr.IsPrimaryKey && gv.DataKeys[rowIndex].Values[objectAttr.Name] != null)
                    {
                        object value = null;

                        //时间类型特殊处理
                        if (info.PropertyType.Equals(typeof(DateTime)) || info.PropertyType.Equals(typeof(DateTime?)))
                        {
                            value = (DateTime)gv.DataKeys[rowIndex].Values[objectAttr.Name];
                        }
                        else
                        {
                            value = GetPropertyInfoTypeValue(info, gv.DataKeys[rowIndex].Values[objectAttr.Name].ToString());//根据不同类型调用转换
                        }

                        info.SetValue(obj, value, null);//绑定主键值


                        //控件前缀不为空，则讲其置为不可修改
                        if (enabledctlPrefix != "")
                        {
                            TextBox txb = this.FindControl(enabledctlPrefix + info.Name) as TextBox;
                            if (txb != null)
                            {
                                txb.Enabled = false;
                            }

                        }
                    }

                }

            }
            return obj;
        }
        #endregion

        #region 翻页自定义模板
        /// <summary>
        /// Bind Pager CustomInfoHtml
        /// </summary>
        /// Copyright (c) 
        /// 创 建 人：王好(hao-wang@hnair.net)
        /// 创建日期：2012年2月1日
        /// 修 改 人：王好
        /// 修改日期：
        /// 版 本:
        /// <param name="aspNetPager"></param>
        public  void BindPagerCustomInfoHTML(Wuqi.Webdiyer.AspNetPager aspNetPager)
        {
            aspNetPager.CustomInfoHTML = "记录总数：<font color=\"blue\"><b>" + aspNetPager.RecordCount.ToString() + "</b></font>";
            aspNetPager.CustomInfoHTML += " 总页数：<font color=\"blue\"><b>" + aspNetPager.PageCount.ToString() + "</b></font>";
            aspNetPager.CustomInfoHTML += " 当前页：<font color=\"red\"><b>" + aspNetPager.CurrentPageIndex.ToString() + "</b></font>";
        }
        #endregion

        #region 鼠标悬停高亮显示

        /// <summary>
        /// 鼠标悬停高亮显示
        /// </summary>
        /// Copyright (c) 
        /// 创 建 人：王好(hao-wang@hnair.net)
        /// 创建日期：2012年2月1日
        /// 修 改 人：王好
        /// 修改日期：
        /// 版 本: 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GridViewRowsChangeColor(object sender, GridViewRowEventArgs e)
        {
            //鼠标移动变色  
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //鼠标移动变色
                e.Row.Attributes.Add("onmouseover", "currentcolor=this.style.backgroundColor;this.style.backgroundColor='FFF5D2';");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=currentcolor;");
            }

        } 
        #endregion

        #region 注册脚本
        /// <summary>
        /// 注册脚本
        /// </summary>
        /// <param name="script"></param>
        /// <param name="key"></param>
        public void RegisterScript(string script, string key = "LoadScript")
        {
            //显示div
            ClientScript.RegisterStartupScript(this.GetType(),key,
                "<script language='javascript'>"+script+"</script>");
        }
        #endregion

        #region 获取网站根目录
        /// <summary>
        /// 获取网站根目录
        /// </summary>
        /// Copyright   
        /// 创 建 人：王好(hao-wang@hnair.net)
        /// 创建日期：2012年5月28日
        /// 修 改 人：王好
        /// 修改日期：
        /// 版 本:      
        public string WebPath
        {
            get
            {
                string path = Request.ApplicationPath == "" ? "/" : Request.ApplicationPath;
                return path;
            }
        }
        #endregion
    }
}