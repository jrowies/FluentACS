namespace FluentACS.ManagementService
{
    public enum KeyUsage
    {
        // Used for signing tokens issued to RPs. 
        Signing, 

        // Used for decrypting tokens issued by IDPs. 
        Encrypting, 

        ApplicationId, 

        ApplicationSecret
    }
}