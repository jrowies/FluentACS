namespace FluentACS.Commands
{
    using System;
    using System.Linq;

    using FluentACS.Logging;
    using FluentACS.ManagementService;
    using FluentACS.Specs;

    public class AddServiceIdentityCommand : BaseCommand
    {
        private readonly ServiceIdentitySpec serviceIdentitySpec;

        public AddServiceIdentityCommand(ServiceIdentitySpec serviceIdentitySpec)
        {
            Guard.NotNull(() => serviceIdentitySpec, serviceIdentitySpec);

            this.serviceIdentitySpec = serviceIdentitySpec;
        }

        public override void Execute(object receiver, Action<LogInfo> logAction)
        {
            var acsWrapper = (ServiceManagementWrapper)receiver;

            var sidToRemove = acsWrapper.RetrieveServiceIdentities().Where(si => si.Name.Equals(this.serviceIdentitySpec.Name())).SingleOrDefault();
            if (sidToRemove != null)
            {
                this.LogMessage(logAction, string.Format("Removing Service Identity '{0}'", sidToRemove.Name));
                acsWrapper.RemoveServiceIdentity(sidToRemove.Name);
                this.LogSavingChangesMessage(logAction);
            }

            this.LogMessage(logAction, string.Format("Adding Service Identity '{0}'", this.serviceIdentitySpec.Name()));
            acsWrapper.AddServiceIdentity(this.serviceIdentitySpec.Name(), this.serviceIdentitySpec.Password());
            this.LogSavingChangesMessage(logAction);
        }
    }
}