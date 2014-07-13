
namespace Anycmd.Ef
{
    using Host;
    using System;
    using System.Web;

    /// <summary>
    /// 
    /// </summary>
    public sealed class WebEfContextStorage : IEfContextStorage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public WebEfContextStorage(HttpApplication app)
        {
            app.EndRequest += Application_EndRequest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void SetRepositoryContext(EfRepositoryContext context)
        {
            var storage = GetSimpleSessionStorage();
            storage.SetRepositoryContext(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public EfRepositoryContext GetRepositoryContext(string key)
        {
            var storage = GetSimpleSessionStorage();
            return storage.GetRepositoryContext(key);
        }

        private static void Application_EndRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var storage = context.Items[ConstKeys.WEB_EFDBCONTEXT_STORAGEKEY] as SimpleEfContextStorage;
            if (storage != null)
            {
                storage.Dispose();
                context.Items.Remove(ConstKeys.WEB_EFDBCONTEXT_STORAGEKEY);
            }
        }

        private static SimpleEfContextStorage GetSimpleSessionStorage()
        {
            var httpContext = HttpContext.Current;
            var storage = httpContext.Items[ConstKeys.WEB_EFDBCONTEXT_STORAGEKEY] as SimpleEfContextStorage;
            if (storage == null)
            {
                storage = new SimpleEfContextStorage();
                httpContext.Items[ConstKeys.WEB_EFDBCONTEXT_STORAGEKEY] = storage;
            }

            return storage;
        }
    }
}