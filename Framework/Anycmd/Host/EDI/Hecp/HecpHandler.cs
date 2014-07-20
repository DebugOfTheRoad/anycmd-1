﻿
namespace Anycmd.Host.EDI.Hecp
{
    using Exceptions;
    using Handlers;
    using System;
    using System.Collections.Generic;
    using Util;

    /// <summary>
    /// Hecp处理程序，它是一种处理HecpContext的资源<see cref="IWfResource"/>
    /// </summary>
    public sealed class HecpHandler : IWfResource
    {
        private readonly Guid _id = new Guid("F854A771-4AE2-4235-B4E6-5EBEA5420FFE");
        private readonly string _name = "DefaultHecpHandler";
        private readonly string _description = "Hecp处理程序";
        private readonly HashSet<string> _versionSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            ApiVersion.V1.ToName()
        };
        private readonly IAppHost host;

        /// <summary>
        /// 
        /// </summary>
        public HecpHandler(IAppHost host)
        {
            this.host = host;
        }

        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public BuiltInResourceKind BuiltInResourceKind
        {
            get { return BuiltInResourceKind.HecpHandler; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return _description; }
        }
        #endregion

        #region 处理单条命令 Process
        /// <summary>
        /// 处理单条命令
        /// </summary>
        /// <param name="context"></param>
        public void Process(HecpContext context)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
                if (!context.IsValid)
                {
                    return;
                }
                if (!_versionSet.Contains(context.Request.Version))
                {
                    throw new CoreException("本Hecp处理程序不支持处理版本号" + context.Request.Version + "的消息");
                }
                // ApplyPreRequestFilters
                ProcessResult result = host.ApplyPreHecpRequestFilters(context);
                context.Response.Body.Event.Status = (int)result.StateCode;
                context.Response.Body.Event.Description = result.Description;
                if (context.Response.IsClosed)
                {
                    return;
                }
                #region 身份认证
                var author = host.GetRequiredService<IAuthenticator>();
                if (author == null)
                {
                    throw new CoreException("未配置证书验证器，证书验证器是必须的。");
                }
                using (var act = new WfAct(host, context, author, "验证身份"))
                {
                    result = author.Auth(context.Request);
                }
                if (!result.IsSuccess)
                {
                    context.Response.Body.Event.Status = (int)result.StateCode;
                    context.Response.Body.Event.ReasonPhrase = result.StateCode.ToName();
                    context.Response.Body.Event.Description = result.Description;
                    return;
                }
                #endregion
                var commandContext = MessageContext.Create(host, context);
                MessageHandler.Instance.Response(commandContext);
                context.Response.Fill(commandContext.Result);
                // ApplyResponseFilters
                result = host.ApplyHecpResponseFilters(context);
                context.Response.Body.Event.Status = (int)result.StateCode;
                context.Response.Body.Event.ReasonPhrase = result.StateCode.ToName();
                context.Response.Body.Event.Description = result.Description;
                if (context.Response.IsClosed)
                {
                    return;
                }
            }
            catch
            {
                context.Response.Body.Event.Description = "服务器内部逻辑异常";
                context.Response.Body.Event.Status = 500;
                context.Response.Body.Event.ReasonPhrase = Status.InternalServerError.ToName();
                throw;
            }
        }
        #endregion
    }
}
