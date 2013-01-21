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

        public ServiceIdentityWithX509CertificateSpec EncryptionCertificateIdentifiedBy(string thumbprint, StoreName storeName = StoreName.My, StoreLocation storeLocation = StoreLocation.CurrentUser)
        {
            var certificate = X509CertificateHelper.GetX509Certificate(thumbprint, storeName, storeLocation);
            return this.EncryptionCertificate(certificate);
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