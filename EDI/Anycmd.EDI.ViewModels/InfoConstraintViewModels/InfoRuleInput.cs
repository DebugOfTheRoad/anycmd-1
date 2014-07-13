﻿
namespace Anycmd.EDI.ViewModels.InfoConstraintViewModels
{
    using Anycmd.Model;
    using System;
    using System.ComponentModel;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class InfoRuleInput : ManagedPropertyValues, IInfoModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public int IsEnabled { get; set; }
    }
}
