using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Anycmd.EDI.ViewModels.OntologyViewModels
{
    using Host.EDI.ValueObjects;
    using Model;

    public class OntologyCreateInput : EntityCreateInput, IOntologyCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid MessageProviderID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid EntityProviderID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid EntityDatabaseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid MessageDatabaseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string EntitySchemaName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string MessageSchemaName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string EntityTableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public int IsEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int EditWidth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int EditHeight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Icon { get; set; }
    }
}
