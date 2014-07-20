
namespace Anycmd.Host.EDI
{
    using Messages;
    using System;
    using ValueObjects;

    public static class AppHostExtension
    {
        #region EDI
        public static void AddArchive(this IAppHost host, IArchiveCreateInput input)
        {
            host.Handle(new AddArchiveCommand(input));
        }
        public static void UpdateArchive(this IAppHost host, IArchiveUpdateInput input)
        {
            host.Handle(new UpdateArchiveCommand(input));
        }
        public static void RemoveArchive(this IAppHost host, Guid archiveID)
        {
            host.Handle(new RemoveArchiveCommand(archiveID));
        }

        public static void AddBatch(this IAppHost host, IBatchCreateInput input)
        {
            host.Handle(new AddBatchCommand(input));
        }
        public static void UpdateBatch(this IAppHost host, IBatchUpdateInput input)
        {
            host.Handle(new UpdateBatchCommand(input));
        }
        public static void RemoveBatch(this IAppHost host, Guid batchID)
        {
            host.Handle(new RemoveBatchCommand(batchID));
        }

        public static void AddElement(this IAppHost host, IElementCreateInput input)
        {
            host.Handle(new AddElementCommand(input));
        }
        public static void UpdateElement(this IAppHost host, IElementUpdateInput input)
        {
            host.Handle(new UpdateElementCommand(input));
        }
        public static void RemoveElement(this IAppHost host, Guid elementID)
        {
            host.Handle(new RemoveElementCommand(elementID));
        }

        public static void AddInfoDic(this IAppHost host, IInfoDicCreateInput input)
        {
            host.Handle(new AddInfoDicCommand(input));
        }
        public static void UpdateInfoDic(this IAppHost host, IInfoDicUpdateInput input)
        {
            host.Handle(new UpdateInfoDicCommand(input));
        }
        public static void RemoveInfoDic(this IAppHost host, Guid infoDicID)
        {
            host.Handle(new RemoveInfoDicCommand(infoDicID));
        }

        public static void AddInfoDicItem(this IAppHost host, IInfoDicItemCreateInput input)
        {
            host.Handle(new AddInfoDicItemCommand(input));
        }
        public static void UpdateInfoDicItem(this IAppHost host, IInfoDicItemUpdateInput input)
        {
            host.Handle(new UpdateInfoDicItemCommand(input));
        }
        public static void RemoveInfoDicItem(this IAppHost host, Guid infoDicItemID)
        {
            host.Handle(new RemoveInfoDicItemCommand(infoDicItemID));
        }

        public static void AddNode(this IAppHost host, INodeCreateInput input)
        {
            host.Handle(new AddNodeCommand(input));
        }
        public static void UpdateNode(this IAppHost host, INodeUpdateInput input)
        {
            host.Handle(new UpdateNodeCommand(input));
        }
        public static void RemoveNode(this IAppHost host, Guid nodeID)
        {
            host.Handle(new RemoveNodeCommand(nodeID));
        }

        public static void RemoveNodeOntologyCare(this IAppHost host, Guid nodeOntologyCareID)
        {
            host.Handle(new RemoveNodeOntologyCareCommand(nodeOntologyCareID));
        }

        public static void AddNodeOntologyCare(this IAppHost host, INodeOntologyCareCreateInput input)
        {
            host.Handle(new AddNodeOntologyCareCommand(input));
        }

        public static void RemoveNodeElementCare(this IAppHost host, Guid nodeElementCareID)
        {
            host.Handle(new RemoveNodeElementCareCommand(nodeElementCareID));
        }

        public static void AddNodeElementCare(this IAppHost host, INodeElementCareCreateInput input)
        {
            host.Handle(new AddNodeElementCareCommand(input));
        }

        public static void AddOntology(this IAppHost host, IOntologyCreateInput input)
        {
            host.Handle(new AddOntologyCommand(input));
        }
        public static void UpdateOntology(this IAppHost host, IOntologyUpdateInput input)
        {
            host.Handle(new UpdateOntologyCommand(input));
        }
        public static void RemoveOntology(this IAppHost host, Guid ontologyID)
        {
            host.Handle(new RemoveOntologyCommand(ontologyID));
        }

        public static void AddOntologyOrganization(this IAppHost host, IOntologyOrganizationCreateInput input)
        {
            host.Handle(new AddOntologyOrganizationCommand(input));
        }

        public static void RemoveOntologyOrganization(this IAppHost host, Guid ontologyID, Guid organizationID)
        {
            host.Handle(new RemoveOntologyOrganizationCommand(ontologyID, organizationID));
        }

        public static void AddInfoGroup(this IAppHost host, IInfoGroupCreateInput input)
        {
            host.Handle(new AddInfoGroupCommand(input));
        }

        public static void UpdateInfoGroup(this IAppHost host, IInfoGroupUpdateInput input)
        {
            host.Handle(new UpdateInfoGroupCommand(input));
        }

        public static void RemoveInfoGroup(this IAppHost host, Guid infoGroupID)
        {
            host.Handle(new RemoveInfoGroupCommand(infoGroupID));
        }

        public static void AddAction(this IAppHost host, IActionCreateInput input)
        {
            host.Handle(new AddActionCommand(input));
        }

        public static void UpdateAction(this IAppHost host, IActionUpdateInput input)
        {
            host.Handle(new UpdateActionCommand(input));
        }

        public static void RemoveAction(this IAppHost host, Guid actionID)
        {
            host.Handle(new RemoveActionCommand(actionID));
        }

        public static void AddTopic(this IAppHost host, ITopicCreateInput input)
        {
            host.Handle(new AddTopicCommand(input));
        }

        public static void UpdateTopic(this IAppHost host, ITopicUpdateInput input)
        {
            host.Handle(new UpdateTopicCommand(input));
        }

        public static void RemoveTopic(this IAppHost host, Guid topicID)
        {
            host.Handle(new RemoveTopicCommand(topicID));
        }

        public static void AddProcess(this IAppHost host, IProcessCreateInput input)
        {
            host.Handle(new AddProcessCommand(input));
        }

        public static void UpdateProcess(this IAppHost host, IProcessUpdateInput input)
        {
            host.Handle(new UpdateProcessCommand(input));
        }
        #endregion
    }
}
