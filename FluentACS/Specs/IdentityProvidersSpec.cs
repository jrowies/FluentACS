namespace FluentACS.Specs
{
    using System.Collections.Generic;

    using FluentACS.Commands;

    public class IdentityProvidersSpec : BaseSpec
    {
        public IdentityProvidersSpec(List<ICommand> commands)
            : base(commands)
        {
        }

        public IdentityProvidersSpec AddGoogle()
        {
            this.Commands.Add(new AddIdentityProviderCommand(new GoogleIdentityProviderSpec()));
            return this;
        }

        public IdentityProvidersSpec AddYahoo()
        {
            this.Commands.Add(new AddIdentityProviderCommand(new YahooIdentityProviderSpec()));
            return this;
        }
    }
}