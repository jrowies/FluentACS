namespace FluentACS.Specs
{
    using System;
    using System.Collections.Generic;

    using FluentACS.Commands;

    public class RelyingPartiesSpec : BaseSpec
    {
        public RelyingPartiesSpec(List<ICommand> commands)
            : base(commands)
        {
        }

        public RelyingPartiesSpec Add(Action<RelyingPartySpec> configAction)
        {
            var cmds = new List<ICommand>();
            var spec = new RelyingPartySpec(cmds);
            configAction(spec);
            
            this.Commands.Add(new AddRelyingPartyCommand(spec));
            this.Commands.AddRange(cmds);

            return this;
        }
    }
}