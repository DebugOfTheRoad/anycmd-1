
namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IRoleCreateInput : IEntityCreateInput
    {
        string CategoryCode { get; }
        string Description { get; }
        string Icon { get; }
        int IsEnabled { get; }
        string Name { get; }
        int SortCode { get; }
    }
}
