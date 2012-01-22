using System;
using System.Globalization;

namespace FluentACS.ManagementService
{
    public class ServiceManagementException : Exception
    {
        public ServiceManagementException(Exception exception, string swtToken)
            : base(
                string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} (SWT_Token: {1})",
                        "ServiceManagementException",
                        Uri.UnescapeDataString(swtToken)), 
                exception)
        { 
        }
    }
}