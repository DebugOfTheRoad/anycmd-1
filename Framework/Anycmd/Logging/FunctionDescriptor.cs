using System;

namespace Anycmd.Logging
{

    public class FunctionDescriptor
    {
        public Guid AccountID { get; set; }

        public string LoginName { get; set; }

        public string UserName { get; set; }

        public Guid AppSystemID { get; set; }

        public string AppSystemName { get; set; }

        public Guid ResourceTypeID { get; set; }
        public string ResourceName { get; set; }
        public Guid FunctionID { get; set; }
        public string FunctionDescription { get; set; }
        public Guid EntityTypeID { get; set; }
        public string EntityTypeName { get; set; }

        public DateTime CreateOn { get; set; }

        public string IPAddress { get; set; }
    }
}
