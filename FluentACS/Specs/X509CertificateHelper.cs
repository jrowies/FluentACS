namespace FluentACS.Specs
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    public class X509CertificateHelper
    {
        public static X509Certificate2 GetX509Certificate(string thumbprint, StoreName storeName, StoreLocation storeLocation)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);

            if (certificates.Count == 0)
            {
                throw new ArgumentException(
                    string.Format("Thumbprint {0} was not found in {1}//{2}", thumbprint, storeLocation, storeName),
                    "thumbprint");
            }

            var certificate = certificates[0];
            return certificate;
        }
    }
}