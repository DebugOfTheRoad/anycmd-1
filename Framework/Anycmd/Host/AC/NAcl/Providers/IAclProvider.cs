using System.Collections.Generic;

namespace Anycmd.Host.AC.NAcl.Providers
{
    public interface IAclProvider
    {
        IEnumerable<AccessRule> GetAcls(string resource, string verb);
        IEnumerable<AccessRule> GetAclsBySubject(params string[] subjects);
        IAclProvider SetAcls(params AccessRule[] acls);
        IAclProvider DeleteAcls(params AccessRule[] acls);
        IAclProvider DeleteAcls(string resource, params string[] subjects);

        event AclChangedHandler AclChanged;
    }
}
