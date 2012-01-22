using System.Linq;
using System.Collections.Generic;

namespace FluentACS.ManagementService
{
    public struct SocialIdentityProvider
    {
        public string DisplayName { get; set; }
        public string HomeRealm { get; set; }
        public string Id { get; set; }
    }


    public static class SocialIdentityProviders
    {
        public static readonly SocialIdentityProvider Google = new SocialIdentityProvider { DisplayName = "Google", HomeRealm = "Google", Id = "10008641" };
        public static readonly SocialIdentityProvider WindowsLiveId = new SocialIdentityProvider { DisplayName = "Windows Live ID", HomeRealm = "uri:WindowsLiveID", Id = "10007989" };
        public static readonly SocialIdentityProvider Yahoo = new SocialIdentityProvider { DisplayName = "Yahoo!", HomeRealm = "Yahoo!", Id = "10008653" };
        
        public static IEnumerable<SocialIdentityProvider> GetAll()
        {
            return new[] { Google, Yahoo, WindowsLiveId };
        }

        public static string GetHomeRealm(string socialIpId)
        {
            var providers = new[] { Google, Yahoo, WindowsLiveId };
            return providers.Single(p => p.Id == socialIpId).HomeRealm;
        }

        public static bool IsSocial(this IdentityProvider ip)
        {
            if (ip.Issuer.Name.Contains(Google.HomeRealm) ||
                ip.Issuer.Name.Contains(Yahoo.HomeRealm) ||
                ip.Issuer.Name.Contains(WindowsLiveId.HomeRealm))
            {
                return true;
            }
            return false;
        }
    }
}