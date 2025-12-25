using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Common.Interfaces;

namespace ThuocGiaThatAdmin.Common
{
    public class RedisCache : ICacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _db;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };

        public RedisCache(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _db = _connectionMultiplexer.GetDatabase();
        }

        public bool Exists(string key)
        {
            return _db.KeyExists(key);
        }

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            return _db.KeyExistsAsync(key);
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItemCallback, TimeSpan? expire = null, CancellationToken cancellationToken = default)
        {
            var cacheValue = await GetValueAsync<T>(key);

            // 2. Nếu có, trả về ngay (Hit)
            if (cacheValue != null && !EqualityComparer<T>.Default.Equals(cacheValue, default(T)))
            {
                return cacheValue;
            }

            var freshValue = await getItemCallback();

            if (freshValue != null)
            {
                await _db.StringSetAsync(key, JsonSerializer.Serialize(freshValue, _jsonOptions), expire, when: When.Always);
            }

            return freshValue;
        }

        public T GetValue<T>(string key)
        {
            string value = _db.StringGet(key);

            if (string.IsNullOrEmpty(value))
            {
                return default(T)!;
            }

            T parsedValue = JsonSerializer.Deserialize<T>(value, _jsonOptions)!;

            return parsedValue;
        }

        public async Task<T?> GetValueAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            string? value = await _db.StringGetAsync(key);

            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            T parsedValue = JsonSerializer.Deserialize<T>(value, _jsonOptions);

            return parsedValue;
        }

        public Task<long> IncreamentAsync(string key, long value = 1, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            return _db.KeyDelete(key);
        }

        public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return _db.KeyDeleteAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
        {
            var endpoints = _connectionMultiplexer.GetEndPoints();
            var server = _connectionMultiplexer.GetServer(endpoints.First());
            var keys = server.Keys(database: _db.Database, pattern: prefixKey + "*").ToArray();

            if (keys.Any())
            {
                await _db.KeyDeleteAsync(keys);
            }
        }

        public bool SetValue<T>(string key, T value, TimeSpan? expire = null)
        {
            // Serialize Object T thành JSON string
            var jsonValue = JsonSerializer.Serialize(value, _jsonOptions);

            // Lưu vào Redis với expiry parameter
            return _db.StringSet(key, jsonValue, expire, when: When.Always);
        }

        public async Task<bool> SetValueAsync<T>(string key, T value, TimeSpan? expire = null, CancellationToken cancellationToken = default)
        {
            // Serialize Object T thành JSON string
            var jsonValue = JsonSerializer.Serialize(value, _jsonOptions);

            // Lưu vào Redis với expiry parameter
            return await _db.StringSetAsync(key, jsonValue, expire, when: When.Always);
        }
    }
}
