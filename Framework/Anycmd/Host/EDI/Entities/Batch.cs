
namespace Anycmd.Host.EDI.Entities {
    using Anycmd.EDI;
    using Model;
    using System;
    using ValueObjects;

    /// <summary>
    /// 批。“批”最初是用来批量生成待分发向一线通的Create型命令的。
    /// <remarks>
    /// 从两个角度理解批：1，一个批往往涉及多个命令；2，一个批往往影响多个实体。
    /// </remarks>
    /// </summary>
    public class Batch : EntityBase, IAggregateRoot, IBatch {
        public Batch() { }

        public static Batch Create(IBatchCreateInput input)
        {
            return new Batch
            {
                Id = input.Id.Value,
                IncludeDescendants = input.IncludeDescendants,
                NodeID = input.NodeID,
                OntologyID = input.OntologyID,
                OrganizationCode = input.OrganizationCode,
                Title = input.Title,
                Total = 0,
                Type = input.Type,
                Description = input.Description
            };
        }

        public void Update(IBatchUpdateInput input)
        {
            this.Description = input.Description;
            this.Title = input.Title;
        }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid OntologyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid NodeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrganizationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IncludeDescendants { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Total { get; set; }
    }
}
