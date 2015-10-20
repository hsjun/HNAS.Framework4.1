using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HNAS.Framework4.Data
{
    public partial class DataAccess
    {
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
        internal String GetTableName<T>(T model) where T : class,new()
        {
            return ((TableAttribute)model.GetType().GetCustomAttributes(false)[0]).Name;
        }
        #endregion


        /// <summary>
        /// 组装SQL
        /// </summary>
        /// <param name="t">实体对象</param>
        /// <param name="dcmd">DbCommand对象</param>
        /// <param name="whereSql">查询条件语句</param>
        /// <param name="columnNameList">数据库列名列表</param>
        /// <param name="primaryKey">主键列，逗号分隔</param>
        /// <param name="fuzzyQuery">模糊查询标记</param>
        /// <remarks></remarks>
        private void WhereStatmentBuilder<T>(T t, ref DbCommand dcmd, 
            ref string whereSql, ref ArrayList columnNameList, ref string primaryKey,
            bool fuzzyQuery = false) 
            where T : class, new()
        {
            Type entityType = t.GetType();

            List<PropertyInfo> pis = entityType.GetProperties().ToList();
            foreach (var pi in pis)
            {
                List<object> columnAttributeList = pi.GetCustomAttributes(typeof(ColumnAttribute), true).ToList();
                foreach (var x in columnAttributeList)
                {
                    var colAttribute = x as ColumnAttribute;
                    columnNameList.Add(colAttribute.Name);

					//主键
                    if (colAttribute.IsPrimaryKey)
                    {
                        primaryKey += ((primaryKey.Length > 0) ? ",":"") + colAttribute.Storage;
                    }

                    object piValue = pi.GetValue(t, null);
                    if (!(piValue == null || Convert.IsDBNull(piValue)))
                    {
                        var dbType = colAttribute.DbType.ToLower();

                        if (dbType.Equals("datetime")) //日期按天查询
                        {
                            DateTime d = DateTime.Parse(piValue.ToString());

                            whereSql += " AND " + colAttribute.Name + " >= @" + colAttribute.Name + "_gt"
                                        + " AND " + colAttribute.Name + " <= @" + colAttribute.Name + "_lt";

                            db.AddInParameter(dcmd, colAttribute.Name + "_gt", DbType.String,
                                d.ToString("yyyy-MM-dd 00:00:00"));
                            db.AddInParameter(dcmd, colAttribute.Name + "_lt", DbType.String,
                                d.ToString("yyyy-MM-dd 23:59:59"));
                        }
                        else
                        {
							//允许模糊查询且字段类型支持模糊查询
                            if (fuzzyQuery && NeedFuzzy(colAttribute.DbType))
                            {
                                whereSql += " AND " + colAttribute.Name + " LIKE @" + colAttribute.Name;

                                db.AddInParameter(dcmd, colAttribute.Name, GetDbType(colAttribute.DbType), "%" + piValue + "%"); 
                            }
                            else
                            {
                                whereSql += " AND " + colAttribute.Name + " = @" + colAttribute.Name;

                                db.AddInParameter(dcmd, colAttribute.Name, GetDbType(colAttribute.DbType), piValue); 
                            }
                        }
                    }

                }
            }
        }


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
    }
}
