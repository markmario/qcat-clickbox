namespace ClickBox.CreateAccounts.Util
{
    public class CasehubIoRazorReviewDownloadDetail : IDownloadDetail
    {
        public int DaysLicensed
        {
            get
            {
                return 62;
            }
        }

        public string DownloadLink
        {
            get { return "http://install.qcat.com.au/razor/0.0.1/casehub.io.0.0.1.zip"; }
        }

        public string ExtraInstructions
        {
            get
            {
                return "Once installed, import the attached license file called razor.licx.json. "
                       + "For converting load files to import into casehub.io, please read the instructions "
                       + "contained in the readme file contained in the download";
            }
        }

        public bool UsesFreeLicenseFileImport => true;
    }
}