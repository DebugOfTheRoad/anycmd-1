
namespace Anycmd.Model
{
    using System;

    public interface IEntityCreateInput : IInputModel
    {
        /// <summary>
        /// <remarks>
        /// 在ASP.NET MVC中使用它默认的输入模型验证器时需要Id字段为可空的。
        /// </remarks>
        /// </summary>
        Guid? Id { get; }
    }
}
