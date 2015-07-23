namespace ClickBox.Web.TableStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage.Table;

    public static class TableStorageUtil
    {
        private static CloudTable GetTableReference<T>(this CloudTableClient client, T entity) where T : TableEntity, IContainTableReference
        {
            var tableClientRef = client.GetTableReference(entity.TableName);
            return tableClientRef;
        }

        private static CloudTable GetTableReferene<T>(this CloudTableClient client, T tableOfT) where T : TableEntity, IContainTableReference, new()
        {
            var tableClientRef = client.GetTableReference(tableOfT.TableName);
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

        public static async Task PrimeTable<T>(this CloudTableClient client) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();
            var tableClientRef = client.GetTableReference(tableOfT);
            await tableClientRef.CreateIfNotExistsAsync();
        }

        public static async Task InsertStorageEntityAsync<T>(this CloudTableClient client, T entity) where T : TableEntity, IContainTableReference
        {
            var tableClientRef = client.GetTableReference(entity);
            tableClientRef.CreateIfNotExists();
            var insertOperation = TableOperation.InsertOrReplace(entity);
            await tableClientRef.ExecuteAsync(insertOperation);
        }

        public static async Task<IEnumerable<T>> GetEntitiesAsync<T>(this CloudTableClient client, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            string partition = partitionKey;

            if (partitionKey == null)
            {
                partition = tableOfT.PartitionKey;
            }

            var partitionScanQuery = new TableQuery<T>().Where
                    (TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition));
            var tableClientRef = client.GetTableReferene(tableOfT);
            var toRet = await tableClientRef.ExecuteQuerySegmentedAsync(partitionScanQuery, null);
            return toRet.Results;
        }

        public static T GetEntityByPartitionAndRowKey<T>(this CloudTableClient client, string rowKey, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            string partition = partitionKey;

            if (partitionKey == null)
            {
                partition = tableOfT.PartitionKey;
            }

            var retrieveOperation = TableOperation.Retrieve<T>(partition, rowKey);
            var tableClientRef = client.GetTableReferene(tableOfT);
            var result =  tableClientRef.Execute(retrieveOperation);
            var entity = result.Result as T;
            return entity;
        }

        public static async Task<T> GetEntityByPartitionAndRowKeyAsync<T>(this CloudTableClient client, string rowKey, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            string partition = partitionKey;

            if (partitionKey == null)
            {
                partition = tableOfT.PartitionKey;
            }

            var retrieveOperation = TableOperation.Retrieve<T>(partition, rowKey);
            var tableClientRef = client.GetTableReferene(tableOfT); 
            var result = await tableClientRef.ExecuteAsync(retrieveOperation);
            var entity = result.Result as T;
            return entity;
        }

        public static T GetEntityByPropertyFilter<T>(this CloudTableClient client, string propertyName, string filterValue, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
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

            var tableClientRef = client.GetTableReferene(tableOfT);
            var toRet = tableClientRef.ExecuteQuery(partitionScanQuery, null).FirstOrDefault();
            return toRet;
        }

        public static async Task<T> GetEntityByPropertyFilterAsync<T>(this CloudTableClient client, string propertyName, string filterValue, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
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

            var tableClientRef = client.GetTableReferene(tableOfT);
            var toRet = await tableClientRef.ExecuteQuerySegmentedAsync(partitionScanQuery, null).ConfigureAwait(false);
            return toRet.Results.FirstOrDefault();
        }

        public static T GetEntityByPropertyFilterList<T>(this CloudTableClient client, string filters, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();
                
            var partitionScanQuery = new TableQuery<T>().Where(filters);

            var tableClientRef = client.GetTableReferene(tableOfT);
            var toRet = tableClientRef.ExecuteQuery(partitionScanQuery, null).FirstOrDefault();
            return toRet;
        }

        public static async Task<T> GetEntityByPropertyFilterListAsync<T>(this CloudTableClient client, string filters, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            var partitionScanQuery = new TableQuery<T>().Where(filters);

            var tableClientRef = client.GetTableReferene(tableOfT);
            var toRet = await tableClientRef.ExecuteQuerySegmentedAsync(partitionScanQuery, null);
            return toRet.Results.FirstOrDefault() as T;
        }

        public static IEnumerable<T> GetEntityListByPropertyFilterList<T>(this CloudTableClient client, string filters, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            var partitionScanQuery = new TableQuery<T>().Where(filters);

            var tableClientRef = client.GetTableReferene(tableOfT);
            var toRet = tableClientRef.ExecuteQuery(partitionScanQuery, null);
            return toRet;
        }

        public static async Task<IEnumerable<T>> GetEntityListByPropertyFilterListAsync<T>(this CloudTableClient client, string filters, string partitionKey = null) where T : TableEntity, IContainTableReference, new()
        {
            var tableOfT = new T();

            var partitionScanQuery = new TableQuery<T>().Where(filters);

            var tableClientRef = client.GetTableReferene(tableOfT);
            var toRet = await tableClientRef.ExecuteQuerySegmentedAsync(partitionScanQuery, null);
            return toRet.Results;
        }

        public static void DeleteEntity<T>(this CloudTableClient client, T deleteEntity) where T : TableEntity, IContainTableReference, new()
        {
            if (deleteEntity == null)
            {
                throw new ArgumentNullException(@"Cannot delete null entity");
            }

            var tableClientRef = client.GetTableReferene(deleteEntity);
            var deleteOperation = TableOperation.Delete(deleteEntity);
            tableClientRef.ExecuteAsync(deleteOperation);
        }

        public static async Task DeleteEntityAsync<T>(this CloudTableClient client, T deleteEntity) where T : TableEntity, IContainTableReference, new()
        {
            if (deleteEntity == null)
            {
                throw new ArgumentNullException(@"Cannot delete null entity");
            }

            var tableClientRef = client.GetTableReferene(deleteEntity);
            var deleteOperation = TableOperation.Delete(deleteEntity);
            await tableClientRef.ExecuteAsync(deleteOperation);
        }

        public static void UpdateEntity<T>(this CloudTableClient client, T updateEntity) where T : TableEntity, IContainTableReference, new()
        {
            if (updateEntity == null)
            {
                throw new ArgumentNullException(@"Cannot update null entity");
            }

            var mergeOperation = TableOperation.InsertOrMerge(updateEntity);
            var tableClientRef = client.GetTableReferene(updateEntity);
            tableClientRef.Execute(mergeOperation);
        }

        public static async Task UpdateEntityAsync<T>(this CloudTableClient client, T updateEntity) where T : TableEntity, IContainTableReference, new()
        {
            if (updateEntity == null)
            {
                throw new ArgumentNullException(@"Cannot update null entity");
            }

            var mergeOperation = TableOperation.InsertOrMerge(updateEntity);
            var tableClientRef = client.GetTableReferene(updateEntity);
            await tableClientRef.ExecuteAsync(mergeOperation);
        }
    }
}