namespace ClickBox.Messages
{
    public class AccountRenewalMessage : IAzureQueueMessage
    {
        public string QueueName => "account-renewal";
    }
}
