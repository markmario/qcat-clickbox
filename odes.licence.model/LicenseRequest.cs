namespace Odes.Licence.Model
{
    using System;

    public class LicenseRequest
    {
        public LicenseRequest()
        {
            this.RequestId = Guid.NewGuid();
            this.SystemDateTimeStamp = new DateTimeOffset(DateTime.UtcNow);
            this.SystemNetworkCredential = CredentialAgent.GetCredentials();
            this.SystemMachineName = Environment.MachineName;
        }

        public LicenceTypes LicenceType { get; set; }

        public int ClicksReqeusted { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ServiceQueue { get; set; }

        public string SystemId { get; set; }

        public DateTimeOffset SystemDateTimeStamp { get; set; }

        public string SystemNetworkCredential { get; set; }

        public string SystemMachineName { get; set; }

        public Guid RequestId { get; set; }

        public string PublicIp { get; set; }
    }
}