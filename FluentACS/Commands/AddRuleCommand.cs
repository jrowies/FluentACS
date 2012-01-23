namespace FluentACS.Commands
{
    using FluentACS.ManagementService;
    using FluentACS.Specs.Rules;

    public class AddRuleCommand : ICommand
    {
        private readonly RuleSpec ruleSpec;

        public AddRuleCommand(RuleSpec ruleSpec)
        {
            Guard.NotNull(() => ruleSpec, ruleSpec);

            this.ruleSpec = ruleSpec;
        }

        public void Execute(object receiver)
        {
            var acsWrapper = (ServiceManagementWrapper)receiver;

            acsWrapper.AddSimpleRuleToRuleGroup(
                this.ruleSpec.Description(),
                this.ruleSpec.RuleGroupName(), 
                this.ruleSpec.IdentityProvider(), 
                this.ruleSpec.InputClaimType(),
                this.ruleSpec.InputClaimValue(),
                this.ruleSpec.OutputClaimType(),
                this.ruleSpec.OutputClaimValue());
        }
    }
}