namespace FluentACS.ManagementService
{
    public enum KeyType
    {
        // Recommend not to share symmetric signing key across RPs but configure it on RP instead. 
        Symmetric, 

        X509Certificate, 

        ApplicationKey
    }
}