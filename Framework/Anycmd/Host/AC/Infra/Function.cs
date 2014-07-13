
namespace Anycmd.Host.AC.Infra
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 操作。或者叫功能。
    /// <remarks>
    /// 而权限是“访问受控”的操作。
    /// </remarks>
    /// </summary>
    public class Function : FunctionBase, IAggregateRoot
    {
        public Function() { }

        public static Function Create(IFunctionCreateInput input)
        {
            return new Function
            {
                Id = input.Id.Value,
                Code = input.Code,
                Description = input.Description,
                IsEnabled = input.IsEnabled,
                DeveloperID = input.DeveloperID,
                ResourceTypeID = input.ResourceTypeID,
                SortCode = input.SortCode,
                IsManaged = input.IsManaged
            };
        }

        public void Update(IFunctionUpdateInput input)
        {
            this.Code = input.Code;
            this.IsManaged = input.IsManaged;
            this.IsEnabled = input.IsEnabled;
            this.Description = input.Description;
            this.DeveloperID = input.DeveloperID;
            this.SortCode = input.SortCode;
        }
    }
}
