namespace FluentACS
{
    using System;
    using System.Collections.Generic;

    using FluentACS.Commands;
    using FluentACS.ManagementService;
    using FluentACS.Specs;

    public class AcsNamespace
    {
        private readonly AcsNamespaceDescription namespaceDesc;

        private readonly List<ICommand> commands;

        public AcsNamespace(AcsNamespaceDescription namespaceDesc)
        {
            Guard.NotNull(() => namespaceDesc, namespaceDesc);

            this.namespaceDesc = namespaceDesc;
            this.commands = new List<ICommand>();
            this.IdentityProviders = new IdentityProvidersSpec(this.commands);
            this.ServiceIdentities = new ServiceIdentitiesSpec(this.commands);
            this.RelyingParties = new RelyingPartiesSpec(this.commands);
        }

        public IdentityProvidersSpec IdentityProviders { get; private set; }

        public ServiceIdentitiesSpec ServiceIdentities { get; private set; }

        public RelyingPartiesSpec RelyingParties { get; private set; }

        public void SaveChanges()
        {
            var managementWrapper = new ServiceManagementWrapper(this.namespaceDesc.Namespace, this.namespaceDesc.UserName, this.namespaceDesc.Password);

            CheckConnection(managementWrapper);

            foreach (var command in this.commands)
            {
                command.Execute(managementWrapper);
            }
        }

        private static void CheckConnection(ServiceManagementWrapper managementWrapper)
        {
            try
            {
                managementWrapper.GetTokenFromACS();
            }
            catch (Exception ex)
            {
                throw new Exception("A token could not be retrieved from ACS, there might be connection problems or errors in the configuration." + 
                    Environment.NewLine + "Please check the namespace, username and password provided.", ex);
            }
        }
    }
}