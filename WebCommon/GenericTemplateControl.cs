using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HNAS.Framework4.WebCommon
{
    /// <summary>
    /// 通用控件模板
    /// </summary>
    /// Copyright (c) 2012 海航航空信息系统有限公司
    /// 创 建 人：王宇
    /// 创建日期：2012-2-20
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    public class GenericTemplateControl : WebControl, INamingContainer
    {
        /// <summary>
        /// 
        /// </summary>
        protected virtual void AttachChildControl()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            ArgumentAssertion.StringIsNotEmpty("Skin", this.Skin);
            Control child = this.Page.LoadControl(this.Skin);
            this.Controls.Add(child);
            this.AttachChildControl();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void DataBind()
        {
            this.EnsureChildControls();
            base.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Control FindControl(string id)
        {
            Control control = base.FindControl(id);
            if ((control == null) && (this.Controls.Count == 1))
            {
                control = this.Controls[0].FindControl(id);
            }
            return control;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (base.DesignMode)
            {
                this.EnsureChildControls();
            }
            base.Render(writer);
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ClientID
        {
            get
            {
                if (this.Controls.Count > 0)
                {
                    return this.Controls[0].ClientID;
                }
                return base.ClientID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override ControlCollection Controls
        {
            get
            {
                this.EnsureChildControls();
                return base.Controls;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Category("Appearance"), Description("Path to the control skin"), DefaultValue(""), Bindable(true)]
        public string Skin
        {
            get
            {
                if (this.ViewState[this.ID + ".Skin"] == null)
                {
                    return string.Empty;
                }
                return (this.ViewState[this.ID + ".Skin"] as string);
            }
            set
            {
                this.ViewState[this.ID + ".Skin"] = value;
            }
        }

    }
}
