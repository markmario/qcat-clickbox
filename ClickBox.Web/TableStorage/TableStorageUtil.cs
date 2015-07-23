namespace ClickBox.Web.TableStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ClickBox.Web.Models;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

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

        private static CloudTable GetTableReference<T>(T entity) where T : TableEntity, IContainTableReference
        {
            var account = MvcApplication.TableStore;
            var tableClient = account.CreateCloudTableClient();
            var tableClientRef = tableClient.GetTableReference(entity.TableName);
            return tableClientRef;
        }

        private static CloudTable GetTableReferene<T>(T tableOfT) where T : TableEntity, IContainTableReference, new()
        {
            var account = MvcApplication.TableStore;
            var tableClient = account.CreateCloudTableClient();
            var tableClientRef = tableClient.GetTableReference(tableOfT.TableName);
            return tableClientRef;
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

        public static async Task PrimeTable<T>() where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();
            var tableClientRef = GetTableReference(tableOfT);
            await tableClientRef.CreateIfNotExistsAsync();
        }

        public static async Task InsertStorageEntityAsync<T>(T entity) where T : TableEntity, IContainTableReference
        {
            var tableClientRef = GetTableReference(entity);
            tableClientRef.CreateIfNotExists();
            var insertOperation = TableOperation.InsertOrReplace(entity);
            await tableClientRef.ExecuteAsync(insertOperation);
        }

        public static async Task<IEnumerable<T>> GetEntitiesAsync<T>(string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            string partition = partitionKey;

            if (partitionKey == null)
            {
                partition = tableOfT.PartitionKey;
            }

            var partitionScanQuery = new TableQuery<T>().Where
                    (TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition));
            var tableClientRef = GetTableReferene(tableOfT);
            var toRet = await tableClientRef.ExecuteQuerySegmentedAsync(partitionScanQuery, null);
            return toRet.Results;
        }

        public static T GetEntityByPartitionAndRowKey<T>(string rowKey, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            string partition = partitionKey;

            if (partitionKey == null)
            {
                partition = tableOfT.PartitionKey;
            }

            var retrieveOperation = TableOperation.Retrieve<T>(partition, rowKey);
            var tableClientRef = GetTableReferene(tableOfT);
            var result =  tableClientRef.Execute(retrieveOperation);
            var entity = result.Result as T;
            return entity;
        }

        public static async Task<T> GetEntityByPartitionAndRowKeyAsync<T>(string rowKey, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            string partition = partitionKey;

            if (partitionKey == null)
            {
                partition = tableOfT.PartitionKey;
            }

            var retrieveOperation = TableOperation.Retrieve<T>(partition, rowKey);
            var tableClientRef = GetTableReferene(tableOfT); 
            var result = await tableClientRef.ExecuteAsync(retrieveOperation);
            var entity = result.Result as T;
            return entity;
        }

        public static T GetEntityByPropertyFilter<T>(string propertyName, string filterValue, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            string partition = partitionKey;

            if (partitionKey == null)
            {
                partition = tableOfT.PartitionKey;
            }

            var partitionScanQuery = new TableQuery<T>().Where(
                    (TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition(propertyName, QueryComparisons.Equal, filterValue),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition))));

            var tableClientRef = GetTableReferene(tableOfT);
            var toRet = tableClientRef.ExecuteQuery(partitionScanQuery, null).FirstOrDefault();
            return toRet;
        }

        public static async Task<T> GetEntityByPropertyFilterAsync<T>(string propertyName, string filterValue, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            string partition = partitionKey;

            if (partitionKey == null)
            {
                partition = tableOfT.PartitionKey;
            }

            var partitionScanQuery = new TableQuery<T>().Where(
                    (TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition(propertyName, QueryComparisons.Equal, filterValue),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition))));

            var tableClientRef = GetTableReferene(tableOfT);
            var toRet = await tableClientRef.ExecuteQuerySegmentedAsync(partitionScanQuery, null).ConfigureAwait(false);
            return toRet.Results.FirstOrDefault();
        }

        public static T GetEntityByPropertyFilterList<T>(string filters, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();
                
            var partitionScanQuery = new TableQuery<T>().Where(filters);

            var tableClientRef = GetTableReferene(tableOfT);
            var toRet = tableClientRef.ExecuteQuery(partitionScanQuery, null).FirstOrDefault();
            return toRet;
        }

        public static async Task<T> GetEntityByPropertyFilterListAsync<T>(string filters, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            var partitionScanQuery = new TableQuery<T>().Where(filters);

            var tableClientRef = GetTableReferene(tableOfT);
            var toRet = await tableClientRef.ExecuteQuerySegmentedAsync(partitionScanQuery, null);
            return toRet.Results.FirstOrDefault() as T;
        }

        public static IEnumerable<T> GetEntityListByPropertyFilterList<T>(string filters, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            var partitionScanQuery = new TableQuery<T>().Where(filters);

            var tableClientRef = GetTableReferene(tableOfT);
            var toRet = tableClientRef.ExecuteQuery(partitionScanQuery, null);
            return toRet;
        }

        public static async Task<IEnumerable<T>> GetEntityListByPropertyFilterListAsync<T>(string filters, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            var partitionScanQuery = new TableQuery<T>().Where(filters);

            var tableClientRef = GetTableReferene(tableOfT);
            var toRet = await tableClientRef.ExecuteQuerySegmentedAsync(partitionScanQuery, null);
            return toRet.Results;
        }

        public static void DeleteEntity<T>(T deleteEntity) where T : TableEntity, IContainTableReference, new()
        {
            if (deleteEntity == null)
            {
                throw new ArgumentNullException(@"Cannot delete null entity");
            }

            var tableClientRef = GetTableReferene(deleteEntity);
            var deleteOperation = TableOperation.Delete(deleteEntity);
            tableClientRef.ExecuteAsync(deleteOperation);
        }

        public static async Task DeleteEntityAsync<T>(T deleteEntity) where T : TableEntity, IContainTableReference, new()
        {
            if (deleteEntity == null)
            {
                throw new ArgumentNullException(@"Cannot delete null entity");
            }

            var tableClientRef = GetTableReferene(deleteEntity);
            var deleteOperation = TableOperation.Delete(deleteEntity);
            await tableClientRef.ExecuteAsync(deleteOperation);
        }

        public static void UpdateEntity<T>(T updateEntity) where T : TableEntity, IContainTableReference, new()
        {
            if (updateEntity == null)
            {
                throw new ArgumentNullException(@"Cannot update null entity");
            }

            var mergeOperation = TableOperation.InsertOrMerge(updateEntity);
            var tableClientRef = GetTableReferene(updateEntity);
            tableClientRef.Execute(mergeOperation);
        }

        public static async Task UpdateEntityAsync<T>(T updateEntity) where T : TableEntity, IContainTableReference, new()
        {
            if (updateEntity == null)
            {
                throw new ArgumentNullException(@"Cannot update null entity");
            }

            var mergeOperation = TableOperation.InsertOrMerge(updateEntity);
            var tableClientRef = GetTableReferene(updateEntity);
            await tableClientRef.ExecuteAsync(mergeOperation);
        }
    }
}