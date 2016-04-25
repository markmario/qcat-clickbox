namespace ClickBox.CreateAccounts.Util
{
    public interface IDownloadDetail
    {
        string DownloadLink { get; }
        string ExtraInstructions { get; }
        int DaysLicensed { get; }
        bool UsesFreeLicenseFileImport { get; }
    }
}