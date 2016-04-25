namespace ClickBox.CreateAccounts.Util
{
    public class CasehubIoRazorReviewDownloadDetail : IDownloadDetail
    {
        public int DaysLicensed
        {
            get
            {
                return 370;
            }
        }

        public string DownloadLink
        {
            get { return "http://www.qcat.com.au/casehub.io/downloads"; }
        }

        public string ExtraInstructions
        {
            get
            {
                return "Once installed, import the attached license file called razor.licx.json.";
            }
        }

        public bool UsesFreeLicenseFileImport => true;
    }
}