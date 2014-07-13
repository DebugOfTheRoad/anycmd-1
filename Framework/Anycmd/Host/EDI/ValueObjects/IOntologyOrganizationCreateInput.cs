using Anycmd.Model;
using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    public interface IOntologyOrganizationCreateInput : IEntityCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        Guid OntologyID { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid OrganizationID { get; }
    }
}
