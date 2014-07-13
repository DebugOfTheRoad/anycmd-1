
namespace Anycmd.EDI.Service.Tests
{
    using Anycmd.Host.EDI;
    using Anycmd.Host.EDI.Handlers;
    using Anycmd.Host.EDI.Hecp;
    using Anycmd.Host.EDI.Info;
    using Client;
    using DataContracts;
    using ServiceModel.Operations;
    using ServiceStack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Util;
    using Xunit;

    /// <summary>
    /// 在Web服务器上对外提供有一个AnyMessage接口，该接口采用两种方式提供：一种基于soap/webservice，一种基于原生的http协议。
    /// 这只是同一个接口的两种终结点，两者的内涵完全一样，两者没有任何不同。
    /// <remarks>
    /// 本类中的测试用例最终到底是调用的WebApi接口还是WebService接口是由被测试的服务提供节点配置的“命令转移方式”决定的。
    /// 下面测试的都是中心节点的服务，命令转移方式在Anycmd.dbo.Node表的TransferID列中配置。也可从Mis系统的“节点管理”界面配置。
    /// 注意：因暂不支持配置同步，所以改变配置需重启服务应用程序。
    /// </remarks>
    /// </summary>
    public class ApiTest : IUseFixture<Boot>
    {
        public void SetFixture(Boot data)
        {
        }

        #region IsAlive
        [Fact]
        public async Task IsAlive()
        {
            var client = new JsonServiceClient(NodeHost.Instance.Nodes.ThisNode.Node.AnycmdApiAddress);
            IsAlive isAlive = new IsAlive
            {
                Version = "v1"
            };
            var response = await client.GetAsync(isAlive);
            Assert.True(response.IsAlive);
            isAlive.Version = "version2";
            response = await client.GetAsync(isAlive);
            Assert.False(response.IsAlive);
            Assert.True(Status.InvalidApiVersion.ToName() == response.ReasonPhrase, response.Description);
        }
        #endregion

