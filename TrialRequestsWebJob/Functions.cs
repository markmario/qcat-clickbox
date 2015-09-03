namespace TrialRequestsWebJob
{
    using ClickBox.Web.Models;
    using Microsoft.WindowsAzure.Storage.Table;
    using Microsoft.Azure.WebJobs;
    using System.IO;
    using System;

    public class Functions
    {
        public static void ProcessQueueMessage([QueueTrigger("create-pagemaker-account")] PageMergerTrialMessage msg,
            [Table("UserAccounts")] CloudTable tableBinding, TextWriter log)        {
            log.WriteLine(msg);
            var account = new PersistedUserAccount()
            {
                ContactName = msg.AccountName,
                UserName = msg.AccountEmail,
                AccountType = "Trial",
                Active = true,
                AllocatedSeats = 1,
                CompanyName = msg.AccountOrganisation,
                IsEnterprise = false,
                IsolationEnabled = false,
                IssuedLicenses = 0,
                KoderEnabled = false,
                MaxVersionNumber = null,
                PageMakerEnabled = false,
                Password = msg.AccountPassword,
                SupportEndDate = DateTime.Now.AddDays(1)
            };
            TableOperation insertOperation = TableOperation.Insert(account);
            tableBinding.Execute(insertOperation);
        }
    }
}