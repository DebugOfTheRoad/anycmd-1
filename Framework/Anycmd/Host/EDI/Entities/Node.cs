
namespace Anycmd.Host.EDI.Entities
{
    using Anycmd.EDI;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 节点
    /// </summary>
    public class Node : NodeBase, IAggregateRoot
    {
        public Node() { }

        public static Node Create(INodeCreateInput input)
        {
            return new Node
            {
                Abstract = input.Abstract,
                AnycmdApiAddress = input.AnycmdApiAddress,
                AnycmdWSAddress = input.AnycmdWSAddress,
                BeatPeriod = input.BeatPeriod,
                Code = input.Code,
                Description = input.Description,
                Email = input.Email,
                Icon = input.Icon,
                Id = input.Id.Value,
                IsEnabled = input.IsEnabled,
                Mobile = input.Mobile,
                Name = input.Name,
                TransferID = input.TransferID,
                Telephone = input.Telephone,
                Steward = input.Steward,
                SortCode = input.SortCode,
                SecretKey = input.SecretKey,
                QQ = input.QQ,
                PublicKey = input.PublicKey,
                Organization = input.Organization
            };
        }

        public void Update(INodeUpdateInput input)
        {
            this.Abstract = input.Abstract;
            this.AnycmdApiAddress = input.AnycmdApiAddress;
            this.AnycmdWSAddress = input.AnycmdWSAddress;
            this.BeatPeriod = input.BeatPeriod;
            this.Code = input.Code;
            this.Description = input.Description;
            this.Email = input.Email;
            this.Icon = input.Icon;
            this.IsEnabled = input.IsEnabled;
            this.Mobile = input.Mobile;
            this.Name = input.Name;
            this.Organization = input.Organization;
            this.PublicKey = input.PublicKey;
            this.QQ = input.QQ;
            this.SecretKey = input.SecretKey;
            this.SortCode = input.SortCode;
            this.Steward = input.Steward;
            this.Telephone = input.Telephone;
            this.TransferID = input.TransferID;
        }
    }
}
