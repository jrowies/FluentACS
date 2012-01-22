namespace FluentACS.Commands
{
    using System.Linq;

    using FluentACS.ManagementService;
    using FluentACS.Specs;

    public class AddIdentityProviderCommand : ICommand
    {
        private readonly IdentityProviderSpec identityProviderSpec;

        public AddIdentityProviderCommand(IdentityProviderSpec identityProviderSpec)
        {
            this.identityProviderSpec = identityProviderSpec;
        }

        public void Execute(object receiver)
        {
            var acsWrapper = (ServiceManagementWrapper)receiver;

            var idpToRemove = acsWrapper.RetrieveIdentityProviders().Where(idp => idp.DisplayName.Equals(this.identityProviderSpec.DisplayName())).SingleOrDefault();
            if (idpToRemove != null)
            {
                acsWrapper.RemoveIdentityProvider(idpToRemove.DisplayName);
            }

            if (this.identityProviderSpec is GoogleIdentityProviderSpec)
            {
                acsWrapper.AddGoogleIdentityProvider();
            }

            if (this.identityProviderSpec is YahooIdentityProviderSpec)
            {
                acsWrapper.AddYahooIdentityProvider();
            }
        }
    }
}