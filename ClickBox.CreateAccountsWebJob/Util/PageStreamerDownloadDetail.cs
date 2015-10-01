namespace ClickBox.CreateAccounts.Util
{
    public class PageStreamerDownloadDetail : IDownloadDetail
    {
        public string DownloadLink
        {
            get { return "http://qcatinstalls.blob.core.windows.net/pagestreamer/PageStreamer.application"; }
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