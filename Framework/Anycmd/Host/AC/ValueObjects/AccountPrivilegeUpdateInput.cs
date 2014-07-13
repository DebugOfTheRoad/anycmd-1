
namespace Anycmd.Host.AC.ValueObjects
{
    using Model;
    using System;

    public class AccountPrivilegeUpdateInput : IInputModel, IAccountPrivilegeUpdateInput
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PrivilegeConstraint { get; set; }
    }
}
