
namespace Anycmd.EDI.ViewModels.NodeViewModels
{
    using Host.EDI.Handlers;
    using Model;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public partial class NodeInfo : Dictionary<string, object>
    {
        public NodeInfo() { }

        public NodeInfo(DicReader dic)
        {
            if (dic == null)
            {
                throw new ArgumentNullException("dic");
            }
            foreach (var item in dic)
            {
                this.Add(item.Key, item.Value);
            }
            if (!this.ContainsKey("TransferName"))
            {
                string transferName = string.Empty;
                IMessageTransfer transfer;
                if (dic.Host.Transfers.TryGetTransfer((Guid)this["TransferID"], out transfer))
                {
                    transferName = transfer.Name; ;
                }
                this.Add("TransferName", transferName);
            }
            if (!this.ContainsKey("DeletionStateName"))
            {
                this.Add("DeletionStateName", dic.Host.Translate("EDI", "Node", "DeletionStateName", (int)this["DeletionStateCode"]));
            }
            if (!this.ContainsKey("IsEnabledName"))
            {
                this.Add("IsEnabledName", dic.Host.Translate("EDI", "Node", "IsEnabledName", (int)this["IsEnabled"]));
            }
        }
    }
}
