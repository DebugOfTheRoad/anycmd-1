using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IAccountUpdateInput : IEntityUpdateInput
    {
        DateTime? AllowEndTime { get; }
        DateTime? AllowStartTime { get; }
        string AuditState { get; }
        string Description { get; }
        int IsEnabled { get; }
        DateTime? LockEndTime { get; }
        DateTime? LockStartTime { get; }
        Guid? ContractorID { get; }
        string Code { get; }
        string Email { get; }
        string Mobile { get; }
        string Name { get; }
        string OrganizationCode { get; }
        string QQ { get; }
        string QuickQuery { get; }
        string QuickQuery1 { get; }
        string QuickQuery2 { get; }
        string Telephone { get; }
    }
}
