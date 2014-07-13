
namespace Anycmd.EDI.MessageServices {
    using Anycmd.Host.EDI.Hecp;
    using DataContracts;
    using ServiceModel.Operations;
    using System;

    /// <summary>
    /// 提供<see cref="HecpResponse"/>转化为<see cref="CommandResponse"/>的扩展方法。
    /// </summary>
    public static class HecpResponseExtension {
        /// <summary>
        /// 转化为命令请求。
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Message ToMessage(this HecpResponse response) {
            if (response == null) {
                throw new ArgumentNullException("response");
            }
            var r = new Message {
                MessageType = response.MessageType,
                MessageID = response.MessageID,
                Verb = response.Verb,
                Credential = ((IMessageDto)response).Credential,
                Body = response.Body,
                IsDumb = response.IsDumb,
                Ontology = response.Ontology,
                TimeStamp = response.TimeStamp,
                Version = response.Version
            };

            return r;
        }
    }
}
