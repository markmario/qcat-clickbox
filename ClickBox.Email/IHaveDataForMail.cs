namespace ClickBox.Mail
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
        string LicenseName { get; set; }
        string Password { get; set; }
        string ProductName { get; set; }
        bool PaymentReceived { get; set; }
    }
}
