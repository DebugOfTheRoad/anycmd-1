
namespace Anycmd.Host.AC.Infra.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC.Infra;
    using Anycmd.Events;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class AppSystemUpdatedEvent : DomainEvent
    {
        public AppSystemUpdatedEvent(AppSystemBase source, IAppSystemUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Input = input;
        }

        public IAppSystemUpdateInput Input { get; private set; }
    }
}