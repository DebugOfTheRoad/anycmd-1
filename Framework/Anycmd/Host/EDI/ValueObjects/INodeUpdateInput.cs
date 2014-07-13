using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface INodeUpdateInput : IEntityUpdateInput
    {
        string Abstract { get; }
        string AnycmdApiAddress { get; }
        string AnycmdWSAddress { get; }
        int? BeatPeriod { get; }
        string Code { get; }
        string Description { get; }
        string Email { get; }
        string Icon { get; }
        int IsEnabled { get; }
        string Mobile { get; }
        string Name { get; }
        string Organization { get; }
        string PublicKey { get; }
        string QQ { get; }
        string SecretKey { get; }
        int SortCode { get; }
        string Steward { get; }
        string Telephone { get; }
        Guid TransferID { get; }
    }
}
