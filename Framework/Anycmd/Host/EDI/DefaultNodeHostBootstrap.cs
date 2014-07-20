
namespace Anycmd.Host.EDI
{
    using Entities;
    using Repositories;
    using System.Linq;

    public class DefaultNodeHostBootstrap : INodeHostBootstrap
    {
        private readonly IAppHost host;

        public DefaultNodeHostBootstrap(IAppHost host)
        {
            this.host = host;
        }

        public Archive[] GetArchives()
        {
            var repository = host.GetRequiredService<IRepository<Archive>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToArray();
            }
        }

        public Element[] GetElements()
        {
            var repository = host.GetRequiredService<IRepository<Element>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().Where(a => a.DeletionStateCode == 0).ToArray();
            }
        }

        public InfoDicItem[] GetInfoDicItems()
        {
            var repository = host.GetRequiredService<IRepository<InfoDicItem>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().Where(a => a.DeletionStateCode == 0).ToArray();
            }
        }

        public InfoDic[] GetInfoDics()
        {
            var repository = host.GetRequiredService<IRepository<InfoDic>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().Where(a => a.DeletionStateCode == 0).ToArray();
            }
        }

        public NodeElementAction[] GetNodeElementActions()
        {
            var repository = host.GetRequiredService<IRepository<NodeElementAction>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToArray();
            }
        }

        public NodeElementCare[] GetNodeElementCares()
        {
            var repository = host.GetRequiredService<IRepository<NodeElementCare>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToArray();
            }
        }

        public NodeOntologyCare[] GetNodeOntologyCares()
        {
            var repository = host.GetRequiredService<IRepository<NodeOntologyCare>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToArray();
            }
        }

        public NodeOntologyOrganization[] GetNodeOntologyOrganizations()
        {
            var repository = host.GetRequiredService<IRepository<NodeOntologyOrganization>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToArray();
            }
        }

        public Node[] GetNodes()
        {
            var repository = host.GetRequiredService<IRepository<Node>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().Where(a => a.DeletionStateCode == 0).ToArray();
            }
        }

        public Ontology[] GetOntologies()
        {
            var repository = host.GetRequiredService<IRepository<Ontology>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().Where(a => a.DeletionStateCode == 0).ToArray();
            }
        }

        public InfoGroup[] GetInfoGroups()
        {
            var repository = host.GetRequiredService<IRepository<Ontology>>();
            using (var context = repository.Context)
            {
                return context.Query<InfoGroup>().ToArray();
            }
        }

        public Action[] GetActions()
        {
            var repository = host.GetRequiredService<IRepository<Ontology>>();
            using (var context = repository.Context)
            {
                return context.Query<Action>().ToArray();
            }
        }

        public Topic[] GetTopics()
        {
            var repository = host.GetRequiredService<IRepository<Ontology>>();
            using (var context = repository.Context)
            {
                return context.Query<Topic>().ToArray();
            }
        }

        public OntologyOrganization[] GetOntologyOrganizations()
        {
            var repository = host.GetRequiredService<IRepository<OntologyOrganization>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToArray();
            }
        }

        public Process[] GetProcesses()
        {
            var repository = host.GetRequiredService<IRepository<Process>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToArray();
            }
        }
    }
}
