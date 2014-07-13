
namespace Anycmd.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// 表示Anycmd框架抛出的异常。
    /// </summary>
    [Serializable]
    public class CoreException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public CoreException() : base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public CoreException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public CoreException(string message, Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Initializes a new instance of the <c>InfrastructureException</c> class with the specified
        /// string formatter and the arguments that are used for formatting the message which
        /// describes the error.
        /// </summary>
        /// <param name="format">The string formatter which is used for formatting the error message.</param>
        /// <param name="args">The arguments that are used by the formatter to build the error message.</param>
        public CoreException(string format, params object[] args) : base(string.Format(format, args)) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected CoreException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
