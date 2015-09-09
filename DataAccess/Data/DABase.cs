namespace HNAS.Framework4.Data
{
    /// <summary>
    /// DataAccess层基类
    /// </summary>
    /// Copyright (c) 2011 海航航空信息系统有限公司
    /// 创 建 人：王宇（wang_yu5）
    /// 创建日期：2011-12-5
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    public abstract class DABase
    {
        /// <summary>
        /// 构造函数，创建数据库实例
        /// </summary>
        /// <param name="strDBName">数据库连接配置名</param>
        /// <param name="dataAccess">数据访问接口</param>
        public DABase(string strDBName = "", DataAccess dataAccess = null)
        {
            //IUnityContainer container = new UnityContainer();
            //container.RegisterType<IDataAccess, DataAccess>(strDBName);

            //DataAccess = container.Resolve<IDataAccess>();
            if (dataAccess == null)
            {
                DataAccess = new DataAccess(strDBName);
            }
            else
            {
                DataAccess = dataAccess;
            }
        }

        /// <summary>
        /// 数据访问接口
        /// </summary>
        protected DataAccess DataAccess { set; get; }
    }
}
