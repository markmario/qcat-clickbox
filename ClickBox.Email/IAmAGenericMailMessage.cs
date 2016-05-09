namespace ClickBox.Mail
{
    public interface IAmAGenericMailMessage : IHaveDataForMail
    {
        string Header { get; set; }
        string SubHeader { get; set; }
        string BodyText { get; set; }
        string Subject { get; set; }
    }
}