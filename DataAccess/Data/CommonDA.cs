using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using HNAS.Framework4.Caching;

namespace HNAS.Framework4.Data
{
    /// <summary>
    /// DataAccess层通用实现
    /// </summary>
    /// Copyright (c) 2011 海南海航航空信息系统有限公司
    /// 创 建 人：王宇（wang_yu5）
    /// 创建日期：2011-12-5
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    public class CommonDA : DABase
    {
        /// <summary>
        /// 构造函数，创建数据库实例
        /// </summary>
        /// <param name="strDBName">数据库连接配置名</param>
        /// <param name="dataAccess">数据访问接口（外部事务调用）</param>
        public CommonDA(String strDBName = "", DataAccess dataAccess = null)
            : base(strDBName, dataAccess)
        {
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns>表名</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public String GetTableName<T>(T model)
            where T : class,new()
        {
            return DataAccess.GetTableName(model);
        }

        /// <summary>
        /// 获取主键值
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <returns>返回主键值</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2012-10-22
        /// 修 改 人：
        /// 修改日期：
        public Dictionary<string, object> GetPrimaryKey<T>(T model) where T : class,new()
        {
            return DataAccess.GetPrimaryKey(model);
        }

        #region 查询

        #region 精确查询

        #region 获取数据返回第一个
        /// <summary>
        /// 精确查询，获取数据返回第一个
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <returns>数据实体</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public T GetOne<T>(T model)
            where T : class, new()
        {
            IList<T> results = GetList(model);

            if (results != null && results.Count > 0)
            {
                return results[0];
            }

            return null;
        }
        #endregion

        #region 获取数据返回List
        /// <summary>
        /// 获取数据返回List
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bAccurate">默认精确查询，false为模糊查询</param>
        /// <returns>返回泛型列表</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public IList<T> GetList<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bAccurate = true)
            where T : class, new()
        {
            if (bCache)
            {
                //获取表名
                String strTableName = GetTableName(model);

                //获取默认数据库连接字符串
                String strConnString = DataAccess.GetDefaultConnect();
                //获取缓存名称
                if (String.IsNullOrEmpty(strCacheKey))
                {
                    strCacheKey = strTableName;
                }

                SqlCacheDep sqlDep = new SqlCacheDep(strConnString);
                //启用对表的缓存
                sqlDep.EnableTableForNotifications(strTableName, strConnString);
                //停用对表的缓存
                //sqlDep.DisableTableForNotifications("tbStudent,tbClass");//支持多表


                //从缓存中获取
                var objModel = sqlDep.GetCache(strCacheKey);
                if (objModel == null)//缓存里没有
                {
                    //从数据库获取数据
                    if (bAccurate)
                    {
                        objModel = DataAccess.GetList(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    else
                    {
                        objModel = DataAccess.GetListByLike(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    //设置值
                    sqlDep.SetSqlCacheData(strCacheKey, objModel, strTableName);
                }
                return (IList<T>)objModel;
            }

            //精确查询
            if (bAccurate)
            {
                return DataAccess.GetList(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
            }
            //模糊查询
            return DataAccess.GetListByLike(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
        }
        #endregion

        #region 获取数据返回DataSet
        /// <summary>
        /// 获取数据返回DataSet
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bAccurate">默认精确查询，false为模糊查询</param>
        /// <returns>返回DataSet</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public DataSet GetDataSet<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bAccurate = true)
            where T : class, new()
        {
            if (bCache)
            {
                //获取表名
                String strTableName = GetTableName(model);

                //获取默认数据库连接字符串
                String strConnString = DataAccess.GetDefaultConnect();
                //获取缓存名称
                if (String.IsNullOrEmpty(strCacheKey))
                {
                    strCacheKey = strTableName;
                }

                //启用主动通知
                SqlCacheDep sqlDep = new SqlCacheDep(strConnString);
                sqlDep.EnableTableForNotifications(strTableName, strConnString);
                //停用
                //sqlDep.DisableTableForNotifications("tbStudent,tbClass");//支持多表


                //从缓存中获取
                var objModel = sqlDep.GetCache(strCacheKey);
                if (objModel == null)//缓存里没有
                {
                    //从数据库获取数据
                    if (bAccurate)
                    {
                        objModel = DataAccess.GetDataSet(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    else
                    {
                        objModel = DataAccess.GetDataSetByLike(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    //设置值
                    sqlDep.SetSqlCacheData(strCacheKey, objModel, strTableName);
                }
                return (DataSet)objModel;
            }

            //精确查询
            if (bAccurate)
            {
                return DataAccess.GetDataSet(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
            }
            //模糊查询
            return DataAccess.GetDataSetByLike(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
        }
        #endregion

        #region 获取数据返回DataReader
        /// <summary>
        /// 获取数据返回DataReader
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bAccurate">默认精确查询，false为模糊查询</param>
        /// <returns>返回DataReader</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public IDataReader GetDataReader<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bAccurate = true) where T : class, new()
        {
            if (bCache)
            {
                //获取表名
                String strTableName = GetTableName(model);

                //获取默认数据库连接字符串
                String strConnString = DataAccess.GetDefaultConnect();
                //获取缓存名称
                if (String.IsNullOrEmpty(strCacheKey))
                {
                    strCacheKey = strTableName;
                }

                //启用主动通知
                SqlCacheDep sqlDep = new SqlCacheDep(strConnString);
                sqlDep.EnableTableForNotifications(strTableName, strConnString);
                //停用
                //sqlDep.DisableTableForNotifications("tbStudent,tbClass");//支持多表


                //从缓存中获取
                var objModel = sqlDep.GetCache(strCacheKey);
                if (objModel == null)//缓存里没有
                {
                    //从数据库获取数据
                    if (bAccurate)
                    {
                        objModel = DataAccess.GetDataReader(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    else
                    {
                        objModel = DataAccess.GetDataReaderByLike(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    //设置值
                    sqlDep.SetSqlCacheData(strCacheKey, objModel, strTableName);
                }
                return (IDataReader)objModel;
            }

            //精确查询
            if (bAccurate)
            {
                return DataAccess.GetDataReader(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
            }
            //模糊查询
            return DataAccess.GetDataReaderByLike(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
        }
        #endregion

        #region 获取数据返回DataTable
        /// <summary>
        /// 获取数据返回DataTable
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bAccurate">默认精确查询，false为模糊查询</param>
        /// <returns>返回DataTable</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public DataTable GetDataTable<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bAccurate = true) where T : class, new()
        {
            if (bCache)
            {
                //获取表名
                String strTableName = GetTableName(model);

                //获取默认数据库连接字符串
                String strConnString = DataAccess.GetDefaultConnect();
                //获取缓存名称
                if (String.IsNullOrEmpty(strCacheKey))
                {
                    strCacheKey = strTableName;
                }

                //启用主动通知
                SqlCacheDep sqlDep = new SqlCacheDep(strConnString);
                sqlDep.EnableTableForNotifications(strTableName, strConnString);
                //停用
                //sqlDep.DisableTableForNotifications("tbStudent,tbClass");//支持多表


                //从缓存中获取
                var objModel = sqlDep.GetCache(strCacheKey);
                if (objModel == null)//缓存里没有
                {
                    //从数据库获取数据
                    if (bAccurate)
                    {
                        objModel = DataAccess.GetDataTable(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    else
                    {
                        objModel = DataAccess.GetDataTableByLike(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    //设置值
                    sqlDep.SetSqlCacheData(strCacheKey, objModel, strTableName);
                }
                return (DataTable)objModel;
            }

            //精确查询
            if (bAccurate)
            {
                return DataAccess.GetDataTable(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
            }
            //模糊查询
            return DataAccess.GetDataTableByLike(model, strOrderBy, strColumnFilter, strRowFilter, parameters);
        }
        #endregion

        #region 获取数据返回List（支持分页）
        /// <summary>
        /// 获取数据返回List（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bAccurate">默认精确查询，false为模糊查询</param>
        /// <returns>返回泛型列表</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public IList<T> GetList<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bAccurate = true)
            where T : class, new()
        {
            if (bCache)
            {
                //获取表名
                String strTableName = GetTableName(model);

                //获取默认数据库连接字符串
                String strConnString = DataAccess.GetDefaultConnect();
                //获取缓存名称
                if (String.IsNullOrEmpty(strCacheKey))
                {
                    strCacheKey = strTableName;
                }

                SqlCacheDep sqlDep = new SqlCacheDep(strConnString);
                //启用对表的缓存
                sqlDep.EnableTableForNotifications(strTableName, strConnString);
                //停用对表的缓存
                //sqlDep.DisableTableForNotifications("tbStudent,tbClass");//支持多表


                //从缓存中获取
                var objModel = sqlDep.GetCache(strCacheKey);
                if (objModel == null)//缓存里没有
                {
                    //从数据库获取数据
                    if (bAccurate)
                    {
                        objModel = DataAccess.GetList(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    else
                    {
                        objModel = DataAccess.GetListByLike(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    //设置值
                    sqlDep.SetSqlCacheData(strCacheKey, objModel, strTableName);
                }
                return (IList<T>)objModel;
            }

            //精确查询
            if (bAccurate)
            {
                return DataAccess.GetList(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
            }
            //模糊查询
            return DataAccess.GetListByLike(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
        }
        #endregion

        #region 获取数据返回DataSet（支持分页）
        /// <summary>
        /// 获取数据返回DataSet（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bAccurate">默认精确查询，false为模糊查询</param>
        /// <returns>返回DataSet</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public DataSet GetDataSet<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bAccurate = true)
            where T : class, new()
        {
            if (bCache)
            {
                //获取表名
                String strTableName = GetTableName(model);

                //获取默认数据库连接字符串
                String strConnString = DataAccess.GetDefaultConnect();
                //获取缓存名称
                if (String.IsNullOrEmpty(strCacheKey))
                {
                    strCacheKey = strTableName;
                }

                //启用主动通知
                SqlCacheDep sqlDep = new SqlCacheDep(strConnString);
                sqlDep.EnableTableForNotifications(strTableName, strConnString);
                //停用
                //sqlDep.DisableTableForNotifications("tbStudent,tbClass");//支持多表


                //从缓存中获取
                var objModel = sqlDep.GetCache(strCacheKey);
                if (objModel == null)//缓存里没有
                {
                    //从数据库获取数据
                    if (bAccurate)
                    {
                        objModel = DataAccess.GetDataSet(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    else
                    {
                        objModel = DataAccess.GetDataSetByLike(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    //设置值
                    sqlDep.SetSqlCacheData(strCacheKey, objModel, strTableName);
                }
                return (DataSet)objModel;
            }

            //精确查询
            if (bAccurate)
            {
                return DataAccess.GetDataSet(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
            }
            //模糊查询
            return DataAccess.GetDataSetByLike(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
        }
        #endregion

        #region 获取数据返回DataReader（支持分页）
        /// <summary>
        /// 获取数据返回DataReader（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bAccurate">默认精确查询，false为模糊查询</param>
        /// <returns>返回DataReader</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public IDataReader GetDataReader<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bAccurate = true) where T : class, new()
        {
            if (bCache)
            {
                //获取表名
                String strTableName = GetTableName(model);

                //获取默认数据库连接字符串
                String strConnString = DataAccess.GetDefaultConnect();
                //获取缓存名称
                if (String.IsNullOrEmpty(strCacheKey))
                {
                    strCacheKey = strTableName;
                }

                //启用主动通知
                SqlCacheDep sqlDep = new SqlCacheDep(strConnString);
                sqlDep.EnableTableForNotifications(strTableName, strConnString);
                //停用
                //sqlDep.DisableTableForNotifications("tbStudent,tbClass");//支持多表


                //从缓存中获取
                var objModel = sqlDep.GetCache(strCacheKey);
                if (objModel == null)//缓存里没有
                {
                    //从数据库获取数据
                    if (bAccurate)
                    {
                        objModel = DataAccess.GetDataReader(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    else
                    {
                        objModel = DataAccess.GetDataReaderByLike(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    //设置值
                    sqlDep.SetSqlCacheData(strCacheKey, objModel, strTableName);
                }
                return (IDataReader)objModel;
            }

            //精确查询
            if (bAccurate)
            {
                return DataAccess.GetDataReader(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
            }
            //模糊查询
            return DataAccess.GetDataReaderByLike(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
        }
        #endregion

        #region 获取数据返回DataTable（支持分页）
        /// <summary>
        /// 获取数据返回DataTable（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <param name="bAccurate">默认精确查询，false为模糊查询</param>
        /// <returns>返回DataTable</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public DataTable GetDataTable<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null, bool bAccurate = true)
            where T : class, new()
        {
            if (bCache)
            {
                //获取表名
                String strTableName = GetTableName(model);

                //获取默认数据库连接字符串
                String strConnString = DataAccess.GetDefaultConnect();
                //获取缓存名称
                if (String.IsNullOrEmpty(strCacheKey))
                {
                    strCacheKey = strTableName;
                }

                //启用主动通知
                SqlCacheDep sqlDep = new SqlCacheDep(strConnString);
                sqlDep.EnableTableForNotifications(strTableName, strConnString);
                //停用
                //sqlDep.DisableTableForNotifications("tbStudent,tbClass");//支持多表

                //从缓存中获取
                var objModel = sqlDep.GetCache(strCacheKey);
                if (objModel == null)//缓存里没有
                {
                    //从数据库获取数据
                    if (bAccurate)
                    {
                        objModel = DataAccess.GetDataTable(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    else
                    {
                        objModel = DataAccess.GetDataTableByLike(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
                    }
                    //设置值
                    sqlDep.SetSqlCacheData(strCacheKey, objModel, strTableName);
                }
                return (DataTable)objModel;
            }

            //精确查询
            if (bAccurate)
            {
                return DataAccess.GetDataTable(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
            }
            //模糊查询
            return DataAccess.GetDataTableByLike(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters);
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
        /// <returns>数据实体</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public T GetOneByLike<T>(T model)
            where T : class, new()
        {
            IList<T> results = GetListByLike(model);

            if (results != null && results.Count > 0)
            {
                return results[0];
            }

            return null;
        }
        #endregion

        #region 获取数据返回List
        /// <summary>
        /// 模糊查询，获取数据返回List
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回泛型列表</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public IList<T> GetListByLike<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null)
            where T : class, new()
        {
            return GetList(model, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters, false);
        }
        #endregion

        #region 获取数据返回DataSet
        /// <summary>
        /// 模糊查询，获取数据返回DataSet
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataSet</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public DataSet GetDataSetByLike<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null)
            where T : class, new()
        {
            return GetDataSet(model, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters, false);
        }
        #endregion

        #region 获取数据返回DataReader
        /// <summary>
        /// 模糊查询，获取数据返回DataReader
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataReader</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public IDataReader GetDataReaderByLike<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null)
            where T : class, new()
        {
            return GetDataReader(model, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters, false);
        }
        #endregion

        #region 获取数据返回DataTable
        /// <summary>
        /// 模糊查询，获取数据返回DataTable
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataTable</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public DataTable GetDataTableByLike<T>(T model, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null)
            where T : class, new()
        {
            return GetDataTable(model, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters, false);
        }
        #endregion

        #region 获取数据返回List（分页）
        /// <summary>
        /// 模糊查询，获取数据返回List（分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回泛型列表</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public IList<T> GetListByLike<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null)
            where T : class, new()
        {
            return GetList(model, ref pager, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters, false);
        }
        #endregion

        #region 获取数据返回DataSet（分页）
        /// <summary>
        /// 模糊查询，获取数据返回DataSet（分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataSet</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public DataSet GetDataSetByLike<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null)
            where T : class, new()
        {
            return GetDataSet(model, ref pager, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters, false);
        }
        #endregion

        #region 获取数据返回DataReader（分页）
        /// <summary>
        /// 模糊查询，获取数据返回DataReader（分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataReader</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public IDataReader GetDataReaderByLike<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null)
            where T : class, new()
        {
            return GetDataReader(model, ref pager, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters, false);
        }
        #endregion

        #region 获取数据返回DataTable（分页）
        /// <summary>
        /// 模糊查询，获取数据返回DataTable（分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="bCache">true 启用缓存; false 禁用缓存；默认不缓存</param>
        /// <param name="strCacheKey">缓存名称，默认为表名</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataTable</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public DataTable GetDataTableByLike<T>(T model, ref Pager pager, bool bCache = false, String strCacheKey = "", String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null)
            where T : class, new()
        {
            return GetDataTable(model, ref pager, bCache, strCacheKey, strOrderBy, strColumnFilter, strRowFilter, parameters, false);
        }
        #endregion

        #endregion

        #endregion

        #region 添加

        #region 添加数据

        /// <summary>
        /// 添加数据，返回主键
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <returns>主键</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public String Add<T>(T model) where T : class, new()
        {
            return DataAccess.Add(model);
        }

        /// <summary>
        /// 添加数据，返回bool
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <returns>返回操作成功与否</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public bool AddReturnBool<T>(T model) where T : class, new()
        {
            return DataAccess.AddReturnBool(model);
        }
        #endregion

        #region 批量添加数据
        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="modelList">数据实体集合</param>
        /// <returns>返回操作成功与否</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public bool Add<T>(IList<T> modelList) where T : class, new()
        {
            return DataAccess.AddList(modelList);
        }
        #endregion

        #endregion

        #region 更新

        #region 更新数据
        /// <summary>
        /// 更新记录 （默认更新条件为主键）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="conditionParameters">条件参数，参数名称请与数据库列名一致</param>
        /// <returns>返回操作成功与否</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public bool Update<T>(T model, DbParameter[] conditionParameters = null) where T : class, new()
        {
            return DataAccess.Update(model, conditionParameters);
        }
        #endregion

        #region 批量更新数据
        /// <summary>
        /// 批量更新数据（默认更新条件为主键）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="modelList">数据实体集合</param>
        /// <returns>返回操作成功与否</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public bool Update<T>(IList<T> modelList) where T : class, new()
        {
            return DataAccess.UpdateList(modelList);
        }
        #endregion

        #region 批量更新数据（自定义条件更新）
        /// <summary>
        /// 批量更新数据（自定义条件更新）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="dicModel_Parameter">数据实体、更新条件键值集合</param>
        /// <returns>返回操作成功与否</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public bool Update<T>(Dictionary<T, DbParameter[]> dicModel_Parameter) where T : class,new()
        {
            return DataAccess.UpdateList(dicModel_Parameter);
        }
        #endregion
        #endregion

        #region 删除

        #region 删除数据
        /// <summary>
        /// 删除数据（默认删除条件为主键）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="conditionParameters">条件参数，参数名称请与数据库列名一致</param>
        /// <returns>返回操作成功与否</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public bool Delete<T>(T model, DbParameter[] conditionParameters = null) where T : class, new()
        {
            return DataAccess.Delete(model, conditionParameters);
        }
        #endregion

        #region 批量删除数据
        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="modelList">数据实体集合</param>
        /// <returns>返回操作成功与否</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public bool Delete<T>(IList<T> modelList) where T : class, new()
        {
            return DataAccess.DeleteList(modelList);
        }
        #endregion

        #region 批量删除数据（自定义条件删除）
        /// <summary>
        /// 批量删除数据（自定义条件删除）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="dicModel_Parameter">数据实体、更新条件键值集合</param>
        /// <returns>返回操作成功与否</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public bool Delete<T>(Dictionary<T, DbParameter[]> dicModel_Parameter) where T : class,new()
        {
            return DataAccess.DeleteList(dicModel_Parameter);
        }
        #endregion
        #endregion
    }
}
