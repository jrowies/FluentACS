using System;
using FluentACS;

namespace FluentACS.Specs
{
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