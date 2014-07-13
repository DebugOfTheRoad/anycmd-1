
namespace Anycmd.Host
{
    public static class EntityTypeExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="propertyCode"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool TryGetProperty(this EntityTypeState entityType, string propertyCode, out PropertyState property)
        {
            return entityType.AppHost.EntityTypeSet.TryGetProperty(entityType, propertyCode, out property);
        }
    }
}
