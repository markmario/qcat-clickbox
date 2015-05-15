using System.IO;
using System.Net;

namespace Odes.Licence.Model
{
    public static class LicenseRequestExtensions
    {
        public static string LicenceTypeText(this LicenseRequest lic)
        {
            if (lic.LicenceType == LicenceTypes.Client)
                return "";
            return lic.LicenceType == LicenceTypes.Isolator ? "Isolator" : "Koder";

        }
        public static void GetPublicIp(this LicenseRequest lic)
        {
            try
            {
                string direction;
                var request = WebRequest.Create("http://checkip.dyndns.org/");
                using (var response = request.GetResponse())
                using (var stream = new StreamReader(response.GetResponseStream()))
                {
                    direction = stream.ReadToEnd();
                }

                //Search for the ip in the html
                var first = direction.IndexOf("Address: ", System.StringComparison.Ordinal) + 9;
                var last = direction.LastIndexOf("</body>", System.StringComparison.Ordinal);
                direction = direction.Substring(first, last - first);
                //return direction;
                lic.PublicIp = direction;
            }
            catch{}
            
        }
    }
}