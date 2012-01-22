namespace FluentACS.Specs.Rules
{
    using FluentACS.Specs.Rules.Chaining;

    public class OutputClaimValueSpec
    {
        private readonly RuleSpec owner;

        public OutputClaimValueSpec(RuleSpec owner)
        {
            this.owner = owner;
        }

        public IAfterAndOutputClaimValueRuleSpec ShouldPassthroughFirstInputClaimValue()
        {
            this.owner.OutputClaimValue(BaseSpec.Passthrough);
            return this.owner;
        }

        public IAfterAndOutputClaimValueRuleSpec ShouldBe(string outputClaimValue)
        {
            this.owner.OutputClaimValue(outputClaimValue);
            return this.owner;
        }
    }
}