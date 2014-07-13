﻿
namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IButtonCreateInput : IEntityCreateInput
    {
        string Code { get; }
        string CategoryCode { get; }
        string Description { get; }
        string Icon { get; }
        int IsEnabled { get; }
        string Name { get; }
        int SortCode { get; }
    }
}