namespace FluentACS.ManagementService
{
    public enum IdentityKeyTypes
    {
        // X509Certificate service identity is not supported by ACSv2 yet, although database scheme allows it.
        X509Certificate, 

        Password, 

        // Used for supporting Wrap Profile 5.2 - SWT assertion. 
        // Instead of sending username and password, clients can send a signed SWT assertion in a request. 
        Symmetric
    }
}