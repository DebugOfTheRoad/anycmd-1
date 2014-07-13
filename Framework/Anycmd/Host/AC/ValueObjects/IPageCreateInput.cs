
namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IPageCreateInput : IEntityCreateInput
    {
        string Icon { get; }
        string Tooltip { get; }
    }
}
