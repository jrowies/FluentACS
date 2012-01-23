namespace FluentACS.Commands
{
    using System.Data.Services.Client;
    using System.Linq;

    using FluentACS.ManagementService;
    using FluentACS.Specs;

    public class AddRuleGroupCommand : ICommand
    {
        private readonly RuleGroupSpec ruleGroupSpec;

        public AddRuleGroupCommand(RuleGroupSpec ruleGroupSpec)
        {
            Guard.NotNull(() => ruleGroupSpec, ruleGroupSpec);

            this.ruleGroupSpec = ruleGroupSpec;
        }

        public virtual void Execute(object receiver)
        {
            var acsWrapper = (ServiceManagementWrapper)receiver;
            var client = acsWrapper.CreateManagementServiceClient();

            var ruleGroup = client.RuleGroups.Where(m => m.Name.Equals(this.ruleGroupSpec.Name())).FirstOrDefault();
            if (ruleGroup != null)
            {
                client.DeleteObject(ruleGroup);
                client.SaveChanges(SaveChangesOptions.Batch);
            }

            acsWrapper.AddRuleGroup(this.ruleGroupSpec.Name());
            ruleGroup = client.RuleGroups.Where(rg => rg.Name.Equals(this.ruleGroupSpec.Name())).FirstOrDefault();

            var relyingParty = client.RelyingParties.Where(rp => rp.Name.Equals(this.ruleGroupSpec.RelyingPartyName())).FirstOrDefault();

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