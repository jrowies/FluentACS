namespace FluentACS.Specs.Rules
{
    using FluentACS.Specs.Rules.Chaining;

    public class RuleSpec : IStartRuleSpec, 
                            IAfterDescription, 
                            IAfterIfInputClaimIssuerRuleSpec, 
                            IAfterAndInputClaimTypeRuleSpec, 
                            IAfterAndInputClaimValueRuleSpec, 
                            IAfterThenOutputClaimTypeRuleSpec, 
                            IAfterAndOutputClaimValueRuleSpec
    {
        private readonly string ruleGroupName;

        private readonly InputClaimIssuerSpec inputClaimIssuerSpec;

        private readonly InputClaimTypeSpec inputClaimTypeSpec;

        private readonly InputClaimValueSpec inputClaimValueSpec;

        private readonly OutputClaimTypeSpec outputClaimTypeSpec;

        private readonly OutputClaimValueSpec outputClaimValueSpec;

        private string description;

        private string identityProviderName;
        
        private string inputClaimType;
        
        private string inputClaimValue;
        
        private string outputClaimType;

        private string outputClaimValue;

        public RuleSpec(string ruleGroupName)
        {
            this.ruleGroupName = ruleGroupName;
            this.inputClaimIssuerSpec = new InputClaimIssuerSpec(this);
            this.inputClaimTypeSpec = new InputClaimTypeSpec(this);
            this.inputClaimValueSpec = new InputClaimValueSpec(this);
            this.outputClaimTypeSpec = new OutputClaimTypeSpec(this);
            this.outputClaimValueSpec = new OutputClaimValueSpec(this);
        }

        public IAfterDescription Description(string description)
        {
            this.description = description;
            return this;
        }

        public InputClaimIssuerSpec IfInputClaimIssuer()
        {
            return this.inputClaimIssuerSpec;
        }

        public InputClaimTypeSpec AndInputClaimType()
        {
            return this.inputClaimTypeSpec;
        }

        public InputClaimValueSpec AndInputClaimValue()
        {
            return this.inputClaimValueSpec;
        }

        public OutputClaimTypeSpec ThenOutputClaimType()
        {
            return this.outputClaimTypeSpec;
        }

        public OutputClaimValueSpec AndOutputClaimValue()
        {
            return this.outputClaimValueSpec;
        }

        internal string Description()
        {
            return this.description;
        }

        internal void IdentityProvider(string identityProviderName)
        {
            this.identityProviderName = identityProviderName;
        }

        internal string IdentityProvider()
        {
            return this.identityProviderName;
        }

        internal void InputClaimType(string claimType)
        {
            this.inputClaimType = claimType;
        }

        internal string InputClaimType()
        {
            if (this.inputClaimType.Equals(BaseSpec.Any))
            {
                return null;
            }

            return this.inputClaimType;
        }

        internal void InputClaimValue(string claimType)
        {
            this.inputClaimValue = claimType;
        }

        internal string InputClaimValue()
        {
            if (this.inputClaimValue.Equals(BaseSpec.Any))
            {
                return null;
            }

            return this.inputClaimValue;
        }

        internal void OutputClaimType(string claimType)
        {
            this.outputClaimType = claimType;
        }

        internal string OutputClaimType()
        {
            if (this.outputClaimType.Equals(BaseSpec.Passthrough))
            {
                return null;
            }

            return this.outputClaimType;
        }

        internal void OutputClaimValue(string claimType)
        {
            this.outputClaimValue = claimType;
        }

        internal string OutputClaimValue()
        {
            if (this.outputClaimValue.Equals(BaseSpec.Passthrough))
            {
                return null;
            }

            return this.outputClaimValue;
        }

        internal string RuleGroupName()
        {
            return this.ruleGroupName;
        }
    }
}