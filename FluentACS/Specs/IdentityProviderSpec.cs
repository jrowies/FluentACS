namespace FluentACS.Specs
{
    public class IdentityProviderSpec
    {
        private string displayName;

        public void DisplayName(string name)
        {
            Guard.NotNullOrEmpty(() => name, name);

            this.displayName = name;
        }

        internal string DisplayName()
        {
            return this.displayName;
        }
    }
}