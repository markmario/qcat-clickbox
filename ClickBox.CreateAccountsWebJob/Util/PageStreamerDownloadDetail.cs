using System;

namespace ClickBox.CreateAccounts.Util
{
    public class PageStreamerDownloadDetail : IDownloadDetail
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
            get { return "http://install.qcat.com.au/pagestreamer/PageStreamer.application"; }
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