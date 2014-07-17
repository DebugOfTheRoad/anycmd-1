
namespace Anycmd.Host.AC.Identity
{
    using Anycmd.AC.Identity;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 账户。
    /// </summary>
    public class Account : AccountBase, IAggregateRoot
    {
        public Account()
        {
            IsEnabled = 1;
        }

        public static Account Create(IAccountCreateInput input)
        {
            return new Account
            {
                Id = input.Id.Value,
                LoginName = input.LoginName,
                AllowEndTime = input.AllowEndTime,
                AllowStartTime = input.AllowStartTime,
                AuditState = input.AuditState,
                Description = input.Description,
                IsEnabled = input.IsEnabled,
                LockEndTime = input.LockEndTime,
                LockStartTime = input.LockStartTime,
                Code = input.Code,
                Email = input.Email,
                Mobile = input.Mobile,
                Name = input.Name,
                OrganizationCode = input.OrganizationCode,
                QQ = input.QQ,
                QuickQuery = input.QuickQuery,
                QuickQuery1 = input.QuickQuery1,
                QuickQuery2 = input.QuickQuery2,
                Telephone = input.Telephone,
                Question = input.QuickQuery      ,
                Password=input.Password
            };
        }

        public void Update(IAccountUpdateInput input)
        {
            this.AllowEndTime = input.AllowEndTime;
            this.AllowStartTime = input.AllowStartTime;
            this.AuditState = input.AuditState;
            this.Description = input.Description;
            this.IsEnabled = input.IsEnabled;
            this.LockEndTime = input.LockEndTime;
            this.LockStartTime = input.LockStartTime;
            this.Code = input.Code;
            this.Email = input.Email;
            this.Mobile = input.Mobile;
            this.Name = input.Name;
            this.OrganizationCode = input.OrganizationCode;
            this.QQ = input.QQ;
            this.QuickQuery = input.QuickQuery;
            this.QuickQuery1 = input.QuickQuery1;
            this.QuickQuery2 = input.QuickQuery2;
            this.Telephone = input.Telephone;
        }
    }
}
