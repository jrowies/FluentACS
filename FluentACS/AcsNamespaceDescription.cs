namespace FluentACS
{
    public class AcsNamespaceDescription
    {
        public AcsNamespaceDescription(string acsNamespace, string userName, string password)
        {
            Guard.NotNullOrEmpty(() => acsNamespace, acsNamespace);
            Guard.NotNullOrEmpty(() => userName, userName);
            Guard.NotNullOrEmpty(() => password, password);

            this.Namespace = acsNamespace;
            this.UserName = userName;
            this.Password = password;
        }

        public string Namespace { get; private set; }

        public string UserName { get; private set; }

        public string Password { get; private set; }
    }
}