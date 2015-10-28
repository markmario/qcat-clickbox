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
                case "Pagemerger":
                    return new PageMergerDownloadDetail();
                default:
                    return null;
            }
        }
    }
}