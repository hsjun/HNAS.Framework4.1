//===============================================================================
// HNAS Validation Application Block
//===============================================================================
// Copyright © wang_yu5 HNAS.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System.Globalization;
using System.Text.RegularExpressions;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;

using HNAS.Framework4.Validation.Properties;
using HNAS.Framework4.Validation.Configuration;

namespace HNAS.Framework4.Validation.Validators
{
    /// <summary>
    /// Email 验证器.
    /// </summary>
    /// Copyright (c) 2011 海航航空信息系统有限公司
    /// 创 建 人：王宇(wang_yu5)
    /// 创建日期：2011-12-5
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    [ConfigurationElementType(typeof(EmailValidatorData))]
    public class EmailValidator : Validator<string>
    {
        /// <summary>
        /// <para>Initializes a new instance of the EmalValidator</para>
        /// </summary>
        /// <param name="tag"></param>
        public EmailValidator(string tag)
            : base(string.Empty, tag)
        {
        }

        //Email Pattern
        static Regex emailCaptureRegex =
            new Regex(@"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$");

        /// <summary>
        /// DefaultMessageTemplate
        /// </summary>
        protected override string DefaultMessageTemplate
        {
            get { return Resources.EmailDefaultMessageTemplate; }
        }

        /// <summary>
        /// 应用验证，存入验证结果到validationResults。
        /// </summary>
        /// <param name="objectToValidate"></param>
        /// <param name="currentTarget"></param>
        /// <param name="key"></param>
        /// <param name="validationResults"></param>
        protected override void DoValidate(
            string objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            if (!string.IsNullOrEmpty(objectToValidate))
            {
                Match match = emailCaptureRegex.Match(objectToValidate);
                if (!match.Success)
                {
                    LogValidationResult(
                        validationResults,
                        GetMessage(objectToValidate, key),
                        currentTarget,
                        key);
                }
            }

        }

        /// <summary>
        /// 获取验证失败消息。
        /// Gets the message representing a failed validation.
        /// </summary>
        /// <param name="objectToValidate">The object for which validation was performed.</param>
        /// <param name="key">The key representing the value being validated for <paramref name="objectToValidate"/>.</param>
        /// <returns>The message representing the validation failure.</returns>
        protected override string GetMessage(object objectToValidate, string key)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                this.MessageTemplate,
                objectToValidate,
                key,
                this.Tag);
        }
    }
}