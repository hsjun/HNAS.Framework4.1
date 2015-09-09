using System;

namespace HNAS.Framework4.WebCommon
{
    /// <summary>
    /// 参数判断
    /// </summary>
    /// Copyright (c) 2012 海航航空信息系统有限公司
    /// 创 建 人：王宇
    /// 创建日期：2012-2-20
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0
    public static class ArgumentAssertion
    {
        /// <summary>
        /// 初始化验证
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="targetType"></param>
        public static void IsIntanceOfType(String argumentName, object argumentValue, Type targetType)
        {
            String message = "the " + argumentName + " must be an instance of " + targetType.Name;
            IsIntanceOfType(argumentName, argumentValue, targetType, message);
        }

        /// <summary>
        /// 初始化验证
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="targetType"></param>
        /// <param name="message"></param>
        public static void IsIntanceOfType(String argumentName, object argumentValue, Type targetType, String message)
        {
            IsNotNull(argumentName, argumentValue);
            if (!targetType.IsInstanceOfType(argumentValue))
            {
                throw new ArgumentException(argumentName, message);
            }
        }

        /// <summary>
        /// 非空验证
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="argumentValue"></param>
        public static void IsNotNull(String argumentName, object argumentValue)
        {
            String message = "the " + argumentName + " must be specified.";
            IsNotNull(argumentName, argumentValue, message);
        }

        /// <summary>
        /// 非空验证
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="message"></param>
        public static void IsNotNull(String argumentName, object argumentValue, String message)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName, message);
            }
        }

        /// <summary>
        /// 非空验证
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="argumentValue"></param>
        public static void StringIsNotEmpty(String argumentName, String argumentValue)
        {
            String message = "the " + argumentName + " must be specified.";
            StringIsNotEmpty(argumentName, argumentValue, message);
        }

        /// <summary>
        /// 非空验证
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="message"></param>
        public static void StringIsNotEmpty(String argumentName, String argumentValue, String message)
        {
            if (String.IsNullOrEmpty(argumentValue))
            {
                throw new ArgumentException(argumentName, message);
            }
        }
    }
}
