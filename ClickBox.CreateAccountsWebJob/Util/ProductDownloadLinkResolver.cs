namespace ClickBox.CreateAccounts.Util
{
    public static class ProductDownloadLinkResolver
    {
        public static IDownloadDetail ResolveDownloadLinkFromProductName(string productName)
        {
            switch (productName)
            {
                case "Pagemaker":
                    return new PageMakerDownloadDetail();
                case "Pagestreamer":
                    return new PageStreamerDownloadDetail();
                default:
                    return null;
            }
        }
    }
}