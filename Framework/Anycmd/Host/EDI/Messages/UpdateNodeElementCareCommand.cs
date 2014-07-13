
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using System;

    public class UpdateNodeElementCareCommand : Command, ISysCommand {
        #region Ctor
        public UpdateNodeElementCareCommand(Guid nodeElementCareID, bool isInfoIDItem)
        {
            this.NodeElementCareID = nodeElementCareID;
            this.IsInfoIDItem = isInfoIDItem;
        }
        #endregion

        public Guid NodeElementCareID { get; private set; }
        public bool IsInfoIDItem { get; private set; }
    }
}
