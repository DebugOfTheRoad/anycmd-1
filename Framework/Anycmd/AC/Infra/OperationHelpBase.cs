
namespace Anycmd.AC.Infra
{
    using Model;

    /// <summary>
    /// 操作帮助基类。<see cref="IOperationHelp"/>
    /// </summary>
    public abstract class OperationHelpBase : EntityBase, IOperationHelp
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual string Content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int IsEnabled { get; set; }
    }
}
