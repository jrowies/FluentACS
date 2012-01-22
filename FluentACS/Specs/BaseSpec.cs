namespace FluentACS.Specs
{
    using System.Collections.Generic;

    using FluentACS.Commands;

    public class BaseSpec
    {
        internal const string Any = "_ANY";

        internal const string Passthrough = "_PASSTHROUGH";

        internal const string Acs = "LOCAL AUTHORITY";

        public BaseSpec(List<ICommand> commands)
        {
            this.Commands = commands;
        }

        protected List<ICommand> Commands { get; private set; }
    }
}