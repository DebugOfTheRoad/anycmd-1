
namespace Anycmd.Model
{
    using System;

    public class EntityCreateInput : ManagedPropertyValues, IEntityCreateInput
    {
        private Guid? _id = null;

        public Guid? Id
        {
            get
            {
                if (_id == null)
                {
                    _id = Guid.NewGuid();
                }
                return _id;
            }
            set { _id = value; }
        }

    }
}
