namespace FluentACS.Commands
{
    using System.Linq;

    using FluentACS.ManagementService;
    using FluentACS.Specs;

    public class AddServiceIdentityCommand : ICommand
    {
        private readonly ServiceIdentitySpec serviceIdentitySpec;

        public AddServiceIdentityCommand(ServiceIdentitySpec serviceIdentitySpec)
        {
            this.serviceIdentitySpec = serviceIdentitySpec;
        }

        public void Execute(object receiver)
        {
            var acsWrapper = (ServiceManagementWrapper)receiver;

            var sidToRemove = acsWrapper.RetrieveServiceIdentities().Where(si => si.Name.Equals(this.serviceIdentitySpec.Name())).SingleOrDefault();
            if (sidToRemove != null)
            {
                acsWrapper.RemoveServiceIdentity(sidToRemove.Name);
            }

            acsWrapper.AddServiceIdentity(this.serviceIdentitySpec.Name(), this.serviceIdentitySpec.Password());
        }
    }
}