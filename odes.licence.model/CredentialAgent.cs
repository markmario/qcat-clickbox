using System;

namespace Odes.Licence.Model
{
    public class CredentialAgent
    {
        public static string GetCredentials()
        {
            const string part2 = @"\";

            var part1 = string.IsNullOrEmpty(Environment.UserDomainName) ? "Unknown domain" : Environment.UserDomainName;
            var part3 = string.IsNullOrEmpty(Environment.UserName) ? "Unknown user" : Environment.UserName;

            return part1 + part2 + part3;
        }
    }
}