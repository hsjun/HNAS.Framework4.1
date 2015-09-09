using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

using HNAS.Framework4.Data;
using HNAS.Framework4.Validation.Validators;
using HNAS.Framework4.Logging;

namespace HNAS.Framework4.BLBase
{
    /// <summary>
    /// BusinessLogic层通用实现类
    /// </summary>
    /// Copyright (c) 2011 海航航空信息系统有限公司
    /// 创 建 人：HNAS .Net Framework 4.0 项目组
    /// 创建日期：2011-12-5
    /// 修 改 人：王宇
    /// 修改日期：2012-02-24
    /// 版 本：1.0
    public class CommonBL
    {

        /// <summary>
        /// 数据访问实例
        /// </summary>
        private readonly CommonDA commonDA;

        /// <summary>
        /// 操作人
        /// </summary>
        private static String Account = "";

        /// <summary>
        /// 异常策略名称（Web.config配置）
        /// </summary>
        private const String strPolicy = "PolicyOnBusinessLogicLayer";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strDBName">数据库连接配置名</param>
        /// <param name="strAccount">当前用户（操作人）</param>
        /// <param name="dataAccess">数据访问接口</param>
        public CommonBL(String strDBName = "", String strAccount = "Admin", DataAccess dataAccess = null)
        {
            commonDA = new CommonDA(strDBName, dataAccess);
            Account = strAccount;
        }

        #region 精确查询

        #region 获取数据返回第一个
        /// <summary>
        /// 获取数据返回第一个
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<T> GetOne<T>(T model, bool bValidate = false)
            where T : BaseClass<T>, new()
        {
            CallResult<T> result = new CallResult<T>();
            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetOne(model);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回List
        /// <summary>
        /// 获取数据返回List
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">是否启用缓存，默认禁用</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>eg:
        /// <code>
        /// 
        ///   //获取有效数据
        ///   StudentBM studentBM = new StudentBM()
        ///   {
        ///       Valid = 1,
        ///       Name = strName
        ///   };
        ///
        ///   //支持自定义排序，验证开关（查询默认不验证实体对象），请确保列名的合法性
        ///   string strOrderBy = "cniID, cniClassID ASC";
        ///   //列过滤（默认返回所有列）
        ///   string strColumnFilter = "cniID,cniClassID,cnvcSID,cnvcName,cniSex,cndBirthday,cnvcEmail";
        ///   //行过滤
        ///   string strRowFilter = "";
        ///   //参数化
        ///   List&lt;DbParameter> parameters = new List&lt;DbParameter>();
        ///   if (!string.IsNullOrEmpty(txbMin.Text.Trim()))
        ///   {
        ///       strRowFilter += " AND cniID>=@MinID";
        ///       DbParameter parameter1 = new SqlParameter("MinID", Convert.ToInt32(txbMin.Text.Trim()));
        ///       parameters.Add(parameter1);
        ///   }
        ///   if (!string.IsNullOrEmpty(txbMax.Text.Trim()))
        ///   {
        ///       strRowFilter += " AND cniID&lt;=@MaxID";
        ///       DbParameter parameter2 = new SqlParameter("MaxID", Convert.ToInt32(txbMax.Text.Trim()));
        ///       parameters.Add(parameter2);
        ///   }
        ///  
        ///   //调用本方法获取数据
        ///   CallResult&lt;IList&lt;StudentBM>> cr = GetList(studentBM, false, strCacheKey: strCacheName, strOrderBy: strOrderBy, strColumnFilter:
        ///   strColumnFilter, strRowFilter: strRowFilter, parameters: parameters, bValidate: true);
        /// 
        ///   if (cr.HasError)//操作异常则HasError为true
        ///   {
        ///       return null;
        ///   }        
        /// </code>
        /// 返回数据为<c>cr.Result</c>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<IList<T>> GetList<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<IList<T>> result = new CallResult<IList<T>>();

            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetList(model, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回DataSet
        /// <summary>
        /// 获取数据返回DataSet
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<DataSet> GetDataSet<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<DataSet> result = new CallResult<DataSet>();

            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetDataSet(model, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回DataReader
        /// <summary>
        /// 获取数据返回DataReader
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<IDataReader> GetDataReader<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<IDataReader> result = new CallResult<IDataReader>();

            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetDataReader(model, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回DataTable
        /// <summary>
        /// 获取数据返回DataTable
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<DataTable> GetDataTable<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<DataTable> result = new CallResult<DataTable>();

            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetDataTable(model, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                result.Error = ex;
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回List（支持分页）
        /// <summary>
        /// 获取数据返回List（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">是否启用缓存，默认禁用</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>eg:
        /// <code>
        /// 
        ///   //获取有效数据
        ///   StudentBM studentBM = new StudentBM()
        ///   {
        ///       Valid = 1,
        ///       studentBM.Name = strName
        ///   };
        ///
        ///   //支持自定义排序，验证开关（查询默认不验证实体对象），请确保列名的合法性
        ///   string strOrderBy = "cniID, cniClassID ASC";
        ///   //列过滤（默认返回所有列）
        ///   string strColumnFilter = "cniID,cniClassID,cnvcSID,cnvcName,cniSex,cndBirthday,cnvcEmail";
        ///   //分页设置
        ///   Pager pager = new Pager()
        ///   {
        ///       PageSize = AspNetPager1.PageSize,
        ///       PageIndex = AspNetPager1.CurrentPageIndex
        ///   };
        ///   
        ///   //调用本方法获取数据
        ///   CallResult&lt;IList&lt;StudentBM>> cr = new CommonBL().GetList(studentBM, ref pager);//数据库分页默认（UI层调用）
        ///   CallResult&lt;IList&lt;StudentBM>> cr = GetList(studentBM, ref pager, false, strCacheKey: strCacheName, strOrderBy: strOrderBy, 
        ///   strColumnFilter: strColumnFilter, bValidate: true);
        /// 
        ///   if (cr.HasError)//操作异常则HasError为true
        ///   {
        ///       return null;
        ///   }        
        /// </code>
        /// 返回数据为<c>cr.Result</c>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<IList<T>> GetList<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<IList<T>> result = new CallResult<IList<T>>();
            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetList(model, ref pager, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回DataSet（支持分页）
        /// <summary>
        /// 获取数据返回DataSet（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<DataSet> GetDataSet<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false)
            where T : BaseClass<T>, new()
        {
            CallResult<DataSet> result = new CallResult<DataSet>();
            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetDataSet(model, ref pager, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回DataReader（支持分页）
        /// <summary>
        /// 获取数据返回DataReader（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<IDataReader> GetDataReader<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<IDataReader> result = new CallResult<IDataReader>();
            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetDataReader(model, ref pager, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回DataTable（支持分页）
        /// <summary>
        /// 获取数据返回DataTable（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<DataTable> GetDataTable<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<DataTable> result = new CallResult<DataTable>();
            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetDataTable(model, ref pager, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                result.Error = ex;
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #endregion

        #region 模糊查询

        #region 获取数据返回第一个
        /// <summary>
        /// 模糊查询，获取数据返回第一个
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<T> GetOneByLike<T>(T model, bool bValidate = false)
            where T : BaseClass<T>, new()
        {
            CallResult<T> result = new CallResult<T>();
            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;

                    return result;
                }

                result.Result = commonDA.GetOneByLike(model);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回List
        /// <summary>
        /// 模糊查询，获取数据返回List
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<IList<T>> GetListByLike<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<IList<T>> result = new CallResult<IList<T>>();

            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetListByLike(model, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回DataSet
        /// <summary>
        /// 模糊查询，获取数据返回DataSet
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<DataSet> GetDataSetByLike<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<DataSet> result = new CallResult<DataSet>();

            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetDataSetByLike(model, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回DataReader
        /// <summary>
        /// 模糊查询，获取数据返回DataReader
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<IDataReader> GetDataReaderByLike<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<IDataReader> result = new CallResult<IDataReader>();

            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetDataReaderByLike(model, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回DataTable
        /// <summary>
        /// 模糊查询，获取数据返回DataTable
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<DataTable> GetDataTableByLike<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<DataTable> result = new CallResult<DataTable>();

            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetDataTableByLike(model, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回List（支持分页）
        /// <summary>
        /// 模糊查询，获取数据返回List（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<IList<T>> GetListByLike<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<IList<T>> result = new CallResult<IList<T>>();
            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetListByLike(model, ref pager, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回DataSet（支持分页）
        /// <summary>
        /// 模糊查询，获取数据返回DataSet（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<DataSet> GetDataSetByLike<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<DataSet> result = new CallResult<DataSet>();
            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetDataSetByLike(model, ref pager, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回DataReader（支持分页）
        /// <summary>
        /// 模糊查询，获取数据返回DataReader（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<IDataReader> GetDataReaderByLike<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<IDataReader> result = new CallResult<IDataReader>();
            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetDataReaderByLike(model, ref pager, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 获取数据返回DataTable（支持分页）
        /// <summary>
        /// 模糊查询，获取数据返回DataTable（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存</param>
        /// <param name="strCacheKey">缓存名称</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考GetList方法示例。<seealso cref="GetList{T}"/>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<DataTable> GetDataTableByLike<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<DataTable> result = new CallResult<DataTable>();
            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.GetDataTableByLike(model, ref pager, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters);
                result.Message = Message.Operate_Success;
            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Operate_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #endregion

        #region 添加数据
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bLogging">是否记录日志，默认记录</param>
        /// <param name="bValidate">是否验证实体，默认验证</param>
        /// <returns>操作结果集</returns>
        /// <example>eg:
        /// <code>
        /// SubjectBM subjectBM = new SubjectBM()
        /// {
        ///     ID = Guid.NewGuid(),
        ///     Name = txbName.Text.Trim(),
        ///     Score = dScore
        /// };
        ///
        /// //存入数据库
        /// CallResult&lt;string> cr = commonBL.Add(subjectBM);
        /// if (cr.HasError)
        /// {
        ///     AlertMsn.PopMsn(cr.Message);
        /// }
        /// </code>
        /// 返回数据为<c>cr.Result</c>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<String> Add<T>(T model, bool bLogging = true, bool bValidate = true) where T : BaseClass<T>, new()
        {
            CallResult<String> result = new CallResult<String>();

            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    //result.Result = null;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.Add(model);
                result.Message = Message.Add_Success;

                if (bLogging)
                {
                    //日志信息
                    String strLog = String.Format("{0}新增{1}表一条记录。", Account, commonDA.GetTableName(model));
                    //写入日志
                    WriteLogToDB.WriteLog(strLog, Account);
                }

            }
            catch (Exception ex)
            {
                //result.Result = null;
                result.Message = Message.Add_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 批量添加数据

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="modelList">数据实体集合</param>
        /// <param name="bLogging">是否记录日志，默认记录</param>
        /// <param name="bValidate">是否验证实体，默认验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考Add方法示例。<seealso>
        ///             <cref>Add{T}</cref>
        ///           </seealso>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<bool> Add<T>(IList<T> modelList, bool bLogging = true, bool bValidate = true) where T : BaseClass<T>, new()
        {
            CallResult<bool> result = new CallResult<bool>();

            try
            {
                //验证
                if (modelList == null || modelList.Count == 0)
                {
                    result.Result = false;
                    result.Message = Message.Validate_Unsuccess;
                    return result;
                }
                foreach (T model in modelList)
                {
                    if (bValidate && !model.IsValid())
                    {
                        result.Error = new Exception(Message.Validate_Unsuccess);
                        result.Result = false;
                        result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                        return result;
                    }
                }

                result.Result = commonDA.Add(modelList);
                result.Message = Message.Add_Success;

                if (bLogging && result.Result)
                {
                    //日志信息
                    String strLog = String.Format("{0}新增{1}表{2}条记录。", Account, commonDA.GetTableName(modelList[0]), modelList.Count);
                    //写入日志
                    WriteLogToDB.WriteLog(strLog, Account);
                }

            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = Message.Add_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 更新数据
        /// <summary>
        /// 更新数据（默认更新条件为主键）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="conditionParameters">条件参数，参数名称请与数据库列名一致</param>
        /// <param name="bLogging">是否记录日志，默认记录</param>
        /// <param name="bValidate">是否验证实体，默认验证</param>
        /// <returns>操作结果集</returns>
        /// <example>eg:
        /// <code>
        /// SubjectBM subjectBm = new SubjectBM();
        /// subjectBm.Name = txbName.Text.Trim();
        ///
        /// //自定义条件参数
        /// DbParameter[] parameter = new DbParameter[]
        ///                               {
        ///                                   new SqlParameter("cnfScore", SqlDbType.Float)
        ///                               };
        /// parameter[0].Value = txbScore.Text.Trim();
        ///
        /// CallResult&lt;bool> cr = new CommonBL().Update(subjectBm, parameter);
        /// if (!cr.HasError)
        /// {
        ///     //更新成功重新绑定数据
        ///     gvSubject.DataBind();
        /// }
        /// </code>
        /// 返回数据为<c>cr.Result</c>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<bool> Update<T>(T model, DbParameter[] conditionParameters = null, bool bLogging = true, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<bool> result = new CallResult<bool>();

            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    result.Result = false;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }
                else if (bValidate && conditionParameters == null)//更新单独验证，无自定义条件时主键不能为空
                {
                    foreach (var item in commonDA.GetPrimaryKey(model))
                    {
                        if (item.Value == null)
                        {
                            result.Error = new Exception(Message.Validate_Unsuccess);
                            result.Result = false;
                            result.Message = Message.Validate_Unsuccess + "，主键值不能为空！";
                            return result;
                        }
                    }
                }

                result.Result = commonDA.Update(model, conditionParameters);
                if (!result.Result)
                {
                    result.Message = Message.Update_Unsuccess;
                }
                else
                {
                    result.Message = Message.Update_Success;

                    if (bLogging && result.Result)
                    {
                        //日志信息
                        String strLog = String.Format("{0}更新{1}表一条记录。", Account, commonDA.GetTableName(model));
                        //写入日志
                        WriteLogToDB.WriteLog(strLog, Account);
                    }
                }

            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = Message.Update_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 批量更新数据
        /// <summary>
        /// 批量更新数据（默认更新条件为主键）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="modelList">数据实体集合</param>
        /// <param name="bLogging">是否记录日志，默认记录</param>
        /// <param name="bValidate">是否验证实体，默认验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考Update方法示例。<seealso>
        ///             <cref>Update{T}</cref>
        ///           </seealso>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<bool> UpdateList<T>(IList<T> modelList, bool bLogging = true, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<bool> result = new CallResult<bool>();

            try
            {
                //验证
                if (modelList == null || modelList.Count == 0)
                {
                    result.Result = false;
                    result.Message = Message.Validate_Unsuccess;
                    return result;
                }
                foreach (T model in modelList)
                {
                    if (bValidate && !model.IsValid())
                    {
                        result.Error = new Exception(Message.Validate_Unsuccess);
                        result.Result = false;
                        result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                        return result;
                    }
                    else if (bValidate)//更新单独验证，主键不能为空
                    {
                        foreach (var item in commonDA.GetPrimaryKey(model))
                        {
                            if (item.Value == null)
                            {
                                result.Error = new Exception(Message.Validate_Unsuccess);
                                result.Result = false;
                                result.Message = Message.Validate_Unsuccess + "，主键值不能为空！";
                                return result;
                            }
                        }
                    }
                }

                result.Result = commonDA.Update(modelList);
                result.Message = Message.Update_Success;
                if (bLogging && result.Result)
                {
                    //日志信息
                    String strLog = String.Format("{0}更新{1}表{2}条记录。", Account, commonDA.GetTableName(modelList[0]), modelList.Count);
                    //写入日志
                    WriteLogToDB.WriteLog(strLog, Account);
                }

            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = Message.Update_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 批量更新数据（自定义条件更新）
        /// <summary>
        /// 批量更新数据（自定义条件更新）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="dicModel_Parameter">数据实体、更新条件键值集合</param>
        /// <param name="bLogging">是否记录日志，默认记录</param>
        /// <param name="bValidate">是否验证实体，默认验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考Update方法示例。<seealso>
        ///             <cref>Update{T}</cref>
        ///           </seealso>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<bool> UpdateList<T>(Dictionary<T, DbParameter[]> dicModel_Parameter, bool bLogging = true, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<bool> result = new CallResult<bool>();

            try
            {
                //验证
                if (dicModel_Parameter == null || dicModel_Parameter.Count == 0)
                {
                    result.Result = false;
                    result.Message = Message.Validate_Unsuccess;
                    return result;
                }
                foreach (KeyValuePair<T, DbParameter[]> model_Parameter in dicModel_Parameter)
                {
                    if (bValidate && !model_Parameter.Key.IsValid())
                    {
                        result.Error = new Exception(Message.Validate_Unsuccess);
                        result.Result = false;
                        result.Message = Message.Validate_Unsuccess + model_Parameter.Key.ValidateTag;
                        return result;
                    }
                }

                result.Result = commonDA.Update(dicModel_Parameter);
                result.Message = Message.Update_Success;
                if (bLogging && result.Result)
                {
                    //日志信息
                    //TODO 获取表名性能
                    String strLog = String.Format("{0}更新{1}表{2}条记录。", Account, commonDA.GetTableName(dicModel_Parameter.Keys.GetEnumerator().Current), dicModel_Parameter.Count);
                    //写入日志
                    WriteLogToDB.WriteLog(strLog, Account);
                }

            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = Message.Update_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除数据（默认删除条件为主键）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="conditionParameters">条件参数，参数名称请与数据库列名一致</param>
        /// <param name="bLogging">是否记录日志，默认记录</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>eg:
        /// <code>
        /// //从数据库中删除
        /// CallResult&lt;bool> cr = new CommonBL().Delete(new SubjectBM
        /// {
        ///     ID = Guid.Parse(lb.CommandName)
        /// });
        ///
        /// if (!cr.HasError || cr.Result)
        /// {
        ///     //删除成功重新绑定数据
        ///     gvSubject.DataBind();
        /// }        
        /// 
        /// </code>
        /// 返回数据为<c>cr.Result</c><br/>
        /// 自定义条件删除参考Update方法示例。<seealso>
        ///             <cref>Update{T}</cref>
        ///           </seealso>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<bool> Delete<T>(T model, DbParameter[] conditionParameters = null, bool bLogging = true, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<bool> result = new CallResult<bool>();

            try
            {
                //验证
                if (bValidate && !model.IsValid())
                {
                    result.Error = new Exception(Message.Validate_Unsuccess);
                    result.Result = false;
                    result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                    return result;
                }

                result.Result = commonDA.Delete(model, conditionParameters);
                if (!result.Result)
                {
                    result.Message = Message.Del_Unsuccess;
                }
                else
                {
                    result.Message = Message.Del_Success;
                    if (bLogging && result.Result)
                    {
                        //日志信息
                        String strLog = String.Format("{0}删除{1}表一条记录。", Account, commonDA.GetTableName(model));
                        //写入日志
                        WriteLogToDB.WriteLog(strLog, Account);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = Message.Del_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 批量删除数据
        /// <summary>
        /// 批量删除数据（默认删除条件为主键）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="modelList">数据实体集合</param>
        /// <param name="bLogging">是否记录日志，默认记录</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考Delete方法示例。<seealso>
        ///             <cref>Delete{T}</cref>
        ///           </seealso>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<bool> Delete<T>(IList<T> modelList, bool bLogging = true, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<bool> result = new CallResult<bool>();

            try
            {
                //验证
                if (modelList == null || modelList.Count == 0)
                {
                    result.Result = false;
                    result.Message = Message.Validate_Unsuccess;
                    return result;
                }
                foreach (T model in modelList)
                {
                    if (bValidate && !model.IsValid())
                    {
                        result.Error = new Exception(Message.Validate_Unsuccess);
                        result.Result = false;
                        result.Message = Message.Validate_Unsuccess + model.ValidateTag;
                        return result;
                    }
                }

                result.Result = commonDA.Delete(modelList);
                if (!result.Result)
                {
                    result.Message = Message.Del_Unsuccess;
                }
                else
                {
                    result.Message = Message.Del_Success;
                    if (bLogging && result.Result)
                    {
                        //日志信息
                        String strLog = String.Format("{0}删除{1}表{2}条记录。", Account, commonDA.GetTableName(modelList[0]),
                                                      modelList.Count);
                        //写入日志
                        WriteLogToDB.WriteLog(strLog, Account);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = Message.Del_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 批量删除数据（自定义条件删除）
        /// <summary>
        /// 批量删除数据（自定义条件删除）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="dicModel_Parameter">数据实体、更新条件键值集合</param>
        /// <param name="bLogging">是否记录日志，默认记录</param>
        /// <param name="bValidate">是否验证实体，默认不验证</param>
        /// <returns>操作结果集</returns>
        /// <example>
        /// 参考Delete方法示例。<seealso>
        ///             <cref>Delete{T}</cref>
        ///           </seealso>
        /// </example>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public CallResult<bool> Delete<T>(Dictionary<T, DbParameter[]> dicModel_Parameter, bool bLogging = true, bool bValidate = false) where T : BaseClass<T>, new()
        {
            CallResult<bool> result = new CallResult<bool>();

            try
            {
                //验证
                if (dicModel_Parameter == null || dicModel_Parameter.Count == 0)
                {
                    result.Result = false;
                    result.Message = Message.Validate_Unsuccess;
                    return result;
                }
                foreach (KeyValuePair<T, DbParameter[]> model_Parameter in dicModel_Parameter)
                {
                    if (bValidate && !model_Parameter.Key.IsValid())
                    {
                        result.Error = new Exception(Message.Validate_Unsuccess);
                        result.Result = false;
                        result.Message = Message.Validate_Unsuccess + model_Parameter.Key.ValidateTag;
                        return result;
                    }
                }

                result.Result = commonDA.Delete(dicModel_Parameter);
                if (!result.Result)
                {
                    result.Message = Message.Del_Unsuccess;
                }
                else
                {
                    result.Message = Message.Del_Success;
                    if (bLogging && result.Result)
                    {
                        //日志信息
                        String strLog = String.Format("{0}删除{1}表{2}条记录。", Account,
                                                      commonDA.GetTableName(dicModel_Parameter.Keys.GetEnumerator().Current),
                                                      dicModel_Parameter.Count);
                        //写入日志
                        WriteLogToDB.WriteLog(strLog, Account);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = Message.Del_Unsuccess;
                result.Error = ex;

                //处理异常
                HandleException(ex);
            }

            return result;
        }
        #endregion

        #region 处理异常
        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="bLogging">是否记录日志，默认记录</param>
        public void HandleException(Exception ex, bool bLogging = true)
        {
            bool rethrow = ExceptionPolicy.HandleException(ex, strPolicy);
            if (rethrow)
            {
                if (bLogging)
                {
                    //异常写入日志
                    WriteLogToDB.WriteLog(ex.Message, Account);
                }
                throw ex;
            }
        }
        #endregion
    }
}