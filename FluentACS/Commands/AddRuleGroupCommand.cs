namespace FluentACS.Commands
{
    using System;
    using System.Data.Services.Client;
    using System.Linq;

    using FluentACS.Logging;
    using FluentACS.ManagementService;
    using FluentACS.Specs;

    public class AddRuleGroupCommand : BaseCommand
    {
        private readonly RuleGroupSpec ruleGroupSpec;

        public AddRuleGroupCommand(RuleGroupSpec ruleGroupSpec)
        {
            Guard.NotNull(() => ruleGroupSpec, ruleGroupSpec);

            this.ruleGroupSpec = ruleGroupSpec;
        }

        public override void Execute(object receiver, Action<LogInfo> logAction)
        {
            var acsWrapper = (ServiceManagementWrapper)receiver;
            var client = acsWrapper.CreateManagementServiceClient();

            var ruleGroup = client.RuleGroups.Where(m => m.Name.Equals(this.ruleGroupSpec.Name())).FirstOrDefault();
            if (ruleGroup != null)
            {
                this.LogMessage(logAction, string.Format("Removing Rule Group '{0}'", this.ruleGroupSpec.Name()));
                client.DeleteObject(ruleGroup);
                client.SaveChanges(SaveChangesOptions.Batch);
                this.LogSavingChangesMessage(logAction);
            }

            this.LogMessage(logAction, string.Format("Adding Rule Group '{0}'", this.ruleGroupSpec.Name()));
            acsWrapper.AddRuleGroup(this.ruleGroupSpec.Name());
            this.LogSavingChangesMessage(logAction);

            this.LogMessage(logAction, string.Format("Linking Rule Group '{0}' to Relying Party '{1}'", this.ruleGroupSpec.Name(), this.ruleGroupSpec.RelyingPartyName()));
            ruleGroup = client.RuleGroups.Where(rg => rg.Name.Equals(this.ruleGroupSpec.Name())).FirstOrDefault();
            var relyingParty = client.RelyingParties.Where(rp => rp.Name.Equals(this.ruleGroupSpec.RelyingPartyName())).FirstOrDefault();
            LinkRuleGroupToRelyingParty(client, ruleGroup, relyingParty);
            this.LogSavingChangesMessage(logAction);
        }

        private static void LinkRuleGroupToRelyingParty(ManagementService client, RuleGroup ruleGroup, RelyingParty relyingParty)
        {
            Guard.NotNull(() => ruleGroup, ruleGroup);
            Guard.NotNull(() => relyingParty, relyingParty);

            var relyingPartyRuleGroup = new RelyingPartyRuleGroup
                {
                    RuleGroupId = ruleGroup.Id,
                    RelyingParty = relyingParty
                };

            client.AddRelatedObject(relyingParty, "RelyingPartyRuleGroups", relyingPartyRuleGroup);
            client.SaveChanges(SaveChangesOptions.Batch);
        }
    }
}