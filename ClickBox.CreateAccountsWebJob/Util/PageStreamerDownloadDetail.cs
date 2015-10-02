namespace ClickBox.CreateAccounts.Util
{
    public class PageStreamerDownloadDetail : IDownloadDetail
    {
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
    }
}