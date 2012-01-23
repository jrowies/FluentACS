namespace FluentACS.Specs
{
    public class ServiceIdentitySpec
    {
        private string name;

        private string password;

        public ServiceIdentitySpec Name(string name)
        {
            Guard.NotNullOrEmpty(() => name, name);

            this.name = name;
            return this;
        }

        public ServiceIdentitySpec Password(string password)
        {
            Guard.NotNullOrEmpty(() => password, password);

            this.password = password;
            return this;
        }

        internal string Name()
        {
            return this.name;
        }

        internal string Password()
        {
            return this.password;
        }
    }
}