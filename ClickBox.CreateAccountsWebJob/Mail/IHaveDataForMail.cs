namespace ClickBox.CreateAccounts.Mail
{
    public interface IHaveDataForMail
    {
        string MessageBody { get; set; }
        string To { get; set; }
        string From { get; set; }
        string DowloadLink { get; set; }
        string Instructions { get; set; }
        string ContactName { get; set; }
        string FromName { get; set; }
    }
}
