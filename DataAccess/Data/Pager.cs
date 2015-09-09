using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HNAS.Framework4.Data
{
    #region 分页类
    /// <summary>
    /// 分页类
    /// </summary>
    ///  创 建 人：余鹏飞
    ///  创建日期：2011年12月14日
    ///  修 改 人：王宇
    ///  修改日期：2012年2月24日
    ///  Copyright (c) 2012 海南海航航空信息系统有限公司
    ///  版 本：1.0	
    public class Pager
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
    }
    #endregion
}
