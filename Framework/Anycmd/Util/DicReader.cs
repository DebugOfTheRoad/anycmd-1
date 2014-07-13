using System;
using System.Collections.Generic;

namespace Anycmd.Util
{
    public sealed class DicReader : Dictionary<string, object>
    {
        public DicReader(AppHost appHost)
        {
            if (appHost == null)
            {
                throw new ArgumentNullException("appHost");
            }
            this.AppHost = appHost;
        }

        public AppHost AppHost { get; private set; }
    }
}
