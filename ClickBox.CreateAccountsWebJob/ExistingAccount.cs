// --------------------------------------------------------------------------------------------------
//  <copyright file="ExistingAccount.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.CreateAccounts
{
    using System.Linq;

    using ClickBox.CreateAccounts.Util;
    using ClickBox.Web.Models;

    using Messages;

    using Microsoft.Azure.WebJobs;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;

    using Newtonsoft.Json;

    public class ExistingAccount
    {
        public bool DoesAccountForThisGivenProductExist(AccountCreationMessage accountTryingToCreate, CloudTable table, IBinder binder)
        {
            var tableQuery = new TableQuery<PersistedUserAccount>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("UserName", QueryComparisons.Equal, accountTryingToCreate.AccountEmail),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("Product", QueryComparisons.Equal, accountTryingToCreate.AccountProductName)),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, new PersistedUserAccount().PartitionKey)));

            var existingAccount = table.ExecuteQuery(tableQuery).FirstOrDefault();
            if (existingAccount != null)
            {
                SendQueueMessageToCreateMailForExistingAccountForTheGivenProduct(accountTryingToCreate, binder, existingAccount);
            }
            return existingAccount != null;
        }

        private void SendQueueMessageToCreateMailForExistingAccountForTheGivenProduct(AccountCreationMessage msg, IBinder binder, PersistedUserAccount existingAccount)
        {
            var outputQueueName = "account-created";
            var queueAttribute = new QueueAttribute(outputQueueName);
            var outputQueue = binder.Bind<CloudQueue>(queueAttribute);
            var downloadDetail = ProductDownloadLinkResolver
                                .ResolveDownloadLinkFromProductName(msg.AccountProductName);
            var accountVerify = new SendAccountAndDownloadInstructionsMessage()
            {
                From = "licensing@qcat.com.au",
                To = msg.AccountEmail,
                DowloadLink = downloadDetail.DownloadLink,
                Instructions = "Click the link to download your copy of " + msg.AccountProductName +
                                    " and use your License Name and Password to obtain your " + msg.AccountLicenseType +
                                    downloadDetail.ExtraInstructions,
                MessageBody = "You already have an account for a " + msg.AccountProductName + " " + msg.AccountLicenseType + " with the following details: ",
                ContactName = msg.AccountName,
                FromName = "QCAT Licensing Team",
                Password = existingAccount.Password,
                LicenseName = msg.AccountName,
                ProductName = msg.AccountProductName
            };
            outputQueue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(accountVerify)));
        }
    }
}