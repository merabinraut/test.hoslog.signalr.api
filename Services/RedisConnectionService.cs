using System.Net;
using hoslog.signalr.api.Models.Cache;
using StackExchange.Redis;

namespace hoslog.signalr.api.Services
{
    public class RedisConnectionService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly EndPoint _endPoint;
        private readonly RedisCacheSetting _cacheSettings;

        public RedisConnectionService(IConnectionMultiplexer redis, RedisCacheSetting cacheSettings)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
            _endPoint = _redis.GetEndPoints().FirstOrDefault();
            _cacheSettings = cacheSettings;
        }

        private string GetCustomerKey(string customerId) => $"customer:{customerId}";

        public async Task AddConnectionAsync(string customerId, string connectionId, TimeSpan? expiry = null)
        {
            var key = GetCustomerKey(customerId);

            if (!expiry.HasValue)
            {
                expiry = TimeSpan.FromMinutes(30);
            }

            var transaction = _database.CreateTransaction();
            transaction.SetAddAsync(key, connectionId);
            transaction.KeyExpireAsync(key, expiry.Value);

            await transaction.ExecuteAsync();
        }

        public async Task RemoveConnectionAsync(string customerId, string connectionId)
        {
            var key = GetCustomerKey(customerId);
            await _database.SetRemoveAsync(key, connectionId);
        }

        public async Task<RedisValue[]> GetConnectionsAsync(string customerId)
        {
            var key = GetCustomerKey(customerId);
            return await _database.SetMembersAsync(key);
        }

        public async Task RemoveConnectionFromAllAsync(string connectionId)
        {
            if (_endPoint == null)
                throw new InvalidOperationException("No Redis endpoints found.");

            var server = _redis.GetServer(_endPoint);

            var keys = server.Keys(pattern: "customer:*").ToArray();
            var tasks = new List<Task>();
            var batch = _database.CreateBatch();
            foreach (var key in keys)
            {
                var task = batch.SetRemoveAsync(key, connectionId);
                tasks.Add(task);
            }
            batch.Execute();

            await Task.WhenAll(tasks);
        }

        public async Task DeleteCustomerConnectionsAsync(string customerId)
        {
            var key = GetCustomerKey(customerId);
            await _database.KeyDeleteAsync(key);
        }

        public async Task ClearAllData()
        {
            var server = _redis.GetServer(_endPoint);
            var db = _redis.GetDatabase();
            var pattern = $"{_cacheSettings.instanceName}:{_cacheSettings.channelPrefix}*";
            var cursor = 0L;
            var pageSize = 1000;

            do
            {
                var result = await db.ExecuteAsync("SCAN", cursor, "MATCH", pattern, "COUNT", pageSize);
                cursor = (long)result[0];
                var keys = (RedisValue[])result[1];

                foreach (var key in keys)
                {
                    if (!key.IsNullOrEmpty)
                        await db.KeyDeleteAsync(new RedisKey(key));
                }
            } while (cursor != 0);
        }
    }
}