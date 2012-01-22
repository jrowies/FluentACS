namespace FluentACS.ManagementService
{
    public static class Constants
    {
        public const string AcsHostName = "accesscontrol.windows.net";
        public const string ManagementServiceHead = "v2/mgmt/service/";
        public const string MetadataImportHead = "v2/mgmt/service/importFederationMetadata/importIdentityProvider";

        // 1 hour = 3600 seconds
        public const int RelyingPartyTokenLifetime = 3600;
    }
}