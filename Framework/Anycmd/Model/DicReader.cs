
namespace Anycmd.Model
{
    using System;
    using System.Collections.Generic;

    public sealed class DicReader : Dictionary<string, object>
    {
        public DicReader(IAppHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            this.Host = host;
        }

        public IAppHost Host { get; private set; }
    }
}
