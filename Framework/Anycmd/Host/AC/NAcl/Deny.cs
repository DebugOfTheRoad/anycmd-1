
namespace Anycmd.Host.AC.NAcl
{
    public class Deny : AccessRule
    {
        public Deny(string resource, string verb, string subject)
            : base(resource, verb,subject)
        {

        }

        public Deny()
        {

        }

        public override AccessRules Type
        {
            get { return AccessRules.Deny; }
        }
    }
}
