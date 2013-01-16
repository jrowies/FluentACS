namespace FluentACS.Commands
{
    using System;
    using System.Linq;

    using FluentACS.Logging;
    using FluentACS.ManagementService;
    using FluentACS.Specs;

    public class AddIdentityProviderCommand : BaseCommand
    {
        private readonly IdentityProviderSpec identityProviderSpec;

        public AddIdentityProviderCommand(IdentityProviderSpec identityProviderSpec)
        {
            Guard.NotNull(() => identityProviderSpec, identityProviderSpec);

            this.identityProviderSpec = identityProviderSpec;
        }

        public override void Execute(object receiver, Action<LogInfo> logAction)
        {
            // AddIdentityProviderCommand has branches for each of
            // the spec types. At present this is at the limit of its
            // scalability;
            
            // TODO: consider refactoring to reduce the cyclomatic complexity of the Execute method.

            var acsWrapper = (ServiceManagementWrapper)receiver;

            var idpToRemove = acsWrapper.RetrieveIdentityProviders().Where(idp => idp.DisplayName.Equals(this.identityProviderSpec.DisplayName())).SingleOrDefault();
            if (idpToRemove != null)
            {
                this.LogMessage(logAction, string.Format("Removing Identity Provider '{0}'", idpToRemove.DisplayName));
                acsWrapper.RemoveIdentityProvider(idpToRemove.DisplayName);
                this.LogSavingChangesMessage(logAction);
            }

            if (this.identityProviderSpec is GoogleIdentityProviderSpec)
            {
                this.LogMessage(logAction, string.Format("Adding Identity Provider '{0}'", this.identityProviderSpec.DisplayName()));
                acsWrapper.AddGoogleIdentityProvider();
                this.LogSavingChangesMessage(logAction);
            }

            if (this.identityProviderSpec is YahooIdentityProviderSpec)
            {
                this.LogMessage(logAction, string.Format("Adding Identity Provider '{0}'", this.identityProviderSpec.DisplayName()));
                acsWrapper.AddYahooIdentityProvider();
                this.LogSavingChangesMessage(logAction);
            }

            if (this.identityProviderSpec is FacebookIdentityProviderSpec)
            {
                var facebookIdentityProviderSpec = (FacebookIdentityProviderSpec)identityProviderSpec;
                this.LogMessage(logAction, string.Format("Adding Identity Provider '{0}'", facebookIdentityProviderSpec.DisplayName()));
                acsWrapper.AddFacebookIdentityProvider(facebookIdentityProviderSpec.DisplayName(),
                    facebookIdentityProviderSpec.AppId(), 
                    facebookIdentityProviderSpec.AppSecret(),
                    facebookIdentityProviderSpec.ApplicationPermissions());
                this.LogSavingChangesMessage(logAction);
            }
        }
    }
}