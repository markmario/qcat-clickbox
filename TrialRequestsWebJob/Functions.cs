namespace TrialRequestsWebJob
{
    using System.IO;

    using Microsoft.Azure.WebJobs;

    public class Functions
    {
        #region Public Methods and Operators

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage(
            [QueueTrigger("create-pagemaker-account")] PageMergerTrialMessage blobInfo, 
            TextWriter log)
        {
            log.WriteLine(blobInfo.AccountEmail);
        }

        #endregion
    }
}