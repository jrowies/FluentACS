namespace FluentACS.Specs
{
    using System;
    using System.Collections.Generic;

    using FluentACS.Commands;
    using FluentACS.Specs.Rules;
    using FluentACS.Specs.Rules.Chaining;

    public class RuleGroupSpec
    {
        private readonly List<ICommand> commands;

        private readonly string relyingPartyName;

        private string name;

        public RuleGroupSpec(List<ICommand> commands, string relyingPartyName)
        {
            Guard.NotNullOrEmpty(() => relyingPartyName, relyingPartyName);

            this.commands = commands;
            this.relyingPartyName = relyingPartyName;
        }

        public RuleGroupSpec AddRule(Action<IStartRuleSpec> configAction)
        {
            Guard.NotNull(() => configAction, configAction);

            var spec = new RuleSpec(this.Name());
            configAction(spec);

            this.commands.Add(new AddRuleCommand(spec));

            return this;
        }

        public RuleGroupSpec Name(string name)
        {
            Guard.NotNullOrEmpty(() => name, name);

            this.name = name;
            return this;
        }

        internal string Name()
        {
            return this.name;
        }

        internal string RelyingPartyName()
        {
            return this.relyingPartyName;
        }
    }
}