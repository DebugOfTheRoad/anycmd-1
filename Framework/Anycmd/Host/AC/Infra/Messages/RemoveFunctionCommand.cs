﻿
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveFunctionCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveFunctionCommand(Guid functionID)
            : base(functionID)
        {

        }
    }
}
