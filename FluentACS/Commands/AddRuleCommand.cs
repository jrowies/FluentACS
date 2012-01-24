namespace FluentACS.Commands
{
    using System;

    using FluentACS.Logging;
    using FluentACS.ManagementService;
    using FluentACS.Specs.Rules;

    public class AddRuleCommand : BaseCommand
    {
        private readonly RuleSpec ruleSpec;

        public AddRuleCommand(RuleSpec ruleSpec)
        {
            Guard.NotNull(() => ruleSpec, ruleSpec);

            this.ruleSpec = ruleSpec;
        }

        public override void Execute(object receiver, Action<LogInfo> logAction)
        {
            var acsWrapper = (ServiceManagementWrapper)receiver;

            this.LogMessage(logAction, string.Format("Adding Rule '{0}' to Rule Group '{1}'", this.ruleSpec.Description(), this.ruleSpec.RuleGroupName()));
            acsWrapper.AddSimpleRuleToRuleGroup(
                this.ruleSpec.Description(),
                this.ruleSpec.RuleGroupName(), 
                this.ruleSpec.IdentityProvider(), 
                this.ruleSpec.InputClaimType(),
                this.ruleSpec.InputClaimValue(),
                this.ruleSpec.OutputClaimType(),
                this.ruleSpec.OutputClaimValue());
            this.LogSavingChangesMessage(logAction);
        }
    }
}