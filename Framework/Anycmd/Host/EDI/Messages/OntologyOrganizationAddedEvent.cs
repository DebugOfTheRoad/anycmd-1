﻿
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;
    using ValueObjects;

    public class OntologyOrganizationAddedEvent : DomainEvent
    {
        #region Ctor
        public OntologyOrganizationAddedEvent(OntologyOrganizationBase source, IOntologyOrganizationCreateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IOntologyOrganizationCreateInput Input { get; private set; }
    }
}
