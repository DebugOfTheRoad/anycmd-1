
namespace Anycmd.Host.AC.Infra
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 系统模型集合
    /// </summary>
    public interface IEntityTypeSet : IEnumerable<EntityTypeState>
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityTypeID"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        bool TryGetEntityType(Guid entityTypeID, out EntityTypeState entityType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codespace"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        bool TryGetEntityType(string codespace, string entityTypeCode, out EntityTypeState entityType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyID"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        bool TryGetProperty(Guid propertyID, out PropertyState property);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="propertyCode"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        bool TryGetProperty(EntityTypeState entityType, string propertyCode, out PropertyState property);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        IReadOnlyDictionary<string, PropertyState> GetProperties(EntityTypeState entityType);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<PropertyState> GetProperties();
    }
}
