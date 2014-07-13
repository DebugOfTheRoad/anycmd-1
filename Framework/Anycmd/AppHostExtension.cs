
namespace Anycmd
{
    using Host.EDI.Messages;
    using Host.EDI.ValueObjects;
    using System;

    public static class AppHostExtension
    {
        public static void AddArchive(this AppHost host, IArchiveCreateInput input)
        {
            host.Handle(new AddArchiveCommand(input));
        }
        public static void UpdateArchive(this AppHost host, IArchiveUpdateInput input)
        {
            host.Handle(new UpdateArchiveCommand(input));
        }
        public static void RemoveArchive(this AppHost host, Guid archiveID)
        {
            host.Handle(new RemoveArchiveCommand(archiveID));
        }

        public static void AddBatch(this AppHost host, IBatchCreateInput input)
        {
            host.Handle(new AddBatchCommand(input));
        }
        public static void UpdateBatch(this AppHost host, IBatchUpdateInput input)
        {
            host.Handle(new UpdateBatchCommand(input));
        }
        public static void RemoveBatch(this AppHost host, Guid batchID)
        {
            host.Handle(new RemoveBatchCommand(batchID));
        }

        public static void AddElement(this AppHost host, IElementCreateInput input)
        {
            host.Handle(new AddElementCommand(input));
        }
        public static void UpdateElement(this AppHost host, IElementUpdateInput input)
        {
            host.Handle(new UpdateElementCommand(input));
        }
        public static void RemoveElement(this AppHost host, Guid elementID)
        {
            host.Handle(new RemoveElementCommand(elementID));
        }

        public static void AddInfoDic(this AppHost host, IInfoDicCreateInput input)
        {
            host.Handle(new AddInfoDicCommand(input));
        }
        public static void UpdateInfoDic(this AppHost host, IInfoDicUpdateInput input)
        {
            host.Handle(new UpdateInfoDicCommand(input));
        }
        public static void RemoveInfoDic(this AppHost host, Guid infoDicID)
        {
            host.Handle(new RemoveInfoDicCommand(infoDicID));
        }

        public static void AddInfoDicItem(this AppHost host, IInfoDicItemCreateInput input)
        {
            host.Handle(new AddInfoDicItemCommand(input));
        }
        public static void UpdateInfoDicItem(this AppHost host, IInfoDicItemUpdateInput input)
        {
            host.Handle(new UpdateInfoDicItemCommand(input));
        }
        public static void RemoveInfoDicItem(this AppHost host, Guid infoDicItemID)
        {
            host.Handle(new RemoveInfoDicItemCommand(infoDicItemID));
        }

        public static void AddNode(this AppHost host, INodeCreateInput input)
        {
            host.Handle(new AddNodeCommand(input));
        }
        public static void UpdateNode(this AppHost host, INodeUpdateInput input)
        {
            host.Handle(new UpdateNodeCommand(input));
        }
        public static void RemoveNode(this AppHost host, Guid nodeID)
        {
            host.Handle(new RemoveNodeCommand(nodeID));
        }

        public static void RemoveNodeOntologyCare(this AppHost host, Guid nodeOntologyCareID)
        {
            host.Handle(new RemoveNodeOntologyCareCommand(nodeOntologyCareID));
        }

        public static void AddNodeOntologyCare(this AppHost host, INodeOntologyCareCreateInput input)
        {
            host.Handle(new AddNodeOntologyCareCommand(input));
        }

        public static void RemoveNodeElementCare(this AppHost host, Guid nodeElementCareID)
        {
            host.Handle(new RemoveNodeElementCareCommand(nodeElementCareID));
        }

        public static void AddNodeElementCare(this AppHost host, INodeElementCareCreateInput input)
        {
            host.Handle(new AddNodeElementCareCommand(input));
        }

        public static void AddOntology(this AppHost host, IOntologyCreateInput input)
        {
            host.Handle(new AddOntologyCommand(input));
        }
        public static void UpdateOntology(this AppHost host, IOntologyUpdateInput input)
        {
            host.Handle(new UpdateOntologyCommand(input));
        }
        public static void RemoveOntology(this AppHost host, Guid ontologyID)
        {
            host.Handle(new RemoveOntologyCommand(ontologyID));
        }

        public static void AddInfoGroup(this AppHost host, IInfoGroupCreateInput input)
        {
            host.Handle(new AddInfoGroupCommand(input));
        }

        public static void UpdateInfoGroup(this AppHost host, IInfoGroupUpdateInput input)
        {
            host.Handle(new UpdateInfoGroupCommand(input));
        }

        public static void RemoveInfoGroup(this AppHost host, Guid infoGroupID)
        {
            host.Handle(new RemoveInfoGroupCommand(infoGroupID));
        }

        public static void AddAction(this AppHost host, IActionCreateInput input)
        {
            host.Handle(new AddActionCommand(input));
        }

        public static void UpdateAction(this AppHost host, IActionUpdateInput input)
        {
            host.Handle(new UpdateActionCommand(input));
        }

        public static void RemoveAction(this AppHost host, Guid actionID)
        {
            host.Handle(new RemoveActionCommand(actionID));
        }

        public static void AddTopic(this AppHost host, ITopicCreateInput input)
        {
            host.Handle(new AddTopicCommand(input));
        }

        public static void UpdateTopic(this AppHost host, ITopicUpdateInput input)
        {
            host.Handle(new UpdateTopicCommand(input));
        }

        public static void RemoveTopic(this AppHost host, Guid topicID)
        {
            host.Handle(new RemoveTopicCommand(topicID));
        }
    }
}
