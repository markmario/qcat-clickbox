namespace ClickBox.Web.TableStorage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using ClickBox.Web.Models;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    using Newtonsoft.Json;

    using Raven.Client;

    public static class TableStorageUtil
    {
        static TableStorageUtil()
        {
            //StreamReader re = new StreamReader("tsKey.json");
            //JsonTextReader reader = new JsonTextReader(re);
            //JsonSerializer se = new JsonSerializer();
            //object parsedData = se.Deserialize(reader);
        }

        public static CloudStorageAccount GetStorageAccount()
        {
            var debug = CloudConfigurationManager.GetSetting("Runtime");
            if (debug == "debug")
            {
                return CloudStorageAccount.DevelopmentStorageAccount;
            }
            return CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }

        public static string GetPartitionPrefix()
        {
            var debug = CloudConfigurationManager.GetSetting("Runtime");
            if (debug == "debug")
            {
                return "dev_";
            }
            return "prod_";
        }

        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }

        public static void InsertStorageEntity<T>(T entity) where T : TableEntity, IContainTableReference
        {
            var account = GetStorageAccount();
            var tableClient = account.CreateCloudTableClient();
            var tableClientRef = tableClient.GetTableReference(entity.TableName);
            tableClientRef.CreateIfNotExists();
            TableOperation insertOperation = TableOperation.InsertOrReplace(entity);
            tableClientRef.Execute(insertOperation);
        }

        public static IEnumerable<T> GetEntities<T>(string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            string partition = partitionKey;

            if (partitionKey == null)
            {
                partition = tableOfT.PartitionKey;
            }

            var partitionScanQuery = new TableQuery<T>().Where
                    (TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition));
            var account = GetStorageAccount();
            var tableClient = account.CreateCloudTableClient();
            var tableClientRef = tableClient.GetTableReference(tableOfT.TableName);
            var toRet = tableClientRef.ExecuteQuery(partitionScanQuery, null);
            return toRet;
        }
    }
}