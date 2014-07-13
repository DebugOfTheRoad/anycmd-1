using Anycmd.Host.EDI.ValueObjects;
using Anycmd.Model;
using System;

namespace Anycmd.EDI.ViewModels.OntologyViewModels
{
    public class OntologyOrganizationCreateInput : EntityCreateInput, IOntologyOrganizationCreateInput
    {
        public Guid OntologyID { get; set; }

        public Guid OrganizationID { get; set; }
    }
}
