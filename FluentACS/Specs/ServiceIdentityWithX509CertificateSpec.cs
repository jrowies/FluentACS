using System;
using FluentACS;

namespace FluentACS.Specs
{
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    public class ServiceIdentityWithX509CertificateSpec
    {
        private byte[] encryptionCert;

        private string name;

        private DateTime startDate;

        private DateTime endDate;

        public ServiceIdentityWithX509CertificateSpec Name(string name)
        {
            Guard.NotNullOrEmpty(() => name, name);

            this.name = name;
            return this;
        }

        public ServiceIdentityWithX509CertificateSpec EncryptionCertificate(byte[] encryptionCert)
        {
            Guard.NotNull(() => encryptionCert, encryptionCert);

            this.encryptionCert = encryptionCert;
            return this;
        }

        public ServiceIdentityWithX509CertificateSpec EncryptionCertificate(string path)
        {
            Guard.NotNullOrEmpty(() => path, path);
            Guard.FileExists(() => path, path);

            var cert = new X509Certificate(path);
            this.StartDate(DateTime.Parse(cert.GetEffectiveDateString()));
            this.EndDate(DateTime.Parse(cert.GetExpirationDateString()));
            return this.EncryptionCertificate(cert.GetRawCertData());
        }

        public ServiceIdentityWithX509CertificateSpec EncryptionCertificateIdentifiedBy(string thumbprint, StoreName storeName, StoreLocation storeLocation)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);

            if (certificates.Count == 0)
                throw new ArgumentException(string.Format("Thumbprint {0} was not found in {1}//{2}", thumbprint, storeLocation, storeName), "thumbprint");

            this.StartDate(DateTime.Parse(certificates[0].GetEffectiveDateString()));
            this.EndDate(DateTime.Parse(certificates[0].GetExpirationDateString()));
            return this.EncryptionCertificate(certificates[0].GetRawCertData());
        }

        public ServiceIdentityWithX509CertificateSpec EncryptionCertificateIdentifiedBy(string thumbprint)
        {
            return this.EncryptionCertificateIdentifiedBy(thumbprint, StoreName.My, StoreLocation.CurrentUser);
        }

        public ServiceIdentityWithX509CertificateSpec StartDate(DateTime startDate)
        {
            this.startDate = startDate;
            return this;
        }

        public ServiceIdentityWithX509CertificateSpec EndDate(DateTime endDate)
        {
            this.endDate = endDate;
            return this;
        }

        internal string Name()
        {
            return this.name;
        }

        internal byte[] EncryptionCertificate()
        {
            return this.encryptionCert;
        }

        internal DateTime StartDate()
        {
            return this.startDate;
        }

        internal DateTime EndDate()
        {
            return this.endDate;
        }
    }
}