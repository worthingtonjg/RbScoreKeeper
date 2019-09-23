using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RbScoreKeeper.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RbScoreKeeper.Services
{
    public interface IStorageHelper
    {
        Task<List<PlayerEntity>> GetPlayersAsync();
        Task<PlayerEntity> AddPlayerAsync(string name);
        Task<bool> DeletePlayerAsync(Guid playerId);
        Task<List<FlicEntity>> GetFlicsAsync();
        Task<FlicEntity> AddFlicAsync(string name);
        Task<bool> DeleteFlicAsync(Guid flicId);
    }

    public class StorageHelper : IStorageHelper
    {
        private string partitionKey = "worthingtonjg";

        CloudStorageAccount storageAccount;
        CloudTableClient tableClient;
        CloudTable playerTable;
        CloudTable flicTable;

        public StorageHelper(string accountName, string accountKey)
        {
            storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    accountName, accountKey), true);

            tableClient = storageAccount.CreateCloudTableClient();

            playerTable = tableClient.GetTableReference("Player");
            flicTable = tableClient.GetTableReference("Flic");
        }

        public async Task<List<PlayerEntity>> GetPlayersAsync()
        {
            var result = new List<PlayerEntity>();

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<PlayerEntity> query = new TableQuery<PlayerEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "worthingtonjg"));

            // Print the fields for each customer.
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<PlayerEntity> resultSegment = await playerTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                result = resultSegment.Results;

            } while (token != null);

            return result;
        }

        public async Task<PlayerEntity> AddPlayerAsync(string name)
        {
            PlayerEntity player = new PlayerEntity();
            player.PartitionKey = partitionKey;
            player.RowKey = Guid.NewGuid().ToString();
            player.Name = name;

            TableOperation insert = TableOperation.Insert(player);
            await playerTable.ExecuteAsync(insert);

            return player;
        }

        public async Task<bool> DeletePlayerAsync(Guid playerId)
        {
            TableOperation retrieve = TableOperation.Retrieve<PlayerEntity>(partitionKey, playerId.ToString());
            TableResult retrieved = await playerTable.ExecuteAsync(retrieve);
            PlayerEntity deleteEntity = (PlayerEntity)retrieved.Result;

            if (deleteEntity == null)
            {
                return false;
            }

            TableOperation delete = TableOperation.Delete(deleteEntity);
            await playerTable.ExecuteAsync(delete);

            return true;
        }

        public async Task<List<FlicEntity>> GetFlicsAsync()
        {
            var result = new List<FlicEntity>();

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<FlicEntity> query = new TableQuery<FlicEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "worthingtonjg"));

            // Print the fields for each customer.
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<FlicEntity> resultSegment = await flicTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                result = resultSegment.Results;

            } while (token != null);

            return result.OrderBy(f => f.Name).ToList();
        }

        public async Task<FlicEntity> AddFlicAsync(string name)
        {
            FlicEntity flic = new FlicEntity();
            flic.PartitionKey = partitionKey;
            flic.RowKey = Guid.NewGuid().ToString();
            flic.Name = name;

            TableOperation insert = TableOperation.Insert(flic);
            await flicTable.ExecuteAsync(insert);

            return flic;
        }

        public async Task<bool> DeleteFlicAsync(Guid flicId)
        {
            TableOperation retrieve = TableOperation.Retrieve<FlicEntity>(partitionKey, flicId.ToString());
            TableResult retrieved = await flicTable.ExecuteAsync(retrieve);
            FlicEntity deleteEntity = (FlicEntity)retrieved.Result;

            if(deleteEntity == null)
            {
                return false;
            }

            TableOperation delete = TableOperation.Delete(deleteEntity);
            await flicTable.ExecuteAsync(delete);

            return true;
        }
    }
}
