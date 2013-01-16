using System;
using System.Collections.Generic;

namespace FluentACS.Specs
{
    public class FacebookIdentityProviderSpec : IdentityProviderSpec
    {
        private string appId;

        private string appSecret;

        private readonly List<string> applicationPermissions = new List<string>();

        public FacebookIdentityProviderSpec()
        {
            this.DisplayName(IdentityProviderConsts.Facebook);
            this.applicationPermissions.Add("email");
        }

        public FacebookIdentityProviderSpec AppId(string appId)
        {
            this.appId = appId;
            return this;
        }

        public FacebookIdentityProviderSpec AppSecret(string appSecret)
        {
            this.appSecret = appSecret;
            return this;
        }

        public FacebookIdentityProviderSpec WithApplicationPermission(string permission)
        {
            Guard.NotNullOrEmpty(() => permission, permission);

            if (applicationPermissions.Contains(permission.ToLowerInvariant()))
            {
                throw new InvalidOperationException(string.Format("The permission '{0}' already exists for the Facebook Identity provider.", permission));
            }

            this.applicationPermissions.Add(permission.ToLowerInvariant());
            return this;
        }

        internal string AppId()
        {
            return this.appId;
        }

        internal string AppSecret()
        {
            return this.appSecret;
        }

        internal string[] ApplicationPermissions()
        {
            return this.applicationPermissions.ToArray();
        }
    }
}