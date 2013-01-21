using System;
using System.Linq;

using FluentACS.Logging;
using FluentACS.ManagementService;
using FluentACS.Specs;

namespace FluentACS.Commands
{
    public class AddServiceIdentityWithX509CertificateCommand : BaseCommand
    {
        private readonly ServiceIdentityWithX509CertificateSpec serviceIdentityWithX509Spec;

        public AddServiceIdentityWithX509CertificateCommand(ServiceIdentityWithX509CertificateSpec serviceIdentityWithX509Spec)
        {
            Guard.NotNull(() => serviceIdentityWithX509Spec, serviceIdentityWithX509Spec);

            this.serviceIdentityWithX509Spec = serviceIdentityWithX509Spec;
        }

        public override void Execute(object receiver, Action<LogInfo> logAction)
        {
            var acsWrapper = (ServiceManagementWrapper)receiver;

            var sidToRemove = acsWrapper.RetrieveServiceIdentities().SingleOrDefault(si => si.Name.Equals(this.serviceIdentityWithX509Spec.Name()));
            if (sidToRemove != null)
            {
                this.LogMessage(logAction, string.Format("Removing Service Identity '{0}'", sidToRemove.Name));
                acsWrapper.RemoveServiceIdentity(sidToRemove.Name);
                this.LogSavingChangesMessage(logAction);
            }

            this.LogMessage(logAction, string.Format("Adding Service Identity '{0}'", this.serviceIdentityWithX509Spec.Name()));
            acsWrapper.AddServiceIdentityWithCertificate(this.serviceIdentityWithX509Spec.Name(), this.serviceIdentityWithX509Spec.Certificates());
            this.LogSavingChangesMessage(logAction);
        }
    }
}