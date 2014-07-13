
namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IInfoDicCreateInput : IEntityCreateInput
    {
        string Code { get; }
        string Description { get; }
        int IsEnabled { get; }
        string Name { get; }
        int SortCode { get; }
    }
}
