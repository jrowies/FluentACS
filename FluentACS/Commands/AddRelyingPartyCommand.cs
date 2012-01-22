﻿namespace FluentACS.Commands
{
    using System;
    using System.Data.Services.Client;
    using System.Linq;

    using FluentACS.ManagementService;
    using FluentACS.Specs;

    public class AddRelyingPartyCommand : ICommand
    {
        private readonly RelyingPartySpec relyingPartySpec;

        public AddRelyingPartyCommand(RelyingPartySpec relyingPartySpec)
        {
            this.relyingPartySpec = relyingPartySpec;
        }

        public void Execute(object receiver)
        {
            var acsWrapper = (ServiceManagementWrapper)receiver;
            var client = acsWrapper.CreateManagementServiceClient();

            var rpToRemove = acsWrapper.RetrieveRelyingParties().Where(rp => rp.Name.Equals(this.relyingPartySpec.Name())).SingleOrDefault();
            if (rpToRemove != null)
            {
                if (this.relyingPartySpec.ShouldRemoveRelatedRuleGroups())
                {
                    foreach (var ruleGroup in rpToRemove.RelyingPartyRuleGroups)
                    {
                        RelyingPartyRuleGroup @group = ruleGroup;
                        var rgToRemove = client.RuleGroups.Where(rg => rg.Name.Equals(group.RuleGroup.Name)).Single();
                        client.DeleteObject(rgToRemove);
                        client.SaveChanges(SaveChangesOptions.Batch);
                    }
                }

                acsWrapper.RemoveRelyingParty(rpToRemove.Name);
            }

            var tokenLifetime = this.relyingPartySpec.TokenLifetime();

            byte[] signingCertBytes = null;
            string signingCertPassword = null;
            DateTime? signingCertStartDate = null;
            DateTime? signingCertEndDate = null;

            var signingCert = this.relyingPartySpec.SigningCertificate();
            if (signingCert != null)
            {
                signingCertBytes = signingCert.Bytes();
                signingCertPassword = signingCert.Password();
                signingCertStartDate = signingCert.StartDate();
                signingCertEndDate = signingCert.EndDate();
            }

            acsWrapper.AddRelyingPartyWithKey(
                this.relyingPartySpec.Name(),
                this.relyingPartySpec.RealmAddress(),
                this.relyingPartySpec.ReplyAddress(),
                this.relyingPartySpec.SymmetricKey(),
                this.relyingPartySpec.GetTokenType(),
                (tokenLifetime == 0 ? Constants.RelyingPartyTokenLifetime : tokenLifetime),
                signingCertBytes,
                signingCertPassword,
                signingCertStartDate,
                signingCertEndDate,
                this.relyingPartySpec.EncryptionCertificate(),
                string.Empty,
                this.relyingPartySpec.AllowedIdentityProviders().ToArray());

            foreach (var linkedRuleGroup in this.relyingPartySpec.LinkedRuleGroups())
            {
                var @group = linkedRuleGroup;
                var ruleGroup = client.RuleGroups.Where(rg => rg.Name.Equals(group)).Single();

                var relyingParty = client.RelyingParties.Where(rp => rp.Name.Equals(this.relyingPartySpec.Name())).Single();

                var relyingPartyRuleGroup = new RelyingPartyRuleGroup
                {
                    RuleGroupId = ruleGroup.Id,
                    RelyingParty = relyingParty
                };

                client.AddRelatedObject(relyingParty, "RelyingPartyRuleGroups", relyingPartyRuleGroup);
            }

            if (this.relyingPartySpec.LinkedRuleGroups().Any())
            {
                client.SaveChanges(SaveChangesOptions.Batch);
            }
        }
    }
}