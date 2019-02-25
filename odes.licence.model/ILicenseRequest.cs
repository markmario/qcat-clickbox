namespace Odes.Licence.Model
{
    using System;

    public interface ILicenseRequest
    {
        LicenceTypes LicenceType { get; set; }

        int ClicksReqeusted { get; set; }

        string Email { get; set; }

        string UserName { get; set; }

        string Password { get; set; }

        string ServiceQueue { get; set; }

        string SystemId { get; set; }

        Guid ProductId { get; set; }

        DateTimeOffset SystemDateTimeStamp { get; set; }

        string SystemNetworkCredential { get; set; }

        string SystemMachineName { get; set; }

        Guid RequestId { get; set; }

        string PublicIp { get; set; }

        string ProductName { get; set; }
    }
}