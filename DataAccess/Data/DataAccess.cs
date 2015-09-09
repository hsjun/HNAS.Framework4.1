using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Linq.Mapping;
using System.Reflection;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace HNAS.Framework4.Data
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    ///  创 建 人：余鹏飞
    ///  创建日期：2011年12月14日
    ///  修 改 人：王宇
    ///  修改日期：2012年2月24日
    ///  Copyright (c) 2012 海南海航航空信息系统有限公司
    ///  版 本：1.0	
    public class DataAccess
    {
        #region 属性

        /// <summary>
        /// 数据库实例
        /// </summary>
        public Database db
        {
            get;
            set;
        }
        /// <summary>
        /// 数据库连接
        /// </summary>
        public DbConnection dbConnection
        {
            get;
            set;
        }
        /// <summary>
        /// 数据库事务
        /// </summary>
        public DbTransaction dbTransaction
        {
            get;
            set;
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数，创建数据库实例
        /// </summary>
        /// <param name="strDBName">数据库连接配置名</param>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public DataAccess(String strDBName = "")
        {
            db = EnterpriseLibraryContainer.Current.GetInstance<Database>(strDBName);
        }
        #endregion

        #region 获取表名
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <returns>表名</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public String GetTableName<T>(T model) where T : class,new()
        {
            return ((TableAttribute)model.GetType().GetCustomAttributes(false)[0]).Name;
        }
        #endregion

        #region 获取默认数据库连接字符串
        /// <summary>
        /// 获取默认数据库连接字符串
        /// </summary>
        /// <returns>默认数据库连接字符串</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public String GetDefaultConnect()
        {
            var dataConfigurationSection = ConfigurationManager.GetSection("dataConfiguration") as ConfigurationSection;
            var databaseSettings = dataConfigurationSection as Microsoft.Practices.EnterpriseLibrary.Data.Configuration.DatabaseSettings;
            if (databaseSettings != null)
            {
                String defaultDatabase = databaseSettings.DefaultDatabase;

                return ConfigurationManager.ConnectionStrings[defaultDatabase].ToString();
            }
            return null;
        }
        #endregion

        #region 查询

        #region 精确查询

        #region 返回泛型列表
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>100"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回泛型列表</returns>
        public IList<T> GetList<T>(T model, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            IList<T> listDataModel = new List<T>();//返回值

            using (DataTable dt = GetDataSet(model, strOrderBy, strColumnFilter, strRowFilter, parameters).Tables[0])
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    T item = Activator.CreateInstance<T>();

                    ConvertToEntity(item, dt.Rows[i]);
                    listDataModel.Add(item);
                }
            }
            return listDataModel;
        }
        #endregion

        #region 返回DataSet
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>100"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataSet</returns>
        public DataSet GetDataSet<T>(T model, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            if (String.IsNullOrEmpty(strColumnFilter))
            {
                throw new ArgumentNullException("strColumnFilter");
            }

            ArrayList alColumn = new ArrayList();//表所有列集合
            Type entityType = model.GetType();
            TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];

            String strSql = "SELECT " + strColumnFilter + " FROM " + table.Name + " WHERE 1=1 " + strRowFilter;
            DbCommand dcmd = db.DbProviderFactory.CreateCommand();
            if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
            dcmd.CommandType = CommandType.Text;

            //是否有自定义参数
            if (parameters != null && parameters.Count > 0)
            {
                dcmd.Parameters.AddRange(parameters.ToArray());
            }

            #region 组装SQL
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }
                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;
                    alColumn.Add(objectAttr.Name);
                    object oval = info.GetValue(model, null);
                    if (oval != null && !Convert.IsDBNull(oval))
                    {
                        if (objectAttr.DbType.Equals("datetime"))//日期按天查询
                        {
                            String strDateTime = DateTime.Parse(oval.ToString()).ToString("yyyy-MM-dd");

                            strSql += " AND " + objectAttr.Name + " BETWEEN CONVERT(datetime, @" + objectAttr.Name + ") AND DATEADD(ms, -3, DATEADD(dd,1,CONVERT(datetime, @" + objectAttr.Name + ")))";
                            db.AddInParameter(dcmd, objectAttr.Name, DbType.String, strDateTime);
                        }
                        else
                        {
                            strSql += " AND " + objectAttr.Name + "=@" + objectAttr.Name;
                            db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                        }
                    }
                }
            }
            #endregion

            #region 参数合法性判断
            //判断列过滤参数合法性
            if (!strColumnFilter.Equals("*"))
            {
                String[] strColumns = strColumnFilter.Split(',');//列过滤列名
                IfColumnValid(strColumns, alColumn);
            }

            //是否有排序参数
            if (!String.IsNullOrEmpty(strOrderBy))
            {
                String[] strOrderColumns = strOrderBy.ToUpper().Replace("DESC", "").Replace("ASC", "").Trim().Split(',');//列过滤列名
                //判断排序参数合法性
                IfColumnValid(strOrderColumns, alColumn);

                strSql += " ORDER BY " + strOrderBy;
            }
            #endregion

            if (dcmd == null) throw new NullReferenceException();
            dcmd.CommandText = strSql;
            dcmd.CommandTimeout = 60;//默认30秒

            //是否有外部事务
            if (dbTransaction == null)
            {
                return db.ExecuteDataSet(dcmd);
            }
            else
            {
                return db.ExecuteDataSet(dcmd, dbTransaction);
            }
        }
        #endregion

        #region 返回DataReader
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>100"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataReader</returns>
        public IDataReader GetDataReader<T>(T model, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            if (String.IsNullOrEmpty(strColumnFilter))
            {
                throw new ArgumentNullException("strColumnFilter");
            }

            ArrayList alColumn = new ArrayList();//表所有列集合
            Type entityType = model.GetType();
            TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];
            String strSql = "SELECT " + strColumnFilter + " FROM " + table.Name + " WHERE 1=1 " + strRowFilter;
            DbCommand dcmd = db.DbProviderFactory.CreateCommand();
            if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
            dcmd.CommandType = CommandType.Text;

            //是否有自定义参数
            if (parameters != null && parameters.Count > 0)
            {
                dcmd.Parameters.AddRange(parameters.ToArray());
            }

            #region 组装SQL
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }
                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;
                    alColumn.Add(objectAttr.Name);
                    object oval = info.GetValue(model, null);
                    if (oval != null && !Convert.IsDBNull(oval))
                    {
                        if (objectAttr.DbType.Equals("datetime"))//日期按天查询
                        {
                            String strDateTime = DateTime.Parse(oval.ToString()).ToString("yyyy-MM-dd");
                            strSql += " AND " + objectAttr.Name + " BETWEEN CONVERT(datetime, @" + objectAttr.Name + ") AND DATEADD(ms, -3, DATEADD(dd,1,CONVERT(datetime, @" + objectAttr.Name + ")))";
                            db.AddInParameter(dcmd, objectAttr.Name, DbType.String, strDateTime);
                        }
                        else
                        {
                            strSql += " AND " + objectAttr.Name + "=@" + objectAttr.Name;
                            db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                        }
                    }
                }
            }
            #endregion

            #region 参数合法性判断
            //判断列过滤参数合法性
            if (!strColumnFilter.Equals("*"))
            {
                String[] strColumns = strColumnFilter.Split(',');//列过滤列名
                IfColumnValid(strColumns, alColumn);
            }

            //是否有排序参数
            if (!String.IsNullOrEmpty(strOrderBy))
            {
                String[] strOrderColumns = strOrderBy.ToUpper().Replace("DESC", "").Replace("ASC", "").Trim().Split(',');//列过滤列名
                //判断排序参数合法性
                IfColumnValid(strOrderColumns, alColumn);

                strSql += " ORDER BY " + strOrderBy;
            }
            #endregion

            dcmd.CommandText = strSql;
            dcmd.CommandTimeout = 60;//默认30秒

            //是否有外部事务
            if (dbTransaction == null)
            {
                return db.ExecuteReader(dcmd);
            }
            else
            {
                return db.ExecuteReader(dcmd, dbTransaction);
            }
        }
        #endregion

        #region 返回DataTable
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>100"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataTable</returns>
        public DataTable GetDataTable<T>(T model, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            return GetDataSet(model, strOrderBy, strColumnFilter, strRowFilter, parameters).Tables[0];
        }
        #endregion

        #region 返回泛型列表（支持分页）
        /// <summary>
        /// 查询（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>100"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回泛型列表</returns>
        public IList<T> GetList<T>(T model, ref Pager pager, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            IList<T> listDataModel = new List<T>();//返回值

            using (DataTable dt = GetDataSet(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters).Tables[0])
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    T item = Activator.CreateInstance<T>();
                    ConvertToEntity(item, dt.Rows[i]);
                    listDataModel.Add(item);
                }
            }
            return listDataModel;
        }
        #endregion

        #region 返回DataSet（支持分页）
        /// <summary>
        /// 查询（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>100"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataSet</returns>
        public DataSet GetDataSet<T>(T model, ref Pager pager, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            if (String.IsNullOrEmpty(strColumnFilter))
            {
                throw new ArgumentNullException("strColumnFilter");
            }

            ArrayList alColumn = new ArrayList();//表所有列集合
            Type entityType = model.GetType();
            TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];

            String strIndexName = String.Empty;//索引列名
            String strSql = " WHERE 1=1 " + strRowFilter;//SQL语句
            DbCommand dcmd = db.DbProviderFactory.CreateCommand();
            if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
            dcmd.CommandType = CommandType.Text;

            //是否有自定义参数
            if (parameters != null && parameters.Count > 0)
            {
                dcmd.Parameters.AddRange(parameters.ToArray());
            }

            #region 组装SQL
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }
                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;
                    if (objectAttr.IsPrimaryKey && String.IsNullOrEmpty(strIndexName))//获取索引列名
                    {
                        strIndexName = objectAttr.Storage;
                    }

                    alColumn.Add(objectAttr.Name);
                    object oval = info.GetValue(model, null);
                    if (oval != null && !Convert.IsDBNull(oval))
                    {
                        if (objectAttr.DbType.Equals("datetime"))//日期按天查询
                        {
                            String strDateTime = DateTime.Parse(oval.ToString()).ToString("yyyy-MM-dd");
                            strSql += " AND " + objectAttr.Name + " BETWEEN CONVERT(datetime, @" + objectAttr.Name + ") AND DATEADD(ms, -3, DATEADD(dd,1,CONVERT(datetime, @" + objectAttr.Name + ")))";
                            db.AddInParameter(dcmd, objectAttr.Name, DbType.String, strDateTime);
                        }
                        else
                        {
                            strSql += " AND " + objectAttr.Name + "=@" + objectAttr.Name;
                            db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                        }
                    }
                }
            }
            #endregion

            #region 参数合法性判断
            //判断列过滤参数合法性
            if (!strColumnFilter.Equals("*"))
            {
                String[] strColumns = strColumnFilter.Split(',');//列过滤列名
                IfColumnValid(strColumns, alColumn);
            }

            //是否有排序参数
            if (!String.IsNullOrEmpty(strOrderBy))
            {
                String[] strOrderColumns = strOrderBy.ToUpper().Replace("DESC", "").Replace("ASC", "").Trim().Split(',');//列过滤列名
                //判断排序参数合法性
                IfColumnValid(strOrderColumns, alColumn);
            }
            else
            {
                strOrderBy = strIndexName;
            }

            #endregion

            String strCount = "SELECT COUNT(0) AS Counts FROM " + table.Name + strSql;

            //strSql = String.Format(@"SELECT {4} FROM ( SELECT TOP {0} * FROM ( SELECT TOP ( {0}* {1}) * FROM {2} {5} ORDER BY {3} ASC) as a ORDER BY {3} DESC ) as b ORDER BY {6}; {7}", pager.PageSize, pager.PageIndex, table.Name, strIndexName, strColumnFilter, strSql, strOrderBy, strCount);

            int iStartIndex = (pager.PageIndex - 1) * pager.PageSize;
            int iEndndex = pager.PageIndex * pager.PageSize;

            if (string.IsNullOrEmpty(strOrderBy))
            {
                strOrderBy = "(SELECT 0)";
            }
            strSql = String.Format(@"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS RowNum,* FROM (SELECT {1} FROM {2} {3} ) a ) b
WHERE RowNum>{4} AND RowNum<={5}; {6}", strOrderBy, strColumnFilter, table.Name, strSql, iStartIndex, iEndndex, strCount);

            dcmd.CommandText = strSql;
            dcmd.CommandTimeout = 60;//默认30秒


            DataSet ds = new DataSet();
            //是否有外部事务
            if (dbTransaction == null)
            {
                ds = db.ExecuteDataSet(dcmd);
            }
            else
            {
                ds = db.ExecuteDataSet(dcmd, dbTransaction);
            }

            //分页数据
            pager.Total = int.Parse(ds.Tables[1].Rows[0]["Counts"].ToString());
            pager.PageCount = pager.Total / pager.PageSize;
            if (pager.Total % pager.PageSize != 0)
            {
                pager.PageCount = pager.PageCount + 1;
            }

            return ds;
        }
        #endregion

        #region 返回DataReader（支持分页）
        /// <summary>
        /// 查询（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>100"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataReader</returns>
        public IDataReader GetDataReader<T>(T model, ref Pager pager, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            if (String.IsNullOrEmpty(strColumnFilter))
            {
                throw new ArgumentNullException("strColumnFilter");
            }

            ArrayList alColumn = new ArrayList();//表所有列集合
            Type entityType = model.GetType();
            TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];

            String strIndexName = String.Empty;//索引列名
            String strSql = " WHERE 1=1 " + strRowFilter;//SQL语句
            DbCommand dcmd = db.DbProviderFactory.CreateCommand();
            if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
            dcmd.CommandType = CommandType.Text;

            //是否有自定义参数
            if (parameters != null && parameters.Count > 0)
            {
                dcmd.Parameters.AddRange(parameters.ToArray());
            }

            #region 组装SQL
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }
                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;
                    if (objectAttr.IsPrimaryKey && String.IsNullOrEmpty(strIndexName))//获取索引列名
                    {
                        strIndexName = objectAttr.Storage;
                    }
                    alColumn.Add(objectAttr.Name);
                    object oval = info.GetValue(model, null);
                    if (oval != null && !Convert.IsDBNull(oval))
                    {
                        if (objectAttr.DbType.Equals("datetime"))//日期按天查询
                        {
                            String strDateTime = DateTime.Parse(oval.ToString()).ToString("yyyy-MM-dd");
                            strSql += " AND " + objectAttr.Name + " BETWEEN CONVERT(datetime, @" + objectAttr.Name + ") AND DATEADD(ms, -3, DATEADD(dd,1,CONVERT(datetime, @" + objectAttr.Name + ")))";
                            db.AddInParameter(dcmd, objectAttr.Name, DbType.String, strDateTime);
                        }
                        else
                        {
                            strSql += " AND " + objectAttr.Name + "=@" + objectAttr.Name;
                            db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                        }
                    }
                }
            }
            #endregion

            #region 参数合法性判断
            //判断列过滤参数合法性
            if (!strColumnFilter.Equals("*"))
            {
                String[] strColumns = strColumnFilter.Split(',');//列过滤列名
                IfColumnValid(strColumns, alColumn);
            }

            //是否有排序参数
            if (!String.IsNullOrEmpty(strOrderBy))
            {
                String[] strOrderColumns = strOrderBy.ToUpper().Replace("DESC", "").Replace("ASC", "").Trim().Split(',');//列过滤列名
                //判断排序参数合法性
                IfColumnValid(strOrderColumns, alColumn);
            }
            else
            {
                strOrderBy = strIndexName;
            }

            #endregion

            String strCount = "SELECT COUNT(*) FROM " + table.Name + strSql;
            dcmd.CommandText = strCount;
            pager.Total = int.Parse(db.ExecuteScalar(dcmd).ToString());
            pager.PageCount = pager.Total / pager.PageSize;
            if (pager.Total % pager.PageSize != 0)
            {
                pager.PageCount = pager.PageCount + 1;
            }

            //strSql = String.Format(@"SELECT {4} FROM ( SELECT TOP {0} * FROM ( SELECT TOP ( {0}* {1}) * FROM {2} {5} ORDER BY {3} ASC) as a ORDER BY {3} DESC ) as b ORDER BY {6}", pager.PageSize, pager.PageIndex, table.Name, strIndexName, strColumnFilter, strSql, strOrderBy);
            int iStartIndex = (pager.PageIndex - 1) * pager.PageSize;
            int iEndndex = pager.PageIndex * pager.PageSize;

            if (string.IsNullOrEmpty(strOrderBy))
            {
                strOrderBy = "(SELECT 0)";
            }
            strSql = String.Format(@"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS RowNum,* FROM (SELECT {1} FROM {2} {3} ) a ) b
WHERE RowNum>{4} AND RowNum<={5}", strOrderBy, strColumnFilter, table.Name, strSql, iStartIndex, iEndndex);

            dcmd.CommandText = strSql;
            dcmd.CommandTimeout = 60;//默认30秒

            DataSet ds = new DataSet();
            //是否有外部事务
            if (dbTransaction == null)
            {
                return db.ExecuteReader(dcmd);
            }
            else
            {
                return db.ExecuteReader(dcmd, dbTransaction);
            }
        }
        #endregion

        #region 返回DataTable（支持分页）
        /// <summary>
        /// 查询（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>100"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataTable</returns>
        public DataTable GetDataTable<T>(T model, ref Pager pager, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            return GetDataSet(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters).Tables[0];
        }
        #endregion

        #endregion

        #region 模糊查询

        #region 返回泛型列表
        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>100"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回泛型列表</returns>
        public IList<T> GetListByLike<T>(T model, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            IList<T> listDataModel = new List<T>();//返回值

            using (DataTable dt = GetDataSetByLike(model, strOrderBy, strColumnFilter, strRowFilter, parameters).Tables[0])
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    T item = Activator.CreateInstance<T>();
                    ConvertToEntity(item, dt.Rows[i]);
                    listDataModel.Add(item);
                }
            }
            return listDataModel;
        }
        #endregion

        #region 返回DataSet
        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>100"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataSet</returns>
        public DataSet GetDataSetByLike<T>(T model, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            if (String.IsNullOrEmpty(strColumnFilter))
            {
                throw new ArgumentNullException("strColumnFilter");
            }

            ArrayList alColumn = new ArrayList();//表所有列集合
            Type entityType = model.GetType();
            TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];

            String strSql = "SELECT " + strColumnFilter + " FROM " + table.Name + " WHERE 1=1 " + strRowFilter;
            DbCommand dcmd = db.DbProviderFactory.CreateCommand();
            if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
            dcmd.CommandType = CommandType.Text;

            //是否有自定义参数
            if (parameters != null && parameters.Count > 0)
            {
                dcmd.Parameters.AddRange(parameters.ToArray());
            }

            #region 组装SQL
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }

                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;
                    alColumn.Add(objectAttr.Name);
                    object oval = info.GetValue(model, null);

                    if (oval != null && !Convert.IsDBNull(oval))
                    {
                        if (objectAttr.DbType.Equals("datetime"))//日期按天查询
                        {
                            String strDateTime = DateTime.Parse(oval.ToString()).ToString("yyyy-MM-dd");
                            strSql += " AND " + objectAttr.Name + " BETWEEN CONVERT(datetime, @" + objectAttr.Name + ") AND DATEADD(ms, -3, DATEADD(dd,1,CONVERT(datetime, @" + objectAttr.Name + ")))";
                            db.AddInParameter(dcmd, objectAttr.Name, DbType.String, strDateTime);
                        }
                        else if (NeedFuzzy(objectAttr.DbType))
                        {
                            strSql += " AND " + objectAttr.Name + " LIKE @" + objectAttr.Name;
                            db.AddInParameter(dcmd, objectAttr.Name, DbType.String, "%" + oval + "%");
                        }
                        else
                        {
                            strSql += " AND " + objectAttr.Name + " =@" + objectAttr.Name;
                            db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                        }
                    }
                }
            }
            #endregion

            #region 参数合法性判断
            //判断列过滤参数合法性
            if (!strColumnFilter.Equals("*"))
            {
                String[] strColumns = strColumnFilter.Split(',');//列过滤列名
                IfColumnValid(strColumns, alColumn);
            }

            //是否有排序参数
            if (!String.IsNullOrEmpty(strOrderBy))
            {
                String[] strOrderColumns = strOrderBy.ToUpper().Replace("DESC", "").Replace("ASC", "").Trim().Split(',');//列过滤列名
                //判断排序参数合法性
                IfColumnValid(strOrderColumns, alColumn);

                strSql += " ORDER BY " + strOrderBy;
            }
            #endregion

            dcmd.CommandText = strSql;
            dcmd.CommandTimeout = 60;//默认30秒

            //是否有外部事务
            if (dbTransaction == null)
            {
                return db.ExecuteDataSet(dcmd);
            }
            else
            {
                return db.ExecuteDataSet(dcmd, dbTransaction);
            }
        }
        #endregion

        #region 返回DataReader
        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataReader</returns>
        public IDataReader GetDataReaderByLike<T>(T model, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            if (String.IsNullOrEmpty(strColumnFilter))
            {
                throw new ArgumentNullException("strColumnFilter");
            }

            ArrayList alColumn = new ArrayList();//表所有列集合
            Type entityType = model.GetType();
            TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];
            String strSql = "SELECT * FROM " + table.Name + " WHERE 1=1 " + strRowFilter;
            DbCommand dcmd = db.DbProviderFactory.CreateCommand();
            if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
            dcmd.CommandType = CommandType.Text;

            //是否有自定义参数
            if (parameters != null && parameters.Count > 0)
            {
                dcmd.Parameters.AddRange(parameters.ToArray());
            }

            #region 组装SQL
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }

                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;
                    alColumn.Add(objectAttr.Name);
                    object oval = info.GetValue(model, null);
                    if (oval != null && !Convert.IsDBNull(oval))
                    {
                        if (objectAttr.DbType.Equals("datetime"))//日期按天查询
                        {
                            String strDateTime = DateTime.Parse(oval.ToString()).ToString("yyyy-MM-dd");
                            strSql += " AND " + objectAttr.Name + " BETWEEN CONVERT(datetime, @" + objectAttr.Name + ") AND DATEADD(ms, -3, DATEADD(dd,1,CONVERT(datetime, @" + objectAttr.Name + ")))";
                            db.AddInParameter(dcmd, objectAttr.Name, DbType.String, strDateTime);
                        }
                        else if (NeedFuzzy(objectAttr.DbType))
                        {
                            strSql = strSql + " AND " + objectAttr.Name + " LIKE @" + objectAttr.Name;
                            db.AddInParameter(dcmd, objectAttr.Name, DbType.String, "%" + oval + "%");
                        }
                        else
                        {
                            strSql = strSql + " AND " + objectAttr.Name + " =@" + objectAttr.Name;
                            db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                        }
                    }
                }
            }
            #endregion

            #region 参数合法性判断
            //判断列过滤参数合法性
            if (!strColumnFilter.Equals("*"))
            {
                String[] strColumns = strColumnFilter.Split(',');//列过滤列名
                IfColumnValid(strColumns, alColumn);
            }

            //是否有排序参数
            if (!String.IsNullOrEmpty(strOrderBy))
            {
                String[] strOrderColumns = strOrderBy.ToUpper().Replace("DESC", "").Replace("ASC", "").Trim().Split(',');//列过滤列名
                //判断排序参数合法性
                IfColumnValid(strOrderColumns, alColumn);

                strSql += " ORDER BY " + strOrderBy;
            }
            #endregion

            dcmd.CommandText = strSql;
            dcmd.CommandTimeout = 60;//默认30秒

            //是否有外部事务
            if (dbTransaction == null)
            {
                return db.ExecuteReader(dcmd);
            }
            else
            {
                return db.ExecuteReader(dcmd, dbTransaction);
            }
        }
        #endregion

        #region 返回DataTable
        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataTable</returns>
        public DataTable GetDataTableByLike<T>(T model, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            return GetDataSetByLike(model, strOrderBy, strColumnFilter, strRowFilter, parameters).Tables[0];
        }
        #endregion

        #region 返回泛型列表（支持分页）
        /// <summary>
        /// 模糊查询（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回泛型列表</returns>
        public IList<T> GetListByLike<T>(T model, ref Pager pager, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            IList<T> listDataModel = new List<T>();//返回值

            using (DataTable dt = GetDataSetByLike(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters).Tables[0])
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    T item = Activator.CreateInstance<T>();
                    ConvertToEntity(item, dt.Rows[i]);
                    listDataModel.Add(item);
                }
            }
            return listDataModel;
        }
        #endregion

        #region 返回DataSet（支持分页）
        /// <summary>
        /// 模糊查询（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataSet</returns>
        public DataSet GetDataSetByLike<T>(T model, ref Pager pager, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            if (String.IsNullOrEmpty(strColumnFilter))
            {
                throw new ArgumentNullException("strColumnFilter");
            }

            ArrayList alColumn = new ArrayList();//表所有列集合
            Type entityType = model.GetType();
            TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];
            String strIndexName = String.Empty;//索引列名
            String strSql = " WHERE 1=1 " + strRowFilter;//SQL语句
            DbCommand dcmd = db.DbProviderFactory.CreateCommand();
            if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
            dcmd.CommandType = CommandType.Text;

            //是否有自定义参数
            if (parameters != null && parameters.Count > 0)
            {
                dcmd.Parameters.AddRange(parameters.ToArray());
            }

            #region 组装SQL
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }

                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;
                    alColumn.Add(objectAttr.Name);

                    if (objectAttr.IsPrimaryKey && String.IsNullOrEmpty(strIndexName))//获取索引列名
                    {
                        strIndexName = objectAttr.Storage;
                    }
                    object oval = info.GetValue(model, null);

                    if (oval != null && !Convert.IsDBNull(oval))
                    {
                        if (objectAttr.DbType.Equals("datetime"))//日期按天查询
                        {
                            String strDateTime = DateTime.Parse(oval.ToString()).ToString("yyyy-MM-dd");
                            strSql += " AND " + objectAttr.Name + " BETWEEN CONVERT(datetime, @" + objectAttr.Name + ") AND DATEADD(ms, -3, DATEADD(dd,1,CONVERT(datetime, @" + objectAttr.Name + ")))";
                            db.AddInParameter(dcmd, objectAttr.Name, DbType.String, strDateTime);
                        }
                        else if (NeedFuzzy(objectAttr.DbType))
                        {
                            strSql += " AND " + objectAttr.Name + " LIKE @" + objectAttr.Name;
                            db.AddInParameter(dcmd, objectAttr.Name, DbType.String, "%" + oval + "%");
                        }
                        else
                        {
                            strSql += " AND " + objectAttr.Name + " =@" + objectAttr.Name;
                            db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                        }
                    }
                }
            }
            #endregion

            #region 参数合法性判断
            //判断列过滤参数合法性
            if (!strColumnFilter.Equals("*"))
            {
                String[] strColumns = strColumnFilter.Split(',');//列过滤列名
                IfColumnValid(strColumns, alColumn);
            }

            //是否有排序参数
            if (!String.IsNullOrEmpty(strOrderBy))
            {
                String[] strOrderColumns = strOrderBy.ToUpper().Replace("DESC", "").Replace("ASC", "").Trim().Split(',');//列过滤列名
                //判断排序参数合法性
                IfColumnValid(strOrderColumns, alColumn);
            }
            else
            {
                strOrderBy = strIndexName;
            }
            #endregion

            String strCount = "SELECT COUNT(0) AS Counts FROM " + table.Name + strSql;

            //strSql = String.Format(@"SELECT {4} FROM ( SELECT TOP {0} * FROM ( SELECT TOP ( {0}* {1}) * FROM {2} {5} ORDER BY {3} ASC) as a ORDER BY {3} DESC ) as b ORDER BY {6}; {7}", pager.PageSize, pager.PageIndex, table.Name, strIndexName, strColumnFilter, strSql, strOrderBy, strCount);
            int iStartIndex = (pager.PageIndex - 1) * pager.PageSize;
            int iEndndex = pager.PageIndex * pager.PageSize;

            if (string.IsNullOrEmpty(strOrderBy))
            {
                strOrderBy = "(SELECT 0)";
            }
            strSql = String.Format(@"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS RowNum,* FROM (SELECT {1} FROM {2} {3} ) a ) b
WHERE RowNum>{4} AND RowNum<={5}; {6}", strOrderBy, strColumnFilter, table.Name, strSql, iStartIndex, iEndndex, strCount);

            dcmd.CommandText = strSql;
            dcmd.CommandTimeout = 60;//默认30秒

            DataSet ds = new DataSet();
            //是否有外部事务
            if (dbTransaction == null)
            {
                ds = db.ExecuteDataSet(dcmd);
            }
            else
            {
                ds = db.ExecuteDataSet(dcmd, dbTransaction);
            }

            //分页数据
            pager.Total = int.Parse(ds.Tables[1].Rows[0]["Counts"].ToString());
            pager.PageCount = pager.Total / pager.PageSize;
            if (pager.Total % pager.PageSize != 0)
            {
                pager.PageCount = pager.PageCount + 1;
            }

            return ds;
        }
        #endregion

        #region 返回DataReader（支持分页）
        /// <summary>
        /// 模糊查询（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataReader</returns>
        public IDataReader GetDataReaderByLike<T>(T model, ref Pager pager, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            if (String.IsNullOrEmpty(strColumnFilter))
            {
                throw new ArgumentNullException("strColumnFilter");
            }

            ArrayList alColumn = new ArrayList();//表所有列集合
            Type entityType = model.GetType();
            TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];
            String strIndexName = String.Empty;//索引列名
            String strSql = " WHERE 1=1 " + strRowFilter;//SQL语句
            DbCommand dcmd = db.DbProviderFactory.CreateCommand();
            if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
            dcmd.CommandType = CommandType.Text;

            //是否有自定义参数
            if (parameters != null && parameters.Count > 0)
            {
                dcmd.Parameters.AddRange(parameters.ToArray());
            }

            #region 组装SQL
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }

                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;
                    if (objectAttr.IsPrimaryKey && String.IsNullOrEmpty(strIndexName))//获取索引列名
                    {
                        strIndexName = objectAttr.Storage;
                    }
                    alColumn.Add(objectAttr.Name);
                    object oval = info.GetValue(model, null);
                    if (oval != null && !Convert.IsDBNull(oval))
                    {
                        if (objectAttr.DbType.Equals("datetime"))//日期按天查询
                        {
                            String strDateTime = DateTime.Parse(oval.ToString()).ToString("yyyy-MM-dd");
                            strSql += " AND " + objectAttr.Name + " BETWEEN CONVERT(datetime, @" + objectAttr.Name + ") AND DATEADD(ms, -3, DATEADD(dd,1,CONVERT(datetime, @" + objectAttr.Name + ")))";
                            db.AddInParameter(dcmd, objectAttr.Name, DbType.String, strDateTime);
                        }
                        else if (NeedFuzzy(objectAttr.DbType))
                        {
                            strSql = strSql + " AND " + objectAttr.Name + " LIKE @" + objectAttr.Name;
                            db.AddInParameter(dcmd, objectAttr.Name, DbType.String, "%" + oval + "%");
                        }
                        else
                        {
                            strSql = strSql + " AND " + objectAttr.Name + " =@" + objectAttr.Name;
                            db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                        }
                    }
                }
            }
            #endregion

            #region 参数合法性判断
            //判断列过滤参数合法性
            if (!strColumnFilter.Equals("*"))
            {
                String[] strColumns = strColumnFilter.Split(',');//列过滤列名
                IfColumnValid(strColumns, alColumn);
            }

            //是否有排序参数
            if (!String.IsNullOrEmpty(strOrderBy))
            {
                String[] strOrderColumns = strOrderBy.ToUpper().Replace("DESC", "").Replace("ASC", "").Trim().Split(',');//列过滤列名
                //判断排序参数合法性
                IfColumnValid(strOrderColumns, alColumn);
            }
            else
            {
                strOrderBy = strIndexName;
            }
            #endregion

            String strCount = "SELECT COUNT(*) FROM " + table.Name + strSql;
            dcmd.CommandText = strCount;
            pager.Total = int.Parse(db.ExecuteScalar(dcmd).ToString());
            pager.PageCount = pager.Total / pager.PageSize;
            if (pager.Total % pager.PageSize != 0)
            {
                pager.PageCount = pager.PageCount + 1;
            }

            //strSql = String.Format(@"SELECT {4} FROM ( SELECT TOP {0} * FROM ( SELECT TOP ( {0}* {1}) * FROM {2} {5} ORDER BY {3} ASC) as a ORDER BY {3} DESC ) as b ORDER BY {6}", pager.PageSize, pager.PageIndex, table.Name, strIndexName, strColumnFilter, strSql, strOrderBy);
            int iStartIndex = (pager.PageIndex - 1) * pager.PageSize;
            int iEndndex = pager.PageIndex * pager.PageSize;

            if (string.IsNullOrEmpty(strOrderBy))
            {
                strOrderBy = "(SELECT 0)";
            }
            strSql = String.Format(@"SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS RowNum,* FROM (SELECT {1} FROM {2} {3} ) a ) b
WHERE RowNum>{4} AND RowNum<={5}", strOrderBy, strColumnFilter, table.Name, strSql, iStartIndex, iEndndex);

            dcmd.CommandText = strSql;
            dcmd.CommandTimeout = 60;//默认30秒

            //是否有外部事务
            if (dbTransaction == null)
            {
                return db.ExecuteReader(dcmd);
            }
            else
            {
                return db.ExecuteReader(dcmd, dbTransaction);
            }
        }
        #endregion

        #region 返回DataTable（支持分页）
        /// <summary>
        /// 模糊查询（支持分页）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="pager">分页实体</param>
        /// <param name="strOrderBy">排序参数，只传入ORDER BY后面的排序值</param>
        /// <param name="strColumnFilter">列过滤参数，只传入SELECT和FROM之间的SQL语句</param>
        /// <param name="strRowFilter">行过滤参数，只传入"WHERE 1=1"后的SQL语句，如" AND cniID>@ID"</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns>返回DataTable</returns>
        public DataTable GetDataTableByLike<T>(T model, ref Pager pager, String strOrderBy = "", String strColumnFilter = "*", String strRowFilter = "", List<DbParameter> parameters = null) where T : class,new()
        {
            return GetDataSetByLike(model, ref pager, strOrderBy, strColumnFilter, strRowFilter, parameters).Tables[0];
        }
        #endregion

        #endregion

        #endregion

        #region 私有方法

        #region 列过滤参数判断
        /// <summary>
        /// 列过滤参数判断
        /// </summary>
        /// <param name="strColumnFilter">列过滤条件</param>
        /// <param name="alColumn">所有列集合</param>
        /// <returns></returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        private void IfColumnValid(String[] strColumnFilter, ArrayList alColumn)
        {
            if (alColumn == null) throw new ArgumentNullException("alColumn");

            //列过滤参数判断
            foreach (String str in strColumnFilter)
            {
                bool bColumnFilter = false;
                foreach (var column in alColumn)
                {
                    if (str.Trim().ToUpper().Equals(column.ToString().ToUpper()))
                    {
                        bColumnFilter = true;
                        break;
                    }
                }
                if (!bColumnFilter)
                {
                    throw new Exception("参数格式有误");
                }
            }
        }

        #endregion

        #region 判断数据类型是否需要模糊查询
        /// <summary>
        /// 判断数据类型是否需要模糊查询
        /// </summary>
        /// <param name="strDbType">数据类型</param>
        /// <returns>是否需要模糊查询</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        private bool NeedFuzzy(string strDbType)
        {
            switch (strDbType.ToLower().Trim().Split('(')[0])
            {
                case "varchar":
                case "varchar2": //O
                case "nvarchar":
                case "nvarchar2": //O
                case "char":
                case "nchar":
                case "text":
                case "ntext":
                case "string":
                case "clob": //O
                case "nclob": //O
                case "rowid": //O
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region 转换实体
        /// <summary>
        /// 转换实体并赋值
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="row"></param>
        /// 创 建 人：余鹏飞
        /// 创建日期：2011-12-5
        /// 修 改 人：王宇（wang_yu5）
        /// 修改日期：2012-2-17
        private void ConvertToEntity<T>(T model, DataRow row)
        {
            //读特性名
            Type type = model.GetType();
            //读取实体属性
            PropertyInfo[] infos = type.GetProperties();
            foreach (PropertyInfo info in infos)
            {
                //读取实体属性的特性
                object[] attributes = info.GetCustomAttributes(true);

                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }
                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;

                    //根据特性属性进行自动匹配
                    if (row.Table.Columns.Contains(objectAttr.Name))
                    {
                        object value = row[objectAttr.Name];
                        if (value == DBNull.Value) continue;

                        #region 将值做转换
                        ////将值做转换 
                        //if (info.PropertyType.Equals(typeof(String)))
                        //{
                        //    value = row[objectAttr.Name].ToString();
                        //}
                        //else if (info.PropertyType.Equals(typeof(int)))
                        //{
                        //    value = Convert.ToInt32(row[objectAttr.Name]);
                        //}
                        //else if (info.PropertyType.Equals(typeof(decimal)))
                        //{
                        //    value = Convert.ToDecimal(row[objectAttr.Name]);
                        //}
                        //else if (info.PropertyType.Equals(typeof(DateTime)))
                        //{
                        //    value = Convert.ToDateTime(row[objectAttr.Name]);
                        //}
                        //else if (info.PropertyType.Equals(typeof(double)))
                        //{
                        //    value = Convert.ToDouble(row[objectAttr.Name]);
                        //}
                        //else if (info.PropertyType.Equals(typeof(bool)))
                        //{
                        //    value = Convert.ToBoolean(row[objectAttr.Name]);
                        //}
                        //else if (info.PropertyType.Equals(typeof(Guid)))
                        //{
                        //    value = Guid.Parse(row[objectAttr.Name].ToString());
                        //}
                        #endregion

                        //利用反射自动将value赋值给obj的相应公共属性 
                        info.SetValue(model, value, null);
                    }
                }
            }
        }
        #endregion

        #region 数据类型映射
        /// <summary>
        /// 数据类型映射
        /// </summary>
        /// <param name="strDbType">数据库数据类型</param>
        /// <returns>数据类型</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        private static DbType GetDbType(string strDbType)
        {
            switch (strDbType.ToLower().Trim().Split('(')[0])
            {
                case "varchar":
                case "varchar2"://O
                case "nvarchar":
                case "nvarchar2"://O
                case "char":
                case "nchar":
                case "text":
                case "ntext":
                case "string":
                case "clob"://O
                case "nclob"://O
                case "rowid"://O
                    return DbType.String;
                case "xml":
                    return DbType.Xml;

                case "date":
                case "datetime":
                case "smalldatetime":
                //case "timestamp"://O
                case "timestamp with local time zone":
                case "timestamp with time zone":
                    return DbType.DateTime;
                case "datetime2":
                    return DbType.DateTime2;
                case "datetimeoffset":
                case "interval day to second":
                    return DbType.DateTimeOffset;
                case "time":
                    return DbType.Time;

                case "smallint":
                    return DbType.Byte;
                case "tinyint":
                    return DbType.Int16;
                case "int":
                case "interval year to month":
                    return DbType.Int32;
                case "bigint":
                    return DbType.Int64;
                case "number":
                case "numeric":
                case "decimal":
                case "money":
                case "smallmoney":
                case "integer"://O
                case "unsigned integer"://O
                    return DbType.Decimal;
                case "float":
                    return DbType.Double;//Decimal(O)
                case "real":
                    return DbType.Single;

                case "bit":
                case "bool":
                    return DbType.Boolean;

                case "binary":
                case "varbinary":
                case "image":
                case "raw"://O
                case "long"://O
                case "long raw"://O
                case "blob"://O
                case "bfile"://O
                case "byte[]":
                case "rowversion":
                case "timestamp":
                    return DbType.Binary;

                case "uniqueidentifier":
                case "guid":
                    return DbType.Guid;
                case "sql_variant":
                    return DbType.Object;
                default:
                    return DbType.String;
            }
        }
        #endregion

        #region 判断是否是比较操作符
        /// <summary>
        /// 判断是否是比较操作符
        /// </summary>
        /// <param name="strOperator">比较操作符</param>
        /// <returns>是否是比较操作符</returns>
        /// 创 建 人：王宇（wang_yu5）
        /// 创建日期：2012-2-17
        /// 修 改 人：
        /// 修改日期：
        private bool IsOperator(string strOperator)
        {
            switch (strOperator)
            {
                case "=":
                case "!=":
                case "^=":
                case "<>":
                case "<":
                case ">":
                case "<=":
                case ">=":
                case "LIKE":
                case "like":
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #endregion

        #region 添加

        #region 添加数据，返回主键
        /// <summary>
        /// 添加数据，返回主键
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <returns>返回主键值</returns>
        public String Add<T>(T model) where T : class,new()
        {
            Type entityType = model.GetType();
            TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];
            String strSql = "INSERT INTO " + table.Name + " (";
            String strPara = " VALUES(";
            //bool bIsDbGenerated = false;//是否数据库生成
            DbCommand dcmd = db.DbProviderFactory.CreateCommand();
            if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
            dcmd.CommandType = CommandType.Text;

            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }
                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;

                    //if (objectAttr.IsPrimaryKey && objectAttr.IsDbGenerated)
                    if (objectAttr.IsDbGenerated)//数据库生成字段跳过
                    {
                        //bIsDbGenerated = true;
                        continue;
                    }
                    object oval = info.GetValue(model, null);
                    if (oval == null || Convert.IsDBNull(oval))
                    {
                        if (!objectAttr.CanBeNull && GetDbType(objectAttr.DbType) == DbType.Guid)//Guid不允许为空时框架自动生成  //原判断主键
                        {
                            oval = Guid.NewGuid();
                        }
                        else
                        {
                            continue;
                        }
                    }

                    strSql += objectAttr.Name + ",";
                    strPara += "@" + objectAttr.Name + ",";

                    db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                    //DbParameter para = new SqlParameter("@" + objectAttr.Name, oval);
                    //paramerList.Add(para);
                }
            }

            strPara = strPara.Substring(0, strPara.Length - 1) + ");SELECT @@IDENTITY;";
            strSql = strSql.Substring(0, strSql.Length - 1) + ") " + strPara;

            //if (!bIsDbGenerated)
            //{
            //    strSql += strPara + ")";
            //}
            //else
            //{
            //    strSql += strPara + ");SELECT @@IDENTITY;";//;if columnproperty(object_id('" + table.Name + "'),'cnuID','IsIdentity')=1 
            //}

            dcmd.CommandText = strSql;

            object ob;
            //是否有外部事务
            if (dbTransaction == null)
            {
                ob = db.ExecuteScalar(dcmd);
            }
            else
            {
                //dcmd.Connection = dbTransaction.Connection;//可以不赋值
                //dcmd.Transaction = dbTransaction;
                ob = db.ExecuteScalar(dcmd, dbTransaction);
            }

            return (ob == null) ? null : ob.ToString();
        }
        #endregion

        #region 批量添加，需要事务处理
        /// <summary>
        /// 批量添加，需要事务处理
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="modelList">数据实体集合</param>
        /// <returns>返回操作成功与否</returns>
        public bool AddList<T>(IList<T> modelList) where T : class,new()
        {
            //是否有外部事务调用，默认有
            bool bOutTrans = true;
            if (dbTransaction == null)
            {
                bOutTrans = false;
                dbConnection = db.CreateConnection();
                dbConnection.Open();
                dbTransaction = dbConnection.BeginTransaction();
            }
            try
            {
                //逐个添加
                foreach (T model in modelList)
                {
                    AddReturnBool(model);
                }

                if (!bOutTrans)//没有外部事务调用
                {
                    dbTransaction.Commit();
                }

                return true;
            }
            catch
            {
                if (!bOutTrans)//没有外部事务调用
                {
                    dbTransaction.Rollback();
                    return false;
                }
                throw;
            }
            finally
            {
                if (!bOutTrans)//没有外部事务调用
                {
                    dbConnection.Close();
                }
            }
        }
        #endregion

        #region 添加数据返回bool
        /// <summary>
        /// 添加数据返回bool
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <returns>返回操作成功与否</returns>
        public bool AddReturnBool<T>(T model) where T : class,new()
        {
            Type entityType = model.GetType();
            TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];
            String strSql = "INSERT INTO " + table.Name + " (";
            String strPara = " VALUES(";
            DbCommand dcmd = db.DbProviderFactory.CreateCommand();
            if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
            dcmd.CommandType = CommandType.Text;

            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }
                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;

                    if (objectAttr.IsDbGenerated)//数据库生成字段跳过
                    {
                        continue;
                    }

                    object oval = info.GetValue(model, null);
                    if (oval == null || Convert.IsDBNull(oval))
                    {
                        if (objectAttr.IsPrimaryKey && GetDbType(objectAttr.DbType) == DbType.Guid)//Guid主键框架自动生成
                        {
                            oval = Guid.NewGuid();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    strSql += objectAttr.Name + ",";
                    strPara += "@" + objectAttr.Name + ",";

                    db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                }
            }

            strPara = strPara.Substring(0, strPara.Length - 1) + ") ";
            strSql = strSql.Substring(0, strSql.Length - 1) + ") " + strPara;

            dcmd.CommandText = strSql;

            int iReturn;
            //是否有外部事务
            if (dbTransaction == null)
            {
                iReturn = db.ExecuteNonQuery(dcmd);
            }
            else
            {
                iReturn = db.ExecuteNonQuery(dcmd, dbTransaction);
            }

            return iReturn > 0;
        }
        #endregion

        #region 批量添加，需要事务处理（需优化）
        /// <summary>
        /// 批量添加，需要事务处理
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="modelList">数据实体集合</param>
        /// <returns>返回操作成功与否</returns>
        private bool AddListNew<T>(IList<T> modelList) where T : class,new()
        {
            if (modelList == null || modelList.Count < 1)
            {
                return true;
            }

            //是否有外部事务调用，默认有
            bool bOutTrans = true;
            if (dbTransaction == null)
            {
                bOutTrans = false;
                dbConnection = db.CreateConnection();
                dbConnection.Open();
                dbTransaction = dbConnection.BeginTransaction();
            }
            try
            {
                T model = modelList[0];
                Type entityType = model.GetType();
                TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];
                String strSql = "INSERT INTO " + table.Name + " (";
                String strPara = " VALUES(";
                DbCommand dcmd = db.DbProviderFactory.CreateCommand();
                if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
                dcmd.CommandType = CommandType.Text;

                PropertyInfo[] propertyInfos = entityType.GetProperties();

                #region 拼接SQL
                foreach (PropertyInfo info in propertyInfos)
                {
                    object[] attributes = info.GetCustomAttributes(true);
                    //仅获取列特性
                    foreach (var obAttr in attributes)
                    {
                        if (!(obAttr is ColumnAttribute))
                        {
                            continue;
                        }
                        ColumnAttribute objectAttr = obAttr as ColumnAttribute;

                        if (objectAttr.IsDbGenerated)//数据库生成字段跳过
                        {
                            continue;
                        }

                        strSql += objectAttr.Name + ",";
                        strPara += "@" + objectAttr.Name + ",";
                    }
                }
                strPara = strPara.Substring(0, strPara.Length - 1) + ") ";
                strSql = strSql.Substring(0, strSql.Length - 1) + ") " + strPara;
                dcmd.CommandText = strSql;
                #endregion

                #region 逐个添加
                int iReturn = 0;

                foreach (T modelItem in modelList)
                {
                    foreach (PropertyInfo info in propertyInfos)
                    {
                        object[] attributes = info.GetCustomAttributes(true);

                        //仅获取列特性
                        foreach (var obAttr in attributes)
                        {
                            if (!(obAttr is ColumnAttribute))
                            {
                                continue;
                            }
                            ColumnAttribute objectAttr = obAttr as ColumnAttribute;

                            object oval = info.GetValue(modelItem, null);
                            if (oval == null || Convert.IsDBNull(oval))
                            {
                                if (objectAttr.IsPrimaryKey && GetDbType(objectAttr.DbType) == DbType.Guid) //Guid主键框架自动生成
                                {
                                    oval = Guid.NewGuid();
                                }
                            }

                            db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                        }
                    }

                    //是否有外部事务
                    if (dbTransaction == null)
                    {
                        iReturn = db.ExecuteNonQuery(dcmd);
                    }
                    else
                    {
                        iReturn = db.ExecuteNonQuery(dcmd, dbTransaction);
                    }
                    dcmd.Parameters.Clear();

                }
                #endregion

                if (!bOutTrans)//没有外部事务调用
                {
                    dbTransaction.Commit();
                }

                return iReturn > 0;
            }
            catch
            {
                if (!bOutTrans)//没有外部事务调用
                {
                    dbTransaction.Rollback();
                    return false;
                }
                throw;
            }
            finally
            {
                if (!bOutTrans)//没有外部事务调用
                {
                    dbConnection.Close();
                }
            }

        }
        #endregion

        #endregion

        #region 更新

        #region 更新记录
        /// <summary>
        /// 更新记录 （默认更新条件为主键）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="conditionParameters">条件参数，参数名称请与数据库列名一致</param>
        /// <returns>返回操作成功与否</returns>
        public bool Update<T>(T model, DbParameter[] conditionParameters = null) where T : class,new()
        {
            //try
            //{
            Type entityType = model.GetType();
            TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];
            String strSql = "UPDATE " + table.Name + " SET ";

            DbCommand dcmd = db.DbProviderFactory.CreateCommand();
            if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
            dcmd.CommandType = CommandType.Text;

            String strCondition = "";
            //是否有自定义条件
            if (conditionParameters != null)
            {
                foreach (var parameter in conditionParameters)
                {
                    if (parameter.Value != null && !Convert.IsDBNull(parameter.Value))
                    {
                        //strCondition += String.Format(" AND {0}={1}", parameter.ParameterName, parameter.Value);//String.Format效率问题
                        strCondition += " AND " + parameter.ParameterName + "=@" + parameter.ParameterName;
                        dcmd.Parameters.Add(parameter);
                    }
                }
            }

            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }
                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;

                    object oval = info.GetValue(model, null);
                    if (oval == null || Convert.IsDBNull(oval))
                    {
                        continue;
                    }

                    //根据主键更新
                    if (conditionParameters == null && objectAttr.IsPrimaryKey)
                    {
                        strCondition += " AND " + objectAttr.Name + "=@" + objectAttr.Name;
                        db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);

                        continue;
                    }
                    if (!objectAttr.CanBeNull && String.IsNullOrEmpty(oval.ToString()))
                    {
                        throw new Exception("更新操作失败，因为非空字段" + objectAttr.Name + "不能赋值为空字符串。");
                    }
                    strSql += objectAttr.Name + "=@" + objectAttr.Name + ",";
                    db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                }
            }
            strSql = strSql.Substring(0, strSql.Length - 1);
            strSql += " WHERE 1=1 " + strCondition;//更新条件

            dcmd.CommandText = strSql;

            //是否有外部事务
            if (dbTransaction == null)
            {
                db.ExecuteScalar(dcmd);
            }
            else
            {
                db.ExecuteScalar(dcmd, dbTransaction);
            }
            return true;
            //}
            //catch
            //{
            //    //是否有外部事务
            //    if (dbTransaction != null)
            //    {
            //        throw;
            //    }

            //    return false;
            //}
        }
        #endregion

        #region 批量更新
        /// <summary>
        /// 批量更新（默认更新条件为主键）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="modelList">数据实体集合</param>
        /// <returns></returns>
        public bool UpdateList<T>(IList<T> modelList) where T : class,new()
        {
            //是否有外部事务调用，默认有
            bool bOutTrans = true;
            if (dbTransaction == null)
            {
                bOutTrans = false;
                dbConnection = db.CreateConnection();
                dbConnection.Open();
                dbTransaction = dbConnection.BeginTransaction();
            }

            try
            {
                foreach (T model in modelList)
                {
                    Update(model);
                }

                if (!bOutTrans)//没有外部事务调用
                {
                    dbTransaction.Commit();
                }
                return true;
            }
            catch
            {
                if (!bOutTrans)//没有外部事务调用
                {
                    dbTransaction.Rollback();
                }

                return false;
            }
            finally
            {
                if (!bOutTrans)//没有外部事务调用
                {
                    dbConnection.Close();
                }
            }
        }
        #endregion

        #region 批量更新（自定义条件更新）
        /// <summary>
        /// 批量更新（自定义条件更新）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="dicModel_Parameter">数据实体、更新条件键值集合</param>
        /// <returns></returns>
        public bool UpdateList<T>(Dictionary<T, DbParameter[]> dicModel_Parameter) where T : class,new()
        {
            //是否有外部事务调用，默认有
            bool bOutTrans = true;
            if (dbTransaction == null)
            {
                bOutTrans = false;
                dbConnection = db.CreateConnection();
                dbConnection.Open();
                dbTransaction = dbConnection.BeginTransaction();
            }

            try
            {
                foreach (KeyValuePair<T, DbParameter[]> model_Parameter in dicModel_Parameter)
                {
                    Update(model_Parameter.Key, model_Parameter.Value);
                }

                if (!bOutTrans)//没有外部事务调用
                {
                    dbTransaction.Commit();
                }
                return true;
            }
            catch
            {
                if (!bOutTrans)//没有外部事务调用
                {
                    dbTransaction.Rollback();
                }

                return false;
            }
            finally
            {
                if (!bOutTrans)//没有外部事务调用
                {
                    dbConnection.Close();
                }
            }
        }
        #endregion

        #region 获取主键值
        /// <summary>
        /// 获取主键值
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <returns>返回主键名称，值</returns>
        public Dictionary<string, object> GetPrimaryKey<T>(T model) where T : class,new()
        {
            Dictionary<string, object> primaryKeys = new Dictionary<string, object>();
            PropertyInfo[] propertyInfos = model.GetType().GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object[] attributes = info.GetCustomAttributes(true);
                //仅获取列特性
                foreach (var obAttr in attributes)
                {
                    if (!(obAttr is ColumnAttribute))
                    {
                        continue;
                    }
                    ColumnAttribute objectAttr = obAttr as ColumnAttribute;
                    if (objectAttr.IsPrimaryKey)
                    {
                        primaryKeys.Add(objectAttr.Name, info.GetValue(model, null));
                    }
                }
            }

            return primaryKeys;
        }
        #endregion

        #endregion

        #region 删除

        #region 删除记录
        /// <summary>
        /// 删除数据（默认删除条件为主键）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="model">数据实体</param>
        /// <param name="conditionParameters">条件参数，参数名称请与数据库列名一致</param>
        /// <returns>返回操作成功与否</returns>
        public bool Delete<T>(T model, DbParameter[] conditionParameters = null) where T : class,new()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM ");
                Type entityType = model.GetType();
                TableAttribute table = (TableAttribute)entityType.GetCustomAttributes(false)[0];
                sb.Append(table.Name);
                sb.Append(" WHERE 1=1");
                DbCommand dcmd = db.DbProviderFactory.CreateCommand();
                if (dcmd == null) throw new NullReferenceException("实例化DbCommand失败");
                dcmd.CommandType = CommandType.Text;

                //是否有自定义条件
                if (conditionParameters != null)
                {
                    foreach (var parameter in conditionParameters)
                    {
                        if (parameter.Value != null && !Convert.IsDBNull(parameter.Value))
                        {
                            //strCondition += String.Format(" AND {0}={1}", parameter.ParameterName, parameter.Value);//String.Format效率问题
                            sb.Append(" AND " + parameter.ParameterName + "=@" + parameter.ParameterName);
                            dcmd.Parameters.Add(parameter);
                        }
                    }
                }

                PropertyInfo[] propertyInfos = entityType.GetProperties();
                foreach (PropertyInfo info in propertyInfos)
                {
                    object[] attributes = info.GetCustomAttributes(true);
                    //仅获取列特性
                    foreach (var obAttr in attributes)
                    {
                        if (!(obAttr is ColumnAttribute))
                        {
                            continue;
                        }
                        ColumnAttribute objectAttr = obAttr as ColumnAttribute;

                        //按主键删除
                        if (conditionParameters == null && objectAttr.IsPrimaryKey)
                        {
                            object oval = info.GetValue(model, null);
                            if (oval == null || Convert.IsDBNull(oval))
                            {
                                //主键为空则抛出异常
                                throw new ArgumentNullException(objectAttr.Name);
                                //continue;
                            }

                            sb.Append(" AND " + objectAttr.Name + "=@" + objectAttr.Name);
                            db.AddInParameter(dcmd, objectAttr.Name, GetDbType(objectAttr.DbType), oval);
                        }
                    }

                }

                dcmd.CommandText = sb.ToString();

                //是否有外部事务
                if (dbTransaction == null)
                {
                    db.ExecuteScalar(dcmd);
                }
                else
                {
                    db.ExecuteScalar(dcmd, dbTransaction);
                }
                return true;
            }
            catch
            {
                //是否有外部事务
                if (dbTransaction != null)
                {
                    throw;
                }
                return false;
            }
        }
        #endregion

        #region 批量删除
        /// <summary>
        /// 批量删除（默认删除条件为主键）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="modelList">数据实体集合</param>
        /// <returns>返回操作成功与否</returns>
        public bool DeleteList<T>(IList<T> modelList) where T : class,new()
        {
            //是否有外部事务调用，默认有
            bool bOutTrans = true;
            if (dbTransaction == null)
            {
                bOutTrans = false;
                dbConnection = db.CreateConnection();
                dbConnection.Open();
                dbTransaction = dbConnection.BeginTransaction();
            }

            try
            {
                //逐个删除
                foreach (T model in modelList)
                {
                    Delete(model);
                }

                if (!bOutTrans)//没有外部事务调用
                {
                    dbTransaction.Commit();
                }

                return true;
            }
            catch
            {
                if (!bOutTrans)//没有外部事务调用
                {
                    dbTransaction.Rollback();
                }

                return false;
            }
            finally
            {
                if (!bOutTrans)//没有外部事务调用
                {
                    dbConnection.Close();
                }
            }
        }
        #endregion

        #region 批量删除（自定义条件删除）
        /// <summary>
        /// 批量删除（自定义条件删除）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="dicModel_Parameter">数据实体、更新条件键值集合</param>
        /// <returns>返回操作成功与否</returns>
        public bool DeleteList<T>(Dictionary<T, DbParameter[]> dicModel_Parameter) where T : class,new()
        {
            //是否有外部事务调用，默认有
            bool bOutTrans = true;
            if (dbTransaction == null)
            {
                bOutTrans = false;
                dbConnection = db.CreateConnection();
                dbConnection.Open();
                dbTransaction = dbConnection.BeginTransaction();
            }

            try
            {
                foreach (KeyValuePair<T, DbParameter[]> model_Parameter in dicModel_Parameter)
                {
                    Delete(model_Parameter.Key, model_Parameter.Value);
                }

                if (!bOutTrans)//没有外部事务调用
                {
                    dbTransaction.Commit();
                }

                return true;
            }
            catch
            {
                if (!bOutTrans)//没有外部事务调用
                {
                    dbTransaction.Rollback();
                }

                return false;
            }
            finally
            {
                if (!bOutTrans)//没有外部事务调用
                {
                    dbConnection.Close();
                }
            }
        }
        #endregion

        #endregion
    }
}
