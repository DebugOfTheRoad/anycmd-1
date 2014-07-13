
namespace Anycmd.EDI.ViewModels.NodeViewModels
{
    using Host.EDI.Handlers;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public partial class NodeInfo : Dictionary<string, object>
    {
        public NodeInfo() { }

        public NodeInfo(AppHost host, Dictionary<string, object> dic)
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
                if (NodeHost.Instance.Transfers.TryGetTransfer((Guid)this["TransferID"], out transfer))
                {
                    transferName = transfer.Name; ;
                }
                this.Add("TransferName", transferName);
            }
            if (!this.ContainsKey("DeletionStateName"))
            {
                this.Add("DeletionStateName", host.Translate("EDI", "Node", "DeletionStateName", (int)this["DeletionStateCode"]));
            }
            if (!this.ContainsKey("IsEnabledName"))
            {
                this.Add("IsEnabledName", host.Translate("EDI", "Node", "IsEnabledName", (int)this["IsEnabled"]));
            }
        }
    }
}
