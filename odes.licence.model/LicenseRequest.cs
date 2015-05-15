using System;

namespace Odes.Licence.Model
{
    public class LicenseRequest
    {
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
        public LicenseRequest()
        {
            RequestId = Guid.NewGuid();
            SystemDateTimeStamp = new DateTimeOffset(DateTime.UtcNow);
            SystemNetworkCredential = CredentialAgent.GetCredentials();
            SystemMachineName = Environment.MachineName;
        }




    }
}