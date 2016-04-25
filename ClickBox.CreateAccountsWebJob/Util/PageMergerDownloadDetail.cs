namespace ClickBox.CreateAccounts.Util
{
    public class PageMergerDownloadDetail : IDownloadDetail
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
            get { return "http://install.qcat.com.au/pagemerger/PageMerger.application"; }
        }

        public string ExtraInstructions
        {
            get
            {
                return string.Empty;
            }
        }

        public bool UsesFreeLicenseFileImport => false;
    }
}