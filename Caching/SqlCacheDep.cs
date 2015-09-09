using System.Web;
using System.Web.Caching;

namespace HNAS.Framework4.Caching
{
    /// <summary>
    /// sql缓存依赖
    /// </summary>
    ///  创 建 人：王好
    ///  创建日期：2011年12月14日
    ///  修 改 人：
    ///  修改日期：
    ///  Copyright (c) 2011 海南海航航空信息系统有限公司
    ///  版 本：1.0	
    public class SqlCacheDep : CacheDep
    {
        /// <summary>
        /// 默认数据库连接
        /// </summary>
        protected string dataBaseconstr = "";

        #region 构造函数
        /// <summary>
        /// 默认构造
        /// </summary>
        public SqlCacheDep()
            : base()
        {

        }

        /// <summary>
        /// 指定数据库连接字符串构造函数
        /// </summary>
        /// <param name="str"> 指定数据库连接字符</param>
        public SqlCacheDep(string str)
        {
            this.dataBaseconstr = str;
        }
        #endregion

        #region 设置缓存
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="CacheKey">索引值</param>
        /// <param name="data">缓存的对象</param>
        /// <param name="configKey">依赖key逗号隔开</param>
        /// <param name="cacheDatabaseName">缓存依赖对应的数据库连接,默认值SqlCacheDepDB</param>  
        /// <returns>缓存的对象</returns>

        public void SetSqlCacheData(string CacheKey, object data, string configKey, string cacheDatabaseName = "CacheDepDB")
        {

            object objModel = data;//把当前数据进行缓存

            if (objModel != null)
            {

                GetDependency(cacheDatabaseName, configKey);

                SetCache(CacheKey, objModel, dependency);//写入缓存   
            }

        }

        /// <summary>
        /// 设置以缓存依赖的方式缓存数据
        /// </summary>
        /// <param name="CacheKey">索引键值</param>
        /// <param name="objObject">缓存对象</param>
        /// <param name="dep">依赖对象</param>
        protected void SetCache(string CacheKey, object objObject, System.Web.Caching.AggregateCacheDependency dep)
        {

            System.Web.Caching.Cache objCache = HttpRuntime.Cache;

            objCache.Insert(

                CacheKey,

                objObject,

                dep,

                System.Web.Caching.Cache.NoAbsoluteExpiration,//从不过期

                System.Web.Caching.Cache.NoSlidingExpiration,//禁用可调过期

                System.Web.Caching.CacheItemPriority.Default,

                null);

        }
        #endregion

        #region 获取依赖项
        /// <summary>
        /// 获取依赖项
        /// </summary>
        /// <param name="cacheDatabaseName"></param>
        /// <param name="configKey"></param>
        protected void GetDependency(string cacheDatabaseName, string configKey)
        {

            string[] tables = configKey.Split(configurationSeparator);

            foreach (string tableName in tables)
                dependency.Add(new SqlCacheDependency(cacheDatabaseName, tableName));

        }
        #endregion

        /// <summary>
        /// SQL 2005 //推荐将这段代码加到Global.asax的Application_Start方法中
        /// </summary>
        /// <param name="connectionStr"></param>
        public static void SqlDependencyStart(string connectionStr = "")
        {
            System.Data.SqlClient.SqlDependency.Start(connectionStr);
        }

        /// <summary>
        /// 推荐将这段代码加到Global.asax的Application_End方法中，
        /// </summary>
        /// <param name="connectionStr"></param>
        public static void SqlDependencyStop(string connectionStr = "")
        {
            System.Data.SqlClient.SqlDependency.Stop(connectionStr);
        }


        #region 利用SqlCacheDependencyAdmin类 管理数据库对SqlCacheDependency特性

        /// <summary>
        ///  为特定数据库启用SqlCacheDependency对象更改通知
        /// </summary>
        /// <param name="connectionStr"></param>
        public void EnableNotifications(string connectionStr = "")
        {
            SqlCacheDependencyAdmin.EnableNotifications(GetDataBaseconstr(connectionStr));
        }

        /// <summary>
        ///  为特定数据库禁用 SqlCacheDependency对象更改通知
        /// </summary>
        /// <param name="connectionStr"></param>
        public void DisableNotifications(string connectionStr = "")
        {

            SqlCacheDependencyAdmin.DisableNotifications(GetDataBaseconstr(connectionStr));
        }

        /// <summary>
        /// 为数据库中的特定表启用SqlCacheDependency对象更改通知
        /// </summary>
        /// <param name="tables">指定表</param>
        /// <param name="connectionStr"></param>
        public void EnableTableForNotifications(string tables, string connectionStr = "")
        {

            GetDataBaseconstr(connectionStr);

            string[] table = tables.Split(configurationSeparator);

            foreach (string tableName in table)
            {
                SqlCacheDependencyAdmin.EnableTableForNotifications(this.dataBaseconstr, tableName);
            }

        }


        /// <summary>
        ///  为数据库中的特定表禁用SqlCacheDependency对象更改通知
        /// </summary>
        ///<param name="tables">指定表</param>
        /// <param name="connectionStr"></param>
        public void DisableTableForNotifications(string tables, string connectionStr = "")
        {
            GetDataBaseconstr(connectionStr);

            string[] table = tables.Split(configurationSeparator);

            foreach (string tableName in table)
            {
                SqlCacheDependencyAdmin.EnableTableForNotifications(this.dataBaseconstr, tableName);
            }

        }

        /// <summary>
        ///   返回启用了SqlCacheDependency对象更改通知的所有表的列表
        /// </summary>
        /// <param name="connectionStr"></param>
        public string[] GetTablesEnabledForNotifications(string connectionStr = "")
        {

            return SqlCacheDependencyAdmin.GetTablesEnabledForNotifications(GetDataBaseconstr(connectionStr));

        }

        /// <summary>
        /// 获得连接字符串
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <returns></returns>
        protected string GetDataBaseconstr(string connectionStr = "")
        {
            //如果传入为空，则调用构造函数缺省的
            if (!string.IsNullOrEmpty(connectionStr))
            {
                this.dataBaseconstr = connectionStr;
            }
            return this.dataBaseconstr;
        }
        #endregion
    }
}