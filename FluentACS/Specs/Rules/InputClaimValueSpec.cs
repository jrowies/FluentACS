namespace FluentACS.Specs.Rules
{
    using FluentACS.Specs.Rules.Chaining;

    public class InputClaimValueSpec
    {
        private readonly RuleSpec owner;

        public InputClaimValueSpec(RuleSpec owner)
        {
            this.owner = owner;
        }

        public IAfterAndInputClaimValueRuleSpec IsAny()
        {
            this.owner.InputClaimValue(BaseSpec.Any);
            return this.owner;
        }

        public IAfterAndInputClaimValueRuleSpec Is(string inputClaimValue)
        {
            this.owner.InputClaimValue(inputClaimValue);
            return this.owner;
        }
    }
}