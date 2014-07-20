using System;
namespace Anycmd.Host.AC.ValueObjects
{
    public interface IPasswordAssignInput
    {
        Guid Id { get; set; }
        string LoginName { get; }
        string Password { get; }
    }
}
