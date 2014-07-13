
namespace Anycmd.AC.Infra
{
    using System;

    /// <summary>
    /// 操作帮助
    /// </summary>
    public interface IOperationHelp
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        string Content { get; set; }
    }
}
