namespace Odes.License.Updater
{
    using System;
    using System.DirectoryServices;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Principal;
    using System.Text;
    using Odes.Licence.Model;

    public class ProductLicenser
    {
        public LicenseRequest IntialiseLicenseDetails(LicenceTypes licenceTypes)
        {
            var licence = new LicenseRequest
            {
                LicenceType = licenceTypes,
                SystemMachineName = Environment.MachineName,
                SystemId = new SecurityIdentifier((byte[]) new DirectoryEntry(string.Format("WinNT://{0},Computer",Environment.MachineName))
                            .Children
                            .Cast<DirectoryEntry>()
                            .First()
                            .InvokeGet("objectSID"),0)
                            .AccountDomainSid.ToString(),
                UserName = Environment.UserName + "@" + Environment.UserDomainName
            };

            return licence;
        }

        public ProductLicenseResponse GeneratedLisense(LicenseRequest licenseRequest, string requestingAppToken)
        {
            licenseRequest.GetPublicIp();

            try
            {
#if ! DEBUG
                var client = new HttpClient { BaseAddress = new Uri("https://clickbox.qcat.com.au/") };
#else
                var client = new HttpClient { BaseAddress = new Uri("https://localhost:44302/") };
#endif

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Add("Authorization-Token", requestingAppToken);

                var result = client.GetAsync(string.Format("api/License/GetProductDetail?productName={0}", licenseRequest.ProductName)).Result;

                if (result.IsSuccessStatusCode)
                {
                    var re = result.Content.ReadAsAsync<Product>().Result;
                    licenseRequest.ProductId = new Guid(re.Id);
                }

                var response = client.PostAsJsonAsync("api/License/", licenseRequest).Result; 

                if (response.IsSuccessStatusCode)
                {
                    var rep = response.Content.ReadAsAsync<string>().Result;

                    return new ProductLicenseResponse
                    {
                        RespondingWithSuccess = true, 
                        LicenseText = rep
                    };
                }
                return new ProductLicenseResponse
                {
                    RespondingWithSuccess = false,
                    FailureDetails = new FailedResponseDetails()
                                        {
                                            Error = response.ToString(),
                                            StatusCode = (int)response.StatusCode
                                        },
                    LicenseText = response.ToString()
                };
            }
            catch (Exception ex)
            {
                var exFile = Path.GetTempFileName() + ".qlic.elog";
                File.WriteAllText(exFile, ex.ToString(), Encoding.UTF8);

                return new ProductLicenseResponse
                {
                    RespondingWithSuccess = false,
                    ContainsException = new Tuple<bool, string>(true, exFile)
                };
            }
        }
    }

    public class ProductLicenseResponse
    {
        public string LicenseText { get; set; }

        public bool RespondingWithSuccess { get; set; }

        public Tuple<bool, string> ContainsException { get; set; }

        public FailedResponseDetails FailureDetails { get; set; }
    }

    public class FailedResponseDetails
    {
        public int StatusCode { get; set; }

        public string Reason { get; set; }

        public string Error { get; set; }
    }
}