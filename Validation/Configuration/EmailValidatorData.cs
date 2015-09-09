//===============================================================================
// HNAS Enterprise Library
// Validation Application Block
//===============================================================================
// Copyright © wang_yu5 HNAS.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Configuration;
using HNAS.Framework4.Validation.Validators;

namespace HNAS.Framework4.Validation.Configuration
{
	/// <summary>
    /// Configuration object to describe an instance of class <see cref="EmailValidatorData"/>.
	/// </summary>
    /// Copyright (c) 2011 海航航空信息系统有限公司
    /// 创 建 人：王宇(wang_yu5)
    /// 创建日期：2011-12-5
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    [ResourceDescription(typeof(DesignResources), "EmailValidatorDataDescription")]
    [ResourceDisplayName(typeof(DesignResources), "EmailValidatorDataDisplayName")]
    public class EmailValidatorData : ValueValidatorData
    {
        /// <summary>
        /// Email验证配置类
        /// </summary>
        public EmailValidatorData()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        public EmailValidatorData(string name)
            : base(name, typeof(EmailValidator))
        {
        }
        /// <summary>
        /// 创建Email验证器
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new EmailValidator(this.Tag);
        }
    }
}
