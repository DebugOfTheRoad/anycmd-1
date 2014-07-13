
namespace Anycmd.Model
{
    using System;

    public interface IEntityUpdateInput : IInputModel
    {
        Guid Id { get; }
    }
}
