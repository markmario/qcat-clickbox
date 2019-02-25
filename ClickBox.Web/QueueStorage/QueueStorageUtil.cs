namespace ClickBox.Web.QueueStorage
{
    using System.Threading.Tasks;

    using Messages;

    using Microsoft.WindowsAzure.Storage.Queue;

    using Newtonsoft.Json;

    public static class QueueStorageUtil
    {
        public static void SendMessage<T>(this CloudQueueClient client, T msg) 
            where T : IAzureQueueMessage, new()
        {
            var queue = client.GetQueueReference(msg.QueueName);

            queue.CreateIfNotExists();

            var message = new CloudQueueMessage(JsonConvert.SerializeObject(msg));
            queue.AddMessage(message);
        }

        public static async Task SendMessageAync<T>(this CloudQueueClient client, T msg)
            where T : IAzureQueueMessage, new()
        {
            var queue = client.GetQueueReference(msg.QueueName);

            queue.CreateIfNotExists();

            var message = new CloudQueueMessage(JsonConvert.SerializeObject(msg));
            await queue.AddMessageAsync(message);
        }
    }
}