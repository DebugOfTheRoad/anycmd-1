
namespace Anycmd.Host.EDI.Handlers {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 命令命令转移器集合
    /// </summary>
    public interface IMessageTransferSet : IEnumerable<IMessageTransfer> {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferID"></param>
        /// <returns></returns>
        IMessageTransfer this[Guid transferID] { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferID"></param>
        /// <param name="sendStrategy"></param>
        /// <returns></returns>
        bool TryGetTransfer(Guid transferID, out IMessageTransfer sendStrategy);
    }
}
