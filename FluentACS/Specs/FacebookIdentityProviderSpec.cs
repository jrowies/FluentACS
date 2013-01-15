namespace FluentACS.Specs
{
    public class FacebookIdentityProviderSpec : IdentityProviderSpec
    {
        private string appId;

        private string appSecret;

        public FacebookIdentityProviderSpec()
        {
            this.DisplayName(IdentityProviderConsts.Facebook);
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

        internal string AppId()
        {
            return this.appId;
        }

        internal string AppSecret()
        {
            return this.appSecret;
        }
    }
}