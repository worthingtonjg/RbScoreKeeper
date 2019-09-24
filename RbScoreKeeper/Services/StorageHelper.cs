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
        Task<List<StatsEntity>> GetStatsAsync();
        Task AddUpdateStatsAsync(List<string> playerIds, string winnerId);
    }

    public class StorageHelper : IStorageHelper
    {
        private string partitionKey = "worthingtonjg";

        CloudStorageAccount storageAccount;
        CloudTableClient tableClient;
        CloudTable playerTable;
        CloudTable flicTable;
        CloudTable statsTable;

        public StorageHelper(string accountName, string accountKey)
        {
            storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    accountName, accountKey), true);

            tableClient = storageAccount.CreateCloudTableClient();

            playerTable = tableClient.GetTableReference("Player");
            flicTable = tableClient.GetTableReference("Flic");
            statsTable = tableClient.GetTableReference("Stats");
        }

        public async Task<List<PlayerEntity>> GetPlayersAsync()
        {
            var result = new List<PlayerEntity>();

            TableQuery<PlayerEntity> query = new TableQuery<PlayerEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "worthingtonjg"));

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

            TableQuery<FlicEntity> query = new TableQuery<FlicEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "worthingtonjg"));

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

        public async Task<List<StatsEntity>> GetStatsAsync()
        {
            var result = new List<StatsEntity>();

            TableQuery<StatsEntity> query = new TableQuery<StatsEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "worthingtonjg"));

            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<StatsEntity> resultSegment = await statsTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                result = resultSegment.Results;

            } while (token != null);

            return result.OrderBy(f => f.Name).ToList();
        }

        public async Task AddUpdateStatsAsync(List<string> playerIds, string winnerId)
        {
            if (playerIds.Count < 1)
            {
                throw new Exception("Must have at least 1 player");
            }

            if (playerIds.Count > 3)
            {
                throw new Exception("Cannot have more than 3 players");
            }

            if (!playerIds.Contains(winnerId))
            {
                throw new Exception("One of the players must be the winner");
            }

            // Get all the stats
            var stats = await GetStatsAsync();

            // Find an stat that matches the list of players
            StatsEntity match = null;
            foreach(var stat in stats)
            {
                if(stat.PlayerId1 != null && !playerIds.Contains(stat.PlayerId1))
                {
                    continue;
                }

                if (stat.PlayerId2 != null && !playerIds.Contains(stat.PlayerId2))
                {
                    continue;
                }

                if (stat.PlayerId3 != null && !playerIds.Contains(stat.PlayerId3))
                {
                    continue;
                }

                match = stat;
                break;
            }

            if (match == null)
            {
                // No match was found
                var players = await GetPlayersAsync();

                var playerGroup = players.Where(p => playerIds.Contains(p.RowKey.ToString())).OrderBy(p => p.Name).ToList();

                // Create a new stat
                match = new StatsEntity
                {
                    PartitionKey = partitionKey,
                    RowKey = Guid.NewGuid().ToString(),
                    Name = string.Join(" vs ", playerGroup.Select(p => p.Name).ToList()),
                    Wins1 = 0,
                    Wins2 = 0,
                    Wins3 = 0,
                };

                // Update the player ids and the winner
                if (playerIds.Count >= 1)
                {
                    match.PlayerId1 = playerIds[0];
                    if(winnerId == match.PlayerId1)
                    {
                        ++match.Wins1;
                    }
                }

                if (playerIds.Count >= 2)
                {
                    match.PlayerId2 = playerIds[1];
                    if (winnerId == match.PlayerId2)
                    {
                        ++match.Wins2;
                    }
                }

                if (playerIds.Count >= 3)
                {
                    match.PlayerId3 = playerIds[2];
                    if (winnerId == match.PlayerId3)
                    {
                        ++match.Wins3;
                    }
                }

                // Insert new stat
                TableOperation insert = TableOperation.Insert(match);
                await statsTable.ExecuteAsync(insert);
            }
            else
            {
                // Increment the stat for the player that one
                if(winnerId == match.PlayerId1)
                {
                    ++match.Wins1;
                }
                if (winnerId == match.PlayerId2)
                {
                    ++match.Wins2;
                }
                if (winnerId == match.PlayerId3)
                {
                    ++match.Wins3;
                }

                // Update stat
                TableOperation update = TableOperation.Replace(match);
                await statsTable.ExecuteAsync(update);
            }
        }
    }
}
