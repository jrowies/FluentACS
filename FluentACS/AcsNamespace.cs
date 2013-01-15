namespace FluentACS
{
    using System;
    using System.Collections.Generic;

    using FluentACS.Commands;
    using FluentACS.Logging;
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
        }

        public AcsNamespace AddGoogleIdentityProvider()
        {
            this.commands.Add(new AddIdentityProviderCommand(new GoogleIdentityProviderSpec()));
            return this;
        }

        public AcsNamespace AddYahooIdentityProvider()
        {
            this.commands.Add(new AddIdentityProviderCommand(new YahooIdentityProviderSpec()));
            return this;
        }

        public AcsNamespace AddFacebookIdentityProvider(Action<FacebookIdentityProviderSpec> configAction)
        {
            Guard.NotNull(() => configAction, configAction);

            var spec = new FacebookIdentityProviderSpec();
            configAction(spec);

            this.commands.Add(new AddIdentityProviderCommand(spec));

            return this;
        }

        public AcsNamespace AddServiceIdentity(Action<ServiceIdentitySpec> configAction)
        {
            Guard.NotNull(() => configAction, configAction);

            var spec = new ServiceIdentitySpec();
            configAction(spec);

            this.commands.Add(new AddServiceIdentityCommand(spec));

            return this;
        }

        public AcsNamespace AddServiceIdentityWithX509Certificate(Action<ServiceIdentityWithX509CertificateSpec> configAction)
        {
            Guard.NotNull(() => configAction, configAction);

            var spec = new ServiceIdentityWithX509CertificateSpec();
            configAction(spec);

            this.commands.Add(new AddServiceIdentityWithX509CertificateCommand(spec));

            return this;
        }

        public AcsNamespace AddRelyingParty(Action<RelyingPartySpec> configAction)
        {
            Guard.NotNull(() => configAction, configAction);

            var cmds = new List<ICommand>();
            var spec = new RelyingPartySpec(cmds);
            configAction(spec);

            this.commands.Add(new AddRelyingPartyCommand(spec));
            this.commands.AddRange(cmds);

            return this;
        }

        public void SaveChanges(Action<LogInfo> logAction)
        {
            try
            {
                var managementWrapper = new ServiceManagementWrapper(this.namespaceDesc.Namespace, this.namespaceDesc.UserName, this.namespaceDesc.Password);

                CheckConnection(managementWrapper);

                foreach (var command in this.commands)
                {
                    try
                    {
                        command.Execute(managementWrapper, logAction);
                    }
                    catch (Exception ex1)
                    {
                        if (!LogException(logAction, LogInfoTypeEnum.Error, ex1))
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                if (!LogException(logAction, LogInfoTypeEnum.FatalError, ex2))
                {
                    throw;
                }
            }
        }

        private static bool LogException(Action<LogInfo> logAction, LogInfoTypeEnum logInfoType, Exception exception)
        {
            if (logAction != null)
            {
                logAction(new LogInfo { LogInfoType = LogInfoTypeEnum.Error, Exception = exception });
                return true;
            }

            return false;
        }

        public void SaveChanges()
        {
            this.SaveChanges(null);
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