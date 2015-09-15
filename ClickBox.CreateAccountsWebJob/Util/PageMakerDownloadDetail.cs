namespace ClickBox.CreateAccounts.Util
{
    public class PageMakerDownloadDetail : IDownloadDetail
    {
        public string DownloadLink
        {
            get { return "https://qcatinstalls.blob.core.windows.net/pagemaker/QcatPagemaker.application"; }
        }

        public string ExtraInstructions
        {
            get { return ". Pagemaker requires Microsoft Access Drivers. " +
                    " If you do not have Access installed, download the OLEDB Drivers here https://www.microsoft.com/en-us/download/details.aspx?id=13255";
            }
        }
    }
}
