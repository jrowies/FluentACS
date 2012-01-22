namespace FluentACS
{
    public class AcsNamespaceDescription
    {
        public AcsNamespaceDescription(string acsNamespace, string acsUserName, string acsPassword)
        {
            this.AcsNamespace = acsNamespace;
            this.AcsUserName = acsUserName;
            this.AcsPassword = acsPassword;
        }

        public string AcsNamespace { get; private set; }

        public string AcsUserName { get; private set; }

        public string AcsPassword { get; private set; }
    }
}