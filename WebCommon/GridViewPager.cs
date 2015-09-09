using System;
using System.ComponentModel;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HNAS.Framework4.WebCommon
{
    /// <summary>
    /// Gridview分页控件
    /// </summary>
    /// Copyright (c) 2012 海航航空信息系统有限公司
    /// 创 建 人：王宇
    /// 创建日期：2012-2-20
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    [DefaultProperty("ViewID"), ToolboxData("<{0}:GridViewPager runat=\"server\" Skin=\"\" ViewID=\"\"></{0}:GridViewPager>")]
    public class GridViewPager : GenericTemplateControl
    {
        // Fields
        private ITextControl _currentIndexHolder;
        private IButtonControl _firstPageLink;
        private IButtonControl _lastPageLink;
        private IButtonControl _nextPageLink;
        private ITextControl _pageCountHolder;
        private ListControl _pageDDL;
        private ITextControl _pageItemCountHolder;
        private IButtonControl _prevPageLink;
        private ITextControl _totalCountHolder;
        private GridView _viewControl;

        /// <summary>
        /// 
        /// </summary>
        public const String CurrentIndexHolderID = "currentIndexHolder";
        /// <summary>
        /// 
        /// </summary>
        public const String EventPageIndexChanging = "PageIndexChanging";
        /// <summary>
        /// 
        /// </summary>
        public const String FirstPageLinkID = "firstPageLink";
        /// <summary>
        /// 
        /// </summary>
        public const String LastPageLinkID = "lastPageLink";
        /// <summary>
        /// 
        /// </summary>
        public const String NextPageLinkID = "nextPageLink";
        /// <summary>
        /// 
        /// </summary>
        public const String PageCountHolderID = "pageCountHolder";
        /// <summary>
        /// 
        /// </summary>
        public const String PageItemCountID = "pageItemCount";
        /// <summary>
        /// 
        /// </summary>
        public const String PageSelectorID = "pageSellector";
        /// <summary>
        /// 
        /// </summary>
        public const String PrevPageLinkID = "prevPageLink";
        /// <summary>
        /// 
        /// </summary>
        public const String TotalCountID = "totalCount";

        // Events

        /// <summary>
        /// 
        /// </summary>
        public event GridViewPageEventHandler PageIndexChanging
        {
            add
            {
                base.Events.AddHandler("PageIndexChanging", value);
            }
            remove
            {
                base.Events.RemoveHandler("PageIndexChanging", value);
            }
        }

        /// <summary>
        /// 增加子控件
        /// </summary>
        protected override void AttachChildControl()
        {
            base.AttachChildControl();
            ArgumentAssertion.StringIsNotEmpty(this.ViewID, "ViewID");
            this._viewControl = this.Parent.FindControl(this.ViewID) as GridView;
            ArgumentAssertion.IsNotNull("GridViewControl", this._viewControl, "Cannot find the attached gridview control");
            this._firstPageLink = this.FindControl("firstPageLink") as IButtonControl;
            this._lastPageLink = this.FindControl("lastPageLink") as IButtonControl;
            this._prevPageLink = this.FindControl("prevPageLink") as IButtonControl;
            this._nextPageLink = this.FindControl("nextPageLink") as IButtonControl;
            this._pageCountHolder = this.FindControl("pageCountHolder") as ITextControl;
            this._currentIndexHolder = this.FindControl("currentIndexHolder") as ITextControl;
            this._pageDDL = this.FindControl("pageSellector") as ListControl;
            this._totalCountHolder = this.FindControl("totalCount") as ITextControl;
            this._pageItemCountHolder = this.FindControl("pageItemCount") as ITextControl;
        }


        private void Check()
        {
            if (this.PageIndex >= this.PageCount)
            {
                this.ViewState["PageIndex"] = this.PageCount - 1;
                if (this.PageIndex < 0)
                {
                    this.ViewState["PageIndex"] = 0;
                }
            }
        }

        /// <summary>
        /// 数据绑定
        /// </summary>
        public override void DataBind()
        {
            LinkButton button;
            base.DataBind();
            this.EnsureChildControls();
            this._viewControl.PagerSettings.Visible = false;
            this._viewControl.DataSource = this.DataSource;
            this._viewControl.DataBind();
            this._totalCountHolder.Text = this.ItemsCount.ToString();
            this._pageItemCountHolder.Text = this.PageSize.ToString();
            int pageCount = this.PageCount;
            int pageIndex = this.PageIndex;
            if (this._pageCountHolder != null)
            {
                this._pageCountHolder.Text = pageCount.ToString();
            }
            if (this._currentIndexHolder != null)
            {
                this._currentIndexHolder.Text = (pageCount > 0) ? ((pageIndex + 1)).ToString() : "0";
            }
            if (this._prevPageLink != null)
            {
                button = this._prevPageLink as LinkButton;
                button.Enabled = (pageCount > 1) && (pageIndex > 0);
            }
            if (this._nextPageLink != null)
            {
                button = this._nextPageLink as LinkButton;
                button.Enabled = (pageCount > 1) && (pageIndex < (pageCount - 1));
            }
            if (this._pageDDL != null)
            {
                if (!this.Page.IsPostBack || (this._pageDDL.Items.Count != pageCount))
                {
                    int selectedIndex = this._pageDDL.SelectedIndex;
                    this._pageDDL.Items.Clear();
                    for (int i = 0; i < this.PageCount; i++)
                    {
                        String text = (i + 1).ToString();
                        this._pageDDL.Items.Add(new ListItem(text, text));
                    }
                    if (this._pageDDL.Items.Count > 0)
                    {
                        this._pageDDL.SelectedIndex = (selectedIndex < this._pageDDL.Items.Count) ? selectedIndex : 0;
                    }
                }
                this._pageDDL.Enabled = pageCount > 0;
            }
        }

        private void OnFirstPage(object sender, EventArgs e)
        {
            this.OnPageIndexChanging(0);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.EnsureChildControls();
            if (this._firstPageLink != null)
            {
                this._firstPageLink.Click += new EventHandler(this.OnFirstPage);
            }
            if (this._lastPageLink != null)
            {
                this._lastPageLink.Click += new EventHandler(this.OnLastPage);
            }
            if (this._prevPageLink != null)
            {
                this._prevPageLink.Click += new EventHandler(this.OnPreviousPage);
            }
            if (this._nextPageLink != null)
            {
                this._nextPageLink.Click += new EventHandler(this.OnNextPage);
            }
            if (this._pageDDL != null)
            {
                this._pageDDL.SelectedIndexChanged += new EventHandler(this.OnSelectPage);
            }
        }

        private void OnLastPage(object sender, EventArgs e)
        {
            if (this.PageCount > 0)
            {
                this.OnPageIndexChanging(this.PageCount - 1);
            }
        }

        private void OnNextPage(object sender, EventArgs e)
        {
            int newIndex = this.PageIndex + 1;
            if (this.PageCount > newIndex)
            {
                this.OnPageIndexChanging(newIndex);
            }
        }

        /// <summary>
        /// 翻页事件
        /// </summary>
        /// <param name="newIndex"></param>
        protected virtual void OnPageIndexChanging(int newIndex)
        {
            this._pageDDL.SelectedIndex = newIndex;
            GridViewPageEventHandler handler = base.Events["PageIndexChanging"] as GridViewPageEventHandler;
            if (handler != null)
            {
                GridViewPageEventArgs e = new GridViewPageEventArgs(newIndex);
                handler(this, e);
            }
        }

        private void OnPreviousPage(object sender, EventArgs e)
        {
            if (this.PageIndex > 0)
            {
                int newIndex = this.PageIndex - 1;
                this.OnPageIndexChanging(newIndex);
            }
        }

        private void OnSelectPage(object sender, EventArgs e)
        {
            this.EnsureChildControls();
            int selectedIndex = this._pageDDL.SelectedIndex;
            this.OnPageIndexChanging(selectedIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.ItemsCount > 0)
            {
                base.Render(writer);
            }
            else
            {
                StringWriter writer2 = new StringWriter();
                HtmlTextWriter writer3 = new HtmlTextWriter(writer2);
                base.Render(writer3);
                writer.Write("&nbsp;");
            }
        }

        // Properties
        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(""), Bindable(true), Themeable(false)]
        public object DataSource
        {
            get
            {
                this.EnsureChildControls();
                return this._viewControl.DataSource;
            }
            set
            {
                this.EnsureChildControls();
                this._viewControl.DataSource = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Bindable(false)]
        public int ItemsCount
        {
            get
            {
                if (this.ViewState["ItemsCount"] == null)
                {
                    return 0;
                }
                return Convert.ToInt32(this.ViewState["ItemsCount"]);
            }
            set
            {
                this.ViewState["ItemsCount"] = value;
                this.Check();
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [Bindable(false)]
        public int PageCount
        {
            get
            {
                int pageSize = this.PageSize;
                int itemsCount = this.ItemsCount;
                if (pageSize <= 0)
                {
                    return 0;
                }
                return (((itemsCount % this.PageSize) == 0) ? (itemsCount / pageSize) : ((itemsCount / pageSize) + 1));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true), DefaultValue(0)]
        public int PageIndex
        {
            get
            {
                if (this.ViewState["PageIndex"] == null)
                {
                    return 0;
                }
                return Convert.ToInt32(this.ViewState["PageIndex"]);
            }
            set
            {
                this.ViewState["PageIndex"] = value;
                this.Check();
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [DefaultValue(20)]
        public int PageSize
        {
            get
            {
                if (this.ViewState["PageSize"] == null)
                {
                    return 20;
                }
                return Convert.ToInt32(this.ViewState["PageSize"]);
            }
            set
            {
                this.ViewState["PageSize"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true), Bindable(true), Category("Data"), Description("The id of the attached gridview control")]
        public String ViewID
        {
            get
            {
                if (this.ViewState["ViewID"] == null)
                {
                    return String.Empty;
                }
                return (this.ViewState["ViewID"] as String);
            }
            set
            {
                this.ViewState["ViewID"] = value;
            }
        }

    }

}