        #region Should_Get_InvalidVersion
        [Fact]
        public void Should_Get_InvalidVersion()
        {
            KeyValue[] infoValue = new KeyValueBuilder().Append("XM", "测试").Append("ZZJGM", "11011421005").ToArray();
            var request = new Message
            {
                Version = "InvalidVersion",
                MessageType = "action",
                IsDumb = true,
                Verb = Verb.Update.Code,
                Ontology = "JS",
                MessageID = Guid.NewGuid().ToString(),
                Body = new BodyData(new KeyValueBuilder().Append("Id", "010C1D7A-9BA5-4AEA-9D4B-290476A79D12").ToArray(), infoValue),
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidApiVersion == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidApiVersion == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Should_Get_InvalidMessageType
        [Fact]
        public void Should_Get_InvalidMessageType()
        {
            KeyValue[] infoValue = new KeyValueBuilder().Append("XM", "测试").Append("ZZJGM", "11011421005").ToArray();
            var request = new Message
            {
                Version = "v1",
                IsDumb = true,
                Verb = Verb.Update.Code,
                Ontology = "JS",
                MessageType = "InvalidMessageType",
                MessageID = new string('A', 100),
                Body = new BodyData(new KeyValueBuilder().Append("Id", "010C1D7A-9BA5-4AEA-9D4B-290476A79D12").ToArray(), infoValue)
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidMessageType == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidMessageType == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Action_Create_Dumb
        [Fact]
        public void Action_Create_Dumb()
        {
            var xm = System.Guid.NewGuid().ToString();
            // 注意:基于serviceStack.Text的json反序列化貌似不认单引号只认双引号.
            string json = "{\"XM\":\"" + xm + "\",\"ZZJGM\":\"11011421004\"}";
            IInfoStringConverter converter;
            NodeHost.Instance.InfoStringConverters.TryGetInfoStringConverter("json", out converter);
            var infoValue = converter.ToDataItems(json);
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                Verb = Verb.Create.Code,
                MessageType = "action",
                IsDumb = true,
                Ontology = "JS",
                Body = new BodyData(infoValue.ToDto(), infoValue.ToDto())
                {
                    QueryList = new string[] { "Id" }
                },
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXSignature();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            request.Verb = "Get";
            request.JSPXSignature();// 重新签名
            response = request.RequestCenterNode();
            Assert.True((int)Status.NotExist == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region NotAuthorized
        [Fact]
        public void NotAuthorized()
        {
            var xm = NewXM();
            string[] keys = new string[]{
                "XM","ZZJGM"
            };
            string[] values = new string[]{
                xm,"11011421004"
            };
            KeyValue[] infoValue = new KeyValueBuilder(keys, values).ToArray();
            var client = new JsonServiceClient(NodeHost.Instance.Nodes.ThisNode.Node.AnycmdApiAddress);
            var ticks = DateTime.UtcNow.Ticks;
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                MessageType = "action",
                IsDumb = true,
                Verb = Verb.Create.Code,
                Ontology = "JS",
                Body = new BodyData(infoValue, infoValue),
                Credential = new CredentialData
                {
                    ClientType = ClientType.Node.ToName(),
                    CredentialType = CredentialType.Token.ToName(),
                    ClientID = "41e711c6-f215-4606-a0bf-9af11cce1d54",
                    Ticks = ticks,
                    Password = TokenObject.Token("41e711c6-f215-4606-a0bf-9af11cce1d54", ticks, "invalidSecretKey")
                },
                TimeStamp = DateTime.UtcNow.Ticks
            };
            var response = client.Get(request);
            Assert.True(Status.NotAuthorized.ToName() == response.Body.Event.ReasonPhrase, response.Body.Event.Description);
            request.Verb = "Get";
            request.JSPXSignature();// 签名
            var result = request.RequestCenterNode();
            Assert.True((int)Status.NotExist == result.Body.Event.Status, result.Body.Event.Description);
        }
        #endregion

        #region Action_Create
        [Fact]
        public void Action_Create()
        {
            var xm = NewXM();
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM",xm},
                {"ZZJGM","11011421004"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                MessageType = "action",
                IsDumb = true,
                Verb = Verb.Create.Code,
                Ontology = "JS",
                Body = new BodyData(infoValue, infoValue)
                {
                    QueryList = new string[] { "Id" }
                },
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            //var response = request.RequestCenterNode();
            var response = AnyMessage.Create(HecpRequest.Create(request), NodeHost.Instance.Nodes.ThisNode).Response();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            request.JSPXSignature();// 签名
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            request.JSPXSignature();// 命令对象有更改则需重新签名
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            request.Verb = "Delete";
            request.JSPXSignature();// 命令对象有更改则需重新签名
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Action_Delete
        [Fact]
        public void Action_Delete()
        {
            var xm = NewXM();
            KeyValue[] infoValue = new List<KeyValue> {
                new KeyValue("XM",xm),
                new KeyValue("ZZJGM", "11011421004")
            }.ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                MessageType = "action",
                IsDumb = true,
                Verb = "Create",
                Ontology = "JS",
                Body = new BodyData(infoValue, infoValue)
                {
                    QueryList = new string[] { "Id" }
                },
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            request.Verb = "delete";
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);

        }
        #endregion

        #region Command_Create
        [Fact]
        public void Command_Create()
        {
            var xm = System.Guid.NewGuid().ToString();
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM",xm},
                {"ZZJGM","11011421004"},
                {"DZXX", "23934360@qq.com"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                MessageType = "command",
                IsDumb = true,
                Verb = "Create",
                Ontology = "JS",
                Body = new BodyData(infoValue, infoValue),
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.ReceiveOk == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.ReceiveOk == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Command_MessageID_CanNotBeNullOrEmpty
        [Fact]
        public void Command_MessageID_CanNotBeNullOrEmpty()
        {
            var xm = System.Guid.NewGuid().ToString();
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM",xm},
                {"ZZJGM","11011421005"}
            }).ToArray();
            var request = new Message
            {
                Version = "v1",
                MessageType = "command",
                IsDumb = true,
                Verb = "Create",
                Ontology = "JS",
                MessageID = (DateTime.Now.Ticks % 2) == 0 ? null : string.Empty,
                Body = new BodyData(infoValue, infoValue),
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidArgument == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidArgument == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Event_MessageIDNotExist
        [Fact]
        public void Event_MessageIDNotExist()
        {
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM","test"},
                {"ZZJGM","11010424"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                MessageType = "event",
                Verb = "Create",
                Ontology = "JS",
                IsDumb = true,
                TimeStamp = DateTime.UtcNow.Ticks,
                Version = "v1",
                Body = new BodyData(infoValue, infoValue)
                {
                    Event = new EventData
                    {
                        Status = (int)Status.AuditApproved,
                        ReasonPhrase = Status.AuditApproved.ToName(),
                        Subject = "StateCodeChanged.Audit",
                        SourceType = EventSourceType.Command.ToName()
                    }
                },
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.NotExist == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.NotExist == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Command_Event_NotExist
        [Fact]
        public void Command_Event_NotExist()
        {
            var xm = System.Guid.NewGuid().ToString();
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM",xm},
                {"ZZJGM","11011421005"}
            }).ToArray();
            var dto = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                MessageType = "event",
                Verb = "Create",
                Ontology = "JS",
                IsDumb = true,
                TimeStamp = DateTime.UtcNow.Ticks,
                Version = "v1",
                Body = new BodyData(infoValue, infoValue)
                {
                    Event = new EventData
                    {
                        Status = (int)Status.AuditApproved,
                        Subject = "StateCodeChanged",
                        ReasonPhrase = Status.AuditApproved.ToName(),
                        SourceType = EventSourceType.Command.ToName()
                    }
                },
            }.JSPXToken();
            // var response = dto.RequestCenterNode();
            var response = AnyMessage.Create(HecpRequest.Create(dto), NodeHost.Instance.Nodes.ThisNode).Response();
            Assert.True((int)Status.NotExist == response.Body.Event.Status, response.Body.Event.Description);
            dto.IsDumb = false;
            response = dto.RequestCenterNode();
            Assert.True((int)Status.NotExist == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Command_Event_NotExist2
        [Fact]
        public void Command_Event_NotExist2()
        {
            var xm = System.Guid.NewGuid().ToString();
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM",xm},
                {"ZZJGM","11011421005"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Verb = "Create",
                Ontology = "JS",
                MessageType = "event",
                IsDumb = true,
                TimeStamp = DateTime.UtcNow.Ticks,
                Version = "v1",
                Body = new BodyData(infoValue, infoValue)
                {
                    Event = new EventData
                    {
                        Subject = "StateCodeChanged",
                        SourceType = EventSourceType.Command.ToName(),
                        Status = (int)Status.AuditApproved
                    }
                }
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.NotExist == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.NotExist == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Entity_Event_ReceiveOk
        [Fact]
        public void Entity_Event_ReceiveOk()
        {
            var xm = System.Guid.NewGuid().ToString();
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM",xm},
                {"ZZJGM","11011421005"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                MessageType = "event",
                IsDumb = true,
                Verb = "Create",
                Ontology = "JS",
                TimeStamp = DateTime.UtcNow.Ticks,
                Version = "v1",
                Body = new BodyData(infoValue, infoValue)
                {
                    Event = new EventData
                    {
                        Status = (int)Status.Ok,
                        Subject = "StateCodeChanged",
                        ReasonPhrase = Status.Ok.ToName(),
                        SourceType = "entity"
                    }
                }
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.Nonsupport == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.Nonsupport == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Entity_Event_InvalidStateCode
        [Fact]
        public void Entity_Event_InvalidStateCode()
        {
            var xm = System.Guid.NewGuid().ToString();
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM",xm},
                {"ZZJGM","11011421005"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                MessageType = "event",
                IsDumb = true,
                Verb = "Create",
                Ontology = "JS",
                TimeStamp = DateTime.UtcNow.Ticks,
                Version = "v1",
                Body = new BodyData(infoValue, infoValue)
                {
                    Event = new EventData
                    {
                        Subject = "StateCodeChanged",
                        SourceType = "entity"
                    }
                },
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidStatus == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidStatus == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Ticks_TimeOut
        [Fact]
        public void Ticks_TimeOut()
        {
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM","测试"},
                {"ZZJGM","11011421005"}
            }).ToArray();
            var client = new JsonServiceClient(NodeHost.Instance.Nodes.ThisNode.Node.AnycmdApiAddress);
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                MessageType = "action",
                IsDumb = true,
                Verb = "Delete",
                Ontology = "JS",
                Body = new BodyData(new KeyValueBuilder().Append("Id", "69e58ec0-5eb2-4633-9117-b433fc205b8f").ToArray(), infoValue),
                TimeStamp = DateTime.UtcNow.Ticks
            }.TimeOutToken();
            var response = client.Get(request);
            Assert.True(Status.NotAuthorized.ToName() == response.Body.Event.ReasonPhrase, response.Body.Event.Description);
            request.IsDumb = false;
            response = client.Get(request);
            Assert.True(Status.NotAuthorized.ToName() == response.Body.Event.ReasonPhrase, response.Body.Event.Description);
        }
        #endregion

        #region Action_Create_Must_Gave_XM_And_ZZJGM
        [Fact]
        public void Action_Create_Must_Gave_XM_And_ZZJGM()
        {
            KeyValue[] infoValue = new KeyValueBuilder().Append("XM", "test").ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                MessageType = "action",
                IsDumb = true,
                Verb = "Create",
                Ontology = "JS",
                Body = new BodyData(infoValue, infoValue)
                {
                    QueryList = new string[] { "Id" }
                },
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidInfoID == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidInfoID == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Command_Create_Must_Gave_XM_And_ZZJGM
        [Fact]
        public void Command_Create_Must_Gave_XM_And_ZZJGM()
        {
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM","test"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                MessageType = "command",
                IsDumb = true,
                Verb = "Create",
                Ontology = "JS",
                Body = new BodyData(infoValue, infoValue),
                TimeStamp = DateTime.UtcNow.Ticks,
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidInfoID == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidInfoID == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Command_Action_Update
        [Fact]
        public void Command_Action_Update()
        {
            // 首先使用一个Create型命令准备测试数据。然后测试Command和Action，最后打扫现场删除测试数据。
            var xm = NewXM();
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM",xm},
                {"ZZJGM","11011421004"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                MessageType = "action",
                IsDumb = false,
                Verb = "Create",
                Ontology = "JS",
                Body = new BodyData(infoValue, infoValue),
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            // 从配置文件中读取或者从数据库表的列名读取
            KeyValue[] infoID = new KeyValueBuilder(new Dictionary<string, string> {
                {"XBM",(DateTime.Now.Ticks % 3).ToString()}
            }).ToArray();
            request.MessageID = Guid.NewGuid().ToString();
            request.MessageType = "Command";
            request.Body = new BodyData(infoValue, new KeyValueBuilder(request.Body.InfoValue).Append("XBM", "1").ToArray());
            request.TimeStamp = DateTime.UtcNow.Ticks;
            request.Verb = "Update";
            request.IsDumb = true;
            //response = request.RequestCenterNode();
            response = AnyMessage.Create(HecpRequest.Create(request), NodeHost.Instance.Nodes.CenterNode).Response();
            Assert.True((int)Status.ReceiveOk == response.Body.Event.Status, response.Body.Event.Description);
            request.MessageType = "action";
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);

            request.MessageType = "command";
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.ReceiveOk == response.Body.Event.Status, response.Body.Event.Description);
            request.MessageType = "action";
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            request.Verb = "delete";
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Action_Update
        [Fact]
        public void Action_Update()
        {
            var xm = NewXM();
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM",xm},
                {"ZZJGM","11011421004"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                MessageType = "action",
                IsDumb = false,
                Verb = "Create",
                Ontology = "JS",
                Body = new BodyData(infoValue, infoValue),
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            request.MessageID = Guid.NewGuid().ToString();
            request.Verb = "Update";
            request.TimeStamp = DateTime.UtcNow.Ticks;
            request.Body.InfoValue = new KeyValueBuilder().Append("XM", xm).Append("ZZJGM", "11011421004").ToArray();
            xm = NewXM();
            request.Body.InfoValue[0].Value = xm;
            request.Body.InfoValue[1].Value = "11011421005";
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            request.Body.InfoID = new KeyValueBuilder().Append("XM", xm).Append("ZZJGM", "11011421005").ToArray();
            request.Verb = "delete";
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Organization_Must_IsLeaf
        // 组织结构必须是叶子节点
        [Fact]
        public void Organization_Must_IsLeaf()
        {
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM","测试"},
                {"ZZJGM","110114"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                MessageType = "action",
                IsDumb = true,
                Verb = "Update",
                Ontology = "JS",
                Body = new BodyData(new KeyValueBuilder().Append("Id", "0008E9A4-CC11-48FB-9B1C-C72D4795AEDF").ToArray(), infoValue),
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidOrganization == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.InvalidOrganization == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Action_Update_OutOfLength
        [Fact]
        public void Action_Update_OutOfLength()
        {
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM","测试"},
                {"ZZJGM","11011421005"}
            }).ToArray();
            var request = new Message
            {
                Version = "v1",
                MessageType = "action",
                IsDumb = true,
                Verb = "Update",
                Ontology = "JS",
                MessageID = new string('A', 100),
                Body = new BodyData(new KeyValueBuilder().Append("Id", "010C1D7A-9BA5-4AEA-9D4B-290476A79D12").ToArray(), infoValue),
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.OutOfLength == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.OutOfLength == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Action_Get
        [Fact]
        public void Action_Get()
        {
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                Verb = "Get",
                MessageType = "action",
                IsDumb = true,
                Ontology = "JS",
                Body = new BodyData(new KeyValueBuilder().Append("Id", "0000A33A-F0A1-48CD-A9F2-FEB19F8E2BD0").ToArray(), null)
                {
                    QueryList = new string[] { "Id" }
                },
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            Assert.False(string.IsNullOrEmpty(response.Body.InfoValue[0].Key));
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            Assert.False(string.IsNullOrEmpty(response.Body.InfoValue[0].Key));
        }
        #endregion

        #region Action_Get_Performance
        [Fact]
        public async Task Action_Get_PerformanceAsync()
        {
            var client = new JsonServiceClient(NodeHost.Instance.Nodes.ThisNode.Node.AnycmdApiAddress);
            for (int i = 0; i < 1000; i++)
            {
                var request = new Message
                {
                    MessageID = System.Guid.NewGuid().ToString(),
                    Version = "v1",
                    Verb = "Get",
                    MessageType = "action",
                    IsDumb = false,
                    Ontology = "JS",
                    Body = new BodyData(new KeyValueBuilder().Append("Id", Guid.NewGuid().ToString()).ToArray(), null),
                    TimeStamp = DateTime.UtcNow.Ticks
                }.JSPXToken();
                var response = await client.GetAsync(request);
            }
        }

        [Fact]
        public void Action_Get_Performance()
        {
            var client = new JsonServiceClient(NodeHost.Instance.Nodes.ThisNode.Node.AnycmdApiAddress);
            for (int i = 0; i < 1000; i++)
            {
                var request = new Message
                {
                    MessageID = System.Guid.NewGuid().ToString(),
                    Version = "v1",
                    Verb = "Get",
                    MessageType = "action",
                    IsDumb = false,
                    Ontology = "JS",
                    Body = new BodyData(new KeyValueBuilder().Append("Id", Guid.NewGuid().ToString()).ToArray(), null),
                    TimeStamp = DateTime.UtcNow.Ticks
                }.JSPXToken();
                var response = client.Get(request);
            }
        }
        #endregion

        #region Action_Get_IncrementID
        [Fact]
        public void Action_Get_IncrementID()
        {
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                Verb = "Get",
                MessageType = "action",
                IsDumb = true,
                Ontology = "JS",
                Body = new BodyData(new KeyValueBuilder().Append("Id", "0000A33A-F0A1-48CD-A9F2-FEB19F8E2BD0").ToArray(), null),
                TimeStamp = DateTime.UtcNow.Ticks
            }.UIAToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            Assert.False(string.IsNullOrEmpty(response.Body.InfoValue[0].Key));
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            Assert.True(response.Body.InfoValue.Any(a => a.Key.Equals("IncrementID", StringComparison.OrdinalIgnoreCase)));
        }
        #endregion

        #region Action_Get2
        [Fact]
        public void Action_Get2()
        {
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                Verb = "Get",
                MessageType = "action",
                IsDumb = true,
                Ontology = "JS",
                Body = new BodyData(new KeyValueBuilder().Append("Id", "0000A33A-F0A1-48CD-A9F2-FEB19F8E2BD0").ToArray(), null),
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            var xm = response.Body.InfoValue.Where(a => a.Key.Equals("XM", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            Assert.True(xm != null);
            Assert.True(response.Body.InfoValue.Any(a => a.Key.Equals("ZZJGM", StringComparison.OrdinalIgnoreCase)));
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            xm = response.Body.InfoValue.Where(a => a.Key.Equals("XM", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            Assert.True(xm != null);
            Assert.True(response.Body.InfoValue.Any(a => a.Key.Equals("ZZJGM", StringComparison.OrdinalIgnoreCase)));
        }
        #endregion

        #region Command_Get
        [Fact]
        public void Command_Get()
        {
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                Verb = "Get",
                MessageType = "command",
                IsDumb = true,
                Ontology = "JS",
                Body = new BodyData(new KeyValueBuilder().Append("Id", "0000A33A-F0A1-48CD-A9F2-FEB19F8E2BD0").ToArray(), null),
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.ReceiveOk == response.Body.Event.Status, response.Body.Event.Description);
            Assert.False(string.IsNullOrEmpty(response.Body.InfoValue[0].Key));
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.ReceiveOk == response.Body.Event.Status, response.Body.Event.Description);
            Assert.False(string.IsNullOrEmpty(response.Body.InfoValue[0].Key));
        }
        #endregion

        #region Action_Head
        [Fact]
        public void Action_Head()
        {
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM","教师44968"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                Verb = "Head",
                MessageType = "action",
                IsDumb = true,
                Ontology = "JS",
                Body = new BodyData(infoValue, null),
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Command_Head
        [Fact]
        public void Command_Head()
        {
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM","教师44968"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                IsDumb = true,
                Verb = "Head",
                MessageType = "command",
                Ontology = "JS",
                Body = new BodyData(infoValue, null),
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.ReceiveOk == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.ReceiveOk == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        #region Permission
        [Fact]
        public void Permission()
        {
            KeyValue[] infoID = new KeyValueBuilder(new Dictionary<string, string> {
                {"SFZJH","320113198108242027"},
                {"SFZJLXM","1"},
                {"GHHM","85012345"}
            }).ToArray();
            var cmdDto = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                Verb = "Head",
                MessageType = "action",
                IsDumb = true,
                Ontology = "JS",
                Body = new BodyData(infoID, null),
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = cmdDto.RequestCenterNode();
            //var response = AnyMessage.Create(HecpRequest.Create(request, Credential.Create(request))).Response();
            Assert.True((int)Status.NoPermission == response.Body.Event.Status, response.Body.Event.Description);
            cmdDto.IsDumb = false;
            //response = request.RequestCenterNode();
            // 使用下面这行可以绕过网络传输从而易于调试，而上面那行需要网络传输
            response = AnyMessage.Create(HecpRequest.Create(cmdDto), NodeHost.Instance.Nodes.ThisNode).Response();
            Assert.True((int)Status.NoPermission == response.Body.Event.Status, response.Body.Event.Description);

            cmdDto.MessageType = "Command";
            response = cmdDto.RequestCenterNode();
            //var response = AnyMessage.Create(HecpRequest.Create(request, Credential.Create(request))).Response();
            Assert.True((int)Status.NoPermission == response.Body.Event.Status, response.Body.Event.Description);
            cmdDto.IsDumb = false;
            //response = request.RequestCenterNode();
            // 使用下面这行可以绕过网络传输从而易于调试，而上面那行需要网络传输
            response = AnyMessage.Create(HecpRequest.Create(cmdDto), NodeHost.Instance.Nodes.ThisNode).Response();
            Assert.True((int)Status.NoPermission == response.Body.Event.Status, response.Body.Event.Description);
        }

        [Fact]
        public void Permission_Level1Action()
        {

        }

        [Fact]
        public void Permission_Level2ElementAction()
        {

        }

        [Fact]
        public void Permission_Level3ClientAction()
        {

        }

        [Fact]
        public void Permission_Level4ClientElementAction()
        {

        }

        [Fact]
        public void Permission_Level5OrganizationAction()
        {

        }

        [Fact]
        public void Permission_Level6EntityAction()
        {

        }

        [Fact]
        public void Permission_Level7EntityElementAction()
        {

        }
        #endregion

        #region Audit
        [Fact]
        public void Audit_Level1Action()
        {

        }

        [Fact]
        public void Audit_Level2ElementAction()
        {

        }

        [Fact]
        public void Audit_Level3ClientAction()
        {

        }

        [Fact]
        public void Audit_Level4ClientElementAction()
        {

        }

        #region Audit_Level5OrganizationAction
        [Fact]
        public void Audit_Level5OrganizationAction()
        {
            var xm = NewXM();
            KeyValue[] infoValue = new KeyValueBuilder(new Dictionary<string, string> {
                {"XM",xm},
                {"ZZJGM","11010000001"}
            }).ToArray();
            var request = new Message
            {
                MessageID = System.Guid.NewGuid().ToString(),
                Version = "v1",
                MessageType = "action",
                IsDumb = true,
                Verb = "Create",
                Ontology = "JS",
                Body = new BodyData(infoValue, infoValue),
                TimeStamp = DateTime.UtcNow.Ticks
            }.JSPXToken();
            var response = request.RequestCenterNode();
            Assert.True((int)Status.ToAudit == response.Body.Event.Status, response.Body.Event.Description);
            request.IsDumb = false;
            response = request.RequestCenterNode();
            Assert.True((int)Status.ToAudit == response.Body.Event.Status, response.Body.Event.Description);
            request.Verb = "update";
            response = request.RequestCenterNode();
            Assert.True((int)Status.NotExist == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        [Fact]
        public void Audit_Level6EntityAction()
        {

        }

        [Fact]
        public void Audit_Level7EntityElementAction()
        {

        }
        #endregion

        #region UpdateLoginName
        [Fact]
        public void UpdateLoginName()
        {
            string localEntityID = Guid.NewGuid().ToString();
            string xm = NewXM();
            var infoID = new KeyValue[] { 
                new KeyValue("ZZJGM", "11010621022"), 
                new KeyValue("XM", xm) 
            };
            var infoValue = infoID;
            var request = new Message()
            {
                Version = ApiVersion.V1.ToName(),
                IsDumb = false,
                MessageType = MessageType.Action.ToName(),
                MessageID = Guid.NewGuid().ToString(),
                Verb = "create",
                Ontology = "JS",
                TimeStamp = DateTime.UtcNow.Ticks,
                Body = new BodyData(infoValue, infoValue),
            }.UIASignature();
            var response = AnyMessage.Create(HecpRequest.Create(request), NodeHost.Instance.Nodes.ThisNode).Response();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            request.Body.InfoID = response.Body.InfoValue;
            request.Verb = "update";
            request.Body.InfoValue = new KeyValue[] { new KeyValue("LoginName", DateTime.Now.Ticks.ToString()) };
            response = AnyMessage.Create(HecpRequest.Create(request), NodeHost.Instance.Nodes.ThisNode).Response();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
            request.Verb = "delete";
            response = AnyMessage.Create(HecpRequest.Create(request), NodeHost.Instance.Nodes.ThisNode).Response();
            Assert.True((int)Status.ExecuteOk == response.Body.Event.Status, response.Body.Event.Description);
        }
        #endregion

        private string NewXM()
        {
            string s = DateTime.Now.ToString("yyMMddHHmmssfff");
            s = s.Substring(2, 12);
            return s;
        }
    }
}
