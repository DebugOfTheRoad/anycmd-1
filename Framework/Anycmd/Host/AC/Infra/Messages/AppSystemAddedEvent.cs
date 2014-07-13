
namespace Anycmd.Host.AC.Infra.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC.Infra;
    using Anycmd.Events;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class AppSystemAddedEvent : DomainEvent
    {
        public AppSystemAddedEvent(AppSystemBase source, IAppSystemCreateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Input = input;
        }

        public IAppSystemCreateInput Input { get; private set; }
    }
}