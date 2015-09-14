namespace ClickBox.CreateAccounts
{
    using Web.Models;
    using Microsoft.WindowsAzure.Storage.Table;
    using Microsoft.Azure.WebJobs;
    using System.IO;
    using System;
    using Messages;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using Mail;

    public class Functions
    {
        public static void ProcessAccountCreationMessage([QueueTrigger("create-account")] AccountCreationMessage msg,
            [Table("UserAccounts")] CloudTable tableBinding, IBinder binder, TextWriter log)
        {

            log.WriteLine(msg);

            var account = new PersistedUserAccount()
            {
                ContactName = msg.AccountName,
                UserName = msg.AccountEmail,
                AccountType = msg.AccountLicenseType,
                Active = true,
                AllocatedSeats = 1,
                CompanyName = msg.AccountOrganisation,
                IsEnterprise = false,
                IsolationEnabled = false,
                IssuedLicenses = 0,
                KoderEnabled = false,
                MaxVersionNumber = null,
                PageMakerEnabled = false,
                Password = msg.Password,
                SupportEndDate = DateTime.Now.AddDays(35),
                Product = msg.AccountProductName
            };
            //check to see if the account exists?
            TableOperation insertOperation = TableOperation.Insert(account);
            var result = tableBinding.Execute(insertOperation);

            //what to do if this has a different code?
            if (result.HttpStatusCode == 204)
            {
                //place send email message on the queue
                string outputQueueName = "account-created";
                QueueAttribute queueAttribute = new QueueAttribute(outputQueueName);
                CloudQueue outputQueue = binder.Bind<CloudQueue>(queueAttribute);
                var accountVerify = new SendAccountAndDownloadInstructionsMessage()
                {
                    From = "licensing@qcat.com.au",
                    To = msg.AccountEmail,
                    DowloadLink = "http://qcatinstalls.blob.core.windows.net/pagemaker/QcatPagemaker.application",
                    Instructions = "Click the link to download your copy of " + msg.AccountProductName + 
                                    " and use your License Name and Password to obtain your "  + msg.AccountLicenseType,
                    MessageBody = "Welcome to your " + msg.AccountProductName + " " + msg.AccountLicenseType,
                    ContactName = msg.AccountName,
                    FromName = "QCAT Licensing Team",
                    Password = msg.Password,
                    LicenseName = msg.AccountName
                };
                outputQueue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(accountVerify)));
            }
        }

        public static async void ProcessSendAccountAndDownloadInstructionsMessage([QueueTrigger("account-created")]
                                                                            SendAccountAndDownloadInstructionsMessage msg,
                                                                            IBinder binder, TextWriter log)
        {
            var sent = await Mailer.SendMail(msg);
            //if sent ok then send message that mail was sent
        }

        
    }
}
