namespace ClickBox.Messages
{
    public interface IAzureQueueMessage : IContainQueueReference
    {
        string QueueName { get;}
    }
}