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
    using Util;

    public class Functions
    {
        public static async void ProcessAccountCreationMessage([QueueTrigger("create-account")] AccountCreationMessage msg,
            [Table("UserAccounts")] CloudTable tableBinding, IBinder binder, TextWriter log)
        {
            var existingAccount = new ExistingAccount();
            var exists = existingAccount.DoesAccountForThisGivenProductExist(msg, tableBinding, binder);
            //if it exists and the payment was received and charged by the controller then
            //we need update the account with a new expiration date and send a different
            //email - perhaps a different message
            if (exists)
            {
                log.WriteLine($"Account {msg} exists and will now resend email with existing License details");
                return;
            }

            log.WriteLine(msg);
            var downloadDetail = ProductDownloadLinkResolver
                                .ResolveDownloadLinkFromProductName(msg.AccountProductName);

            //TODO: will have to sort out payment here if its not a Trial
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
                SupportEndDate = DateTime.Now.AddDays(downloadDetail.DaysLicensed),
                Product = msg.AccountProductName,
                PaymentReceived = msg.PaymentReceived
            };

            //check to see if the account exists?
            var insertOperation = TableOperation.Insert(account);
            //put the message on the queue only if table insert succeeds
            var result = tableBinding.Execute(insertOperation);

            //what to do if this has a different code?
            if (result.HttpStatusCode == 204)
            {
                //place send email message on the queue
                var outputQueueName = "account-created";
                var queueAttribute = new QueueAttribute(outputQueueName);
                var outputQueue = binder.Bind<CloudQueue>(queueAttribute);
                
                var accountVerify = new SendAccountAndDownloadInstructionsMessage()
                {
                    From = "licensing@qcat.com.au",
                    To = msg.AccountEmail,
                    DowloadLink = downloadDetail.DownloadLink, 
                    Instructions = "Click the link to download your copy of " + msg.AccountProductName + 
                                    " and use your License Name and Password to obtain your "  + msg.AccountLicenseType +
                                    downloadDetail.ExtraInstructions,
                    MessageBody = "Welcome to your " + msg.AccountProductName + " " + msg.AccountLicenseType,
                    ContactName = msg.AccountName,
                    FromName = "QCAT Licensing Team",
                    Password = msg.Password,
                    LicenseName = msg.AccountName,
                    ProductName = msg.AccountProductName,
                    PaymentReceived = msg.PaymentReceived
                };
                await outputQueue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(accountVerify)));
            }
        }

        public static async void ProcessSendAccountAndDownloadInstructionsMessage([QueueTrigger("account-created")]
                                                                            SendAccountAndDownloadInstructionsMessage msg,
                                                                            IBinder binder, TextWriter log)
        {
            //need to store record of failed mails
            //TODO: create a poision message handler to store in table
            var sent = await Mailer.SendMail(msg);
            if(!sent)
            {
                //five fails and into poison queue
                throw new Exception("Failed to send registration email to customer:" + msg);
            }
        }
    }
}
