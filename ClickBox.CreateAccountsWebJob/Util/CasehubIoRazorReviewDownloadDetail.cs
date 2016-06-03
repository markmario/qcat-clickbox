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
            get { return "http://qcatinstalls.blob.core.windows.net/razor/Setup.exe"; }
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