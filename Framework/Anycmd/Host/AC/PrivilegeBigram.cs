
namespace Anycmd.Host.AC
{
    using Anycmd.AC;
    using Model;
    using ValueObjects;

    public class PrivilegeBigram : PrivilegeBigramBase, IAggregateRoot
    {
        public PrivilegeBigram() { }

        public static PrivilegeBigram Create(IPrivilegeBigramCreateInput input)
        {
            return new PrivilegeBigram
            {
                Id = input.Id.Value,
                SubjectType = input.SubjectType,
                SubjectInstanceID = input.SubjectInstanceID,
                ObjectType = input.ObjectType,
                ObjectInstanceID = input.ObjectInstanceID,
                PrivilegeConstraint = input.PrivilegeConstraint,
                PrivilegeOrientation = input.PrivilegeOrientation
            };
        }

        public void Update(IPrivilegeBigramUpdateInput input)
        {
            this.PrivilegeConstraint = input.PrivilegeConstraint;
        }
    }
}
