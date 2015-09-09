using System;

namespace HNAS.Framework4.BLBase
{
    /// <summary>
    /// 返回值调用的方法
    /// </summary>
    /// <remarks>
    /// 该类用于在BusinessLogic项目的方法调用者提供一致的返回值。
    /// </remarks>
    /// <typeparam name="T">实例类型</typeparam>
    public class CallResult<T>
    {
        /// <summary>
        /// 异常
        /// </summary>
        private Exception _error;

        /// <summary>
        ///错误类型
        /// </summary>
        private CallErrorType _errorType = CallErrorType.None;

        /// <summary>
        /// 返回消息
        /// </summary>
        private string _message = string.Empty;

        /// <summary>
        /// the code of the return message of the method
        /// </summary>
        /// <remarks>
        /// Some callers may have realize a message mechanism
        /// which provide a more detailed message about
        /// the return message, in this case, the CallResult needs only
        /// to return the code of the message.
        /// </remarks>
        private string _messageCode = string.Empty;

        /// <summary>
        /// 返回值
        /// </summary>
        private T _result;

        /// <summary>
        /// Initialize an instance of CallResult.
        /// with the return result
        /// </summary>
        /// <remarks>
        /// This constructor will return an instance of CallResult.
        /// indicating the method has runned succefully
        /// </remarks>
        /// <param name="result">the returned value of the method</param>
        public CallResult(T result)
        {
            this._result = result;
        }

        /// <summary>
        /// Initialize an instance of CallResult.
        /// with the return result
        /// </summary>
        /// <remarks>
        /// This constructor will return an instance of CallResult.
        /// indicating the method has runned succefully
        /// </remarks>
        /// <param name="result">the returned value of the method</param>
        /// <param name="message">the returned message of the method</param>
        public CallResult(T result, string message)
        {
            this._result = result;

            StringIsNotEmpty("message", message);
            this._message = message;
        }

        /// <summary>
        /// Initialize an instance of CallResult.
        /// </summary>
        public CallResult()
        {
            // do nothing
        }

        /// <summary>
        /// If there is any error occurs during the operation
        /// </summary>
        public bool HasError
        {
            get
            {
                return this._errorType != CallErrorType.None;
            }
        }

        /// <summary>
        /// Gets or sets the returned message of the method
        /// </summary>
        /// <exception cref="ArgumentException">the returned message must be specified for get_Message</exception>
        public string Message
        {
            get
            {
                return this._message;
            }
            set
            {
                StringIsNotEmpty("Message", value);
                this._message = value;
            }
        }

        /// <summary>
        /// Gets or sets the returned message of the method
        /// </summary>
        public T Result
        {
            get
            {
                return this._result;
            }
            set
            {
                this._result = value;
            }
        }

        /// <summary>
        /// Gets or sets the code of the returned message
        /// </summary>
        /// <exception cref="ArgumentException">the returned message must be specified for get_MessageCode</exception>
        public string MessageCode
        {
            get
            {
                return this._messageCode;
            }
            set
            {
                StringIsNotEmpty("MessageCode", value);
                this._messageCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the error occurs during the method
        /// </summary>
        public Exception Error
        {
            get
            {
                return this._error;
            }
            set
            {
                this._error = value;

                if (value != null && this._errorType == CallErrorType.None)
                {
                    this._errorType = CallErrorType.General;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the error that occurs during the method
        /// </summary>
        public CallErrorType ErrorType
        {
            get
            {
                return this._errorType;
            }
            set
            {
                this._errorType = value;
            }
        }

        /// <summary>
        /// Assert if the string argument is not null and not empty
        /// </summary>
        /// <exception cref="ArgumentException">If the string argument is null or empty</exception>
        /// <param name="argumentName">the name of the argument</param>
        /// <param name="argumentValue">the value of the argument</param>
        private static void StringIsNotEmpty(string argumentName, string argumentValue)
        {
            if (string.IsNullOrEmpty(argumentValue))
            {
                string message = string.Format("the {0} must be specified", argumentName);
                throw new ArgumentException(argumentName, message);
            }
        }
    }

    /// <summary>
    /// Enumeration for the type of error occurs during the operation
    /// </summary>
    public enum CallErrorType
    {
        /// <summary>
        /// It's ok
        /// </summary>
        None,

        /// <summary>
        /// A general error
        /// </summary>
        General,

        /// <summary>
        /// A system level or fatal error
        /// </summary>
        System,

        /// <summary>
        /// An error associated with the breaking of some business rules
        /// </summary>
        Business
    };
}
