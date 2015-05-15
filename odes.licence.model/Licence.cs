namespace Odes.Licence.Model
{
    public class Licence
    {
        private const string DbConnectionString = @"Data Source={0};Initial Catalog={1};user id={2}; password={3};Integrated Security=False;MultipleActiveResultSets=True";

        public string DatabaseName { get; set; }
        public string DatabaseServerName { get; set; }
        public string DatabaseServerPassword { get; set; }
        public string DatabaseServerUserId { get; set; }
        public string PublicallySignedFileLocation { get; set; }
        public string PublicKeyFragment { get; set; }
        public string ProcessingError { get; set; }
        public DatabaseLicenseFragment LicenceIssued { get; set; }
        public string DatabaseLicenceFragment { get; set; }

        public string DatabaseConnectionString
        {
            get
            {
                return
                    DbConnectionString.Replace("{0}", DatabaseServerName)
                                      .Replace("{1}", DatabaseName)
                                      .Replace("{2}", DatabaseServerUserId)
                                      .Replace("{3}", DatabaseServerPassword);
            }
        }

    }
}