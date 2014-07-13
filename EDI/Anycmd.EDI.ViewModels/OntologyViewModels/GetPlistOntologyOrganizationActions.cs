
namespace Anycmd.EDI.ViewModels.OntologyViewModels {
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistOntologyOrganizationActions : GetPlistResult {
        public Guid ontologyID { get; set; }

        public Guid organizationID { get; set; }
    }
}