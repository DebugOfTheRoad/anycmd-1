using System;

namespace Anycmd.AC
{
    /// <summary>
    /// 权限二元组<see cref="IPrivilegeBigram"/>模型化的是9中AC元素的两两组合，为了简化问题我们把二元中的其中一员指定为Subject，另一元指定为Object。9中AC元素都可以充当Object。
    /// </summary>
    [Flags]
    public enum ACObjectType
    {
        /// <summary>
        /// 未定义
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// 
        /// </summary>
        Account = 0x0001,
        /// <summary>
        /// 
        /// </summary>
        Organization = 0x0002,
        /// <summary>
        /// 
        /// </summary>
        Role = 0x0004,
        /// <summary>
        /// 
        /// </summary>
        Group = 0x0008,
        /// <summary>
        /// 
        /// </summary>
        Function = 0x0010,
        /// <summary>
        /// 
        /// </summary>
        Menu = 0x0020,
        /// <summary>
        /// 
        /// </summary>
        AppSystem = 0x0040,
        /// <summary>
        /// 
        /// </summary>
        ResourceType = 0x0080,
        // TODO:考察RBAC中定义的SSD和DSD是否是AC元素。如果是，加入到这里来。
        /// <summary>
        /// 暂不支持，该取值的存在是为了概念完整性。组成授权路由链表。如同面向对象机制中类的“继承”。
        /// </summary>
        Privilege = 0x1fff
    }
}
