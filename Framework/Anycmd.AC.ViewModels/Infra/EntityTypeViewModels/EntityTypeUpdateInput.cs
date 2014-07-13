
namespace Anycmd.AC.Infra.ViewModels.EntityTypeViewModels
{
    using Anycmd.Host.AC.ValueObjects;
    using Model;
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 
    /// </summary>
    public class EntityTypeUpdateInput : IInputModel, IEntityTypeUpdateInput
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Name { get; set; }
        [Required]
        public bool IsOrganizational { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid DatabaseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SchemaName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Codespace { get; set; }
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
        [Required]
        public Guid DeveloperID { get; set; }
    }
}
