
namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IDicCreateInput : IEntityCreateInput
    {
        string Code { get; }
        string Description { get; }
        int IsEnabled { get; }
        int SortCode { get; }
        string Name { get; }
    }
}
