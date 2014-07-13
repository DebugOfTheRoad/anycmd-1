namespace Anycmd.Commands
{
    using Bus;
    using Model;

    /// <summary>
    /// Represents that the implemented classes are commands.
    /// </summary>
    public interface ICommand : IMessage, IEntity
    {

    }
}
