using System;

namespace ClickBox.CreateAccounts.Util
{
    public class PageMakerDownloadDetail : IDownloadDetail
    {
        public int DaysLicensed
        {
            get
            {
                return 50;
            }
        }

        public string DownloadLink
        {
            get { return "http://install.qcat.com.au/pagemaker/QcatPagemaker.application"; }
        }

        public string ExtraInstructions
        {
            get { return ". Pagemaker requires Microsoft Access Drivers. " +
                    " If you do not have Access installed, download the OLEDB Drivers here http://www.microsoft.com/en-us/download/confirmation.aspx?id=23734";
            }
        }
    }
}
