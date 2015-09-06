namespace ClickBox.CreateAccounts
{
    using ClickBox.Web.Models;
    using Microsoft.WindowsAzure.Storage.Table;
    using Microsoft.Azure.WebJobs;
    using System.IO;
    using System;

    public class Functions
    {
        public static void ProcessQueueMessage([QueueTrigger("create-account")] AccountCreationMessage msg,
            [Table("UserAccounts")] CloudTable tableBinding, TextWriter log){
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
            TableOperation insertOperation = TableOperation.Insert(account);
            tableBinding.Execute(insertOperation);
        }
    }
}