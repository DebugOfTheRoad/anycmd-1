﻿using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IInfoDicItemCreateInput : IEntityCreateInput
    {
        string Code { get; }
        string Description { get; }
        Guid InfoDicID { get; }
        int IsEnabled { get; }
        string Level { get; }
        string Name { get; }
        int SortCode { get; }
    }
}
