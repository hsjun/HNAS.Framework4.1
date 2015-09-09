using System.Web;
using System.Web.Caching;

namespace HNAS.Framework4.Caching
{
    /// <summary>
    /// 缓存数据依赖类
    /// </summary>
    ///  创 建 人：王好
    ///  创建日期：2011年12月14日
    ///  修 改 人：
    ///  修改日期：
    ///  Copyright (c) 2011 海南海航航空信息系统有限公司
    ///  版 本：1.0	
    public class CacheDep
    {
        /// <summary>
        /// //分隔符
        /// </summary>
        protected char[] configurationSeparator = new char[] { ',' };
        /// <summary>
        /// 多项依赖对象
        /// </summary>
        protected AggregateCacheDependency dependency = new AggregateCacheDependency();


        #region 设置文件依赖缓存
        /// <summary>
        /// 设置文件依赖缓存
        /// </summary>
        /// <param name="cacheKey">索引值</param>
        /// <param name="obj">缓存的对象</param>
        /// <param name="fileNames">依赖文件逗号隔开</param>
        public void SetFileCache(string cacheKey, object obj, string fileNames)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;

            GetDependency(fileNames);

            objCache.Insert(cacheKey, obj, dependency);
        }
        #endregion


        #region 获取依赖项
        /// <summary>
        /// 获取依赖项
        /// </summary>
        /// <param name="configKey"></param>
        protected void GetDependency(string configKey)
        {
            string[] configKeys = configKey.Split(configurationSeparator);

            foreach (string key in configKeys)
                dependency.Add(new CacheDependency(key));

        }
        #endregion

        #region 获取当前应用程序指定CacheKey的Cache对象值

        /// <summary>
        ///  获取当前应用程序指定CacheKey的Cache对象值
        /// </summary>
        /// <param name="CacheKey">索引键值</param>
        /// <returns>返回缓存对象</returns>
        public object GetCache(string CacheKey)
        {

            System.Web.Caching.Cache objCache = HttpRuntime.Cache;

            return objCache[CacheKey];

        }

        #endregion
    }
}