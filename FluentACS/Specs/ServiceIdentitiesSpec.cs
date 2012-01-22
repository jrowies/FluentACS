namespace FluentACS.Specs
{
    using System;
    using System.Collections.Generic;

    using FluentACS.Commands;

    public class ServiceIdentitiesSpec : BaseSpec
    {
        public ServiceIdentitiesSpec(List<ICommand> commands)
            : base(commands)
        {
        }

        public ServiceIdentitiesSpec Add(Action<ServiceIdentitySpec> configAction)
        {
            var spec = new ServiceIdentitySpec();
            configAction(spec);

            this.Commands.Add(new AddServiceIdentityCommand(spec));

            return this;
        }
    }
}