//===============================================================================
// HNAS Validation Application Block
//===============================================================================
// Copyright © wang_yu5 HNAS.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;

using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace HNAS.Framework4.Validation.Validators
{
    /// <summary>
    /// 用于验证的基类
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// Copyright (c) 2011 海航航空信息系统有限公司
    /// 创 建 人：王宇(wang_yu5)
    /// 创建日期：2011-12-5
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    [Serializable]
    public abstract class BaseClass<T> where T : class
    {
        /// <summary>
        /// 验证结果
        /// </summary>
        public ValidationResults ValidateResults { get; protected set; }

        /// <summary>
        /// 验证结果
        /// </summary>
        public string ValidateTag { get; protected set; }

        /// <summary>
        /// 根据验证配置返回验证结果
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            ValidateResults = Microsoft.Practices.EnterpriseLibrary.Validation.Validation.Validate(this as T);
            if (!ValidateResults.IsValid)
            {
                foreach (var item in ValidateResults)
                {
                    ValidateTag += string.Format(@"\n{0}:{1}", item.Key, item.Message);
                }
                return false;
            }
            return true;
        }
    }
}
