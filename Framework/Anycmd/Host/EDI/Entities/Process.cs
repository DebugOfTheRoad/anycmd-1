
namespace Anycmd.Host.EDI.Entities
{
    using Anycmd.EDI;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 进程
    /// </summary>
    public class Process : ProcessBase, IAggregateRoot
    {
        public Process() { }

        public static Process Create(IProcessCreateInput input)
        {
            return new Process
            {
                Type = input.Type,
                Id = input.Id.Value,
                IsEnabled = input.IsEnabled,
                Description = input.Description,
                Name = input.Name,
                NetPort = input.NetPort,
                OntologyID = input.OntologyID,
                OrganizationCode = input.OrganizationCode
            };
        }

        public void Update(IProcessUpdateInput input)
        {
            this.Name = input.Name;
            this.IsEnabled = input.IsEnabled;
        }
    }
}
