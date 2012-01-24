namespace FluentACS.Logging
{
    using System;

    public class LogInfo
    {
        private string message;

        public LogInfoTypeEnum LogInfoType { get; set; }

        public Exception Exception { get; set; }

        public string Message 
        { 
            get
            {
                if (string.IsNullOrEmpty(this.message) && this.Exception != null)
                {
                    return this.Exception.ToString();
                }

                return this.message;
            }

            set
            {
                this.message = value;
            }
        }
    }
}