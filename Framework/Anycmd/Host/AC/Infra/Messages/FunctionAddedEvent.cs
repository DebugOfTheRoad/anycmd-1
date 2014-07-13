
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class FunctionAddedEvent : EntityAddedEvent<IFunctionCreateInput>
    {
        #region Ctor
        public FunctionAddedEvent(FunctionBase source, IFunctionCreateInput input)
            : base(source, input)
        {
        }
        #endregion
    }
}
