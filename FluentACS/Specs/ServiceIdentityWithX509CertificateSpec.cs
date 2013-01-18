using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace FluentACS.Specs
{

    public class ServiceIdentityWithX509CertificateSpec
    {
        private string name;

        private List<X509Certificate2> certificates = new List<X509Certificate2>();

        public ServiceIdentityWithX509CertificateSpec Name(string name)
        {
            Guard.NotNullOrEmpty(() => name, name);

            this.name = name;
            return this;
        }

        public ServiceIdentityWithX509CertificateSpec EncryptionCertificate(X509Certificate2 certificate)
        {
            Guard.NotNull(() => certificate, certificate);

            this.certificates.Add(certificate);
            return this;
        }

        public ServiceIdentityWithX509CertificateSpec EncryptionCertificate(string path)
        {
            Guard.NotNullOrEmpty(() => path, path);
            Guard.FileExists(() => path, path);

            return this.EncryptionCertificate(new X509Certificate2(path));
        }

        public ServiceIdentityWithX509CertificateSpec EncryptionCertificateIdentifiedBy(string thumbprint, StoreName storeName, StoreLocation storeLocation)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);

            if (certificates.Count == 0)
                throw new ArgumentException(string.Format("Thumbprint {0} was not found in {1}//{2}", thumbprint, storeLocation, storeName), "thumbprint");

            return this.EncryptionCertificate(certificates[0]);
        }

        public ServiceIdentityWithX509CertificateSpec EncryptionCertificateIdentifiedBy(string thumbprint)
        {
            return this.EncryptionCertificateIdentifiedBy(thumbprint, StoreName.My, StoreLocation.CurrentUser);
        }

        internal string Name()
        {
            return this.name;
        }

        internal IEnumerable<X509Certificate2> Certificates()
        {
            return this.certificates;
        }
    }
}