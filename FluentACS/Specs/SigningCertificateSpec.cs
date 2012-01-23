namespace FluentACS.Specs
{
    using System;

    public class SigningCertificateSpec
    {
        private byte[] bytes;

        private string password;

        private DateTime startDate;

        private DateTime endDate;

        public SigningCertificateSpec Bytes(byte[] bytes)
        {
            Guard.NotNull(() => bytes, bytes);

            this.bytes = bytes;
            return this;
        }

        public SigningCertificateSpec Password(string password)
        {
            Guard.NotNullOrEmpty(() => password, password);

            this.password = password;
            return this;
        }

        public SigningCertificateSpec StartDate(DateTime startDate)
        {
            this.startDate = startDate;
            return this;
        }

        public SigningCertificateSpec EndDate(DateTime endDate)
        {
            this.endDate = endDate;
            return this;
        }

        internal byte[] Bytes()
        {
            return this.bytes;
        }

        internal string Password()
        {
            return this.password;
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