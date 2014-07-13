using System;

namespace Anycmd.AC.ViewModels.PrivilegeViewModels
{
    using Host.AC.ValueObjects;
    using Model;

    public class PrivilegeBigramUpdateInput : IInputModel, IPrivilegeBigramUpdateInput
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PrivilegeConstraint { get; set; }
    }
}
