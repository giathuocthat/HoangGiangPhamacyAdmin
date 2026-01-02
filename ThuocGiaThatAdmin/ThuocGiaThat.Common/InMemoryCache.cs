using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ThuocGiaThatAdmin.Common.Interfaces;


namespace ThuocGiaThatAdmin.Common
{
    public class InMemoryCache : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly HashSet<string> _keys = new HashSet<string>();
        private readonly object _lock = new object();

        public InMemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Exists(key));
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expire = null, CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue<T>(key, out var cachedValue))
            {
                return cachedValue;
            }

            var freshValue = await factory();

            if (freshValue != null)
            {
                await SetValueAsync(key, freshValue, expire, cancellationToken);
            }

            return freshValue;
        }

        public T GetValue<T>(string key)
        {
            return _cache.Get<T>(key)!;
        }

        public Task<T> GetValueAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetValue<T>(key));
        }

        public Task<long> IncreamentAsync(string key, long value = 1, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                var currentValue = _cache.TryGetValue<long>(key, out var cached) ? cached : 0;
                var newValue = currentValue + value;
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                    .RegisterPostEvictionCallback((k, v, r, s) => RemoveKeyFromTracking(k?.ToString() ?? string.Empty));

                _cache.Set(key, newValue, options);
                TrackKey(key);

                return Task.FromResult(newValue);
            }
        }

        public bool Remove(string key)
        {
            _cache.Remove(key);
            RemoveKeyFromTracking(key);
            return true;
        }

        public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Remove(key));
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                var keysToRemove = _keys.Where(k => k.StartsWith(prefix)).ToList();

                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                    _keys.Remove(key);
                }
            }

            return Task.CompletedTask;
        }

        public bool SetValue<T>(string key, T value, TimeSpan? expire = null)
        {
            var expiration = expire ?? TimeSpan.FromHours(1);
            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiration)
                .RegisterPostEvictionCallback((k, v, r, s) => RemoveKeyFromTracking(k?.ToString() ?? string.Empty));

            _cache.Set(key, value, options);
            TrackKey(key);
            return true;
        }

        public Task<bool> SetValueAsync<T>(string key, T value, TimeSpan? expire = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(SetValue(key, value, expire));
        }

        private void TrackKey(string key)
        {
            lock (_lock)
            {
                _keys.Add(key);
            }
        }

        private void RemoveKeyFromTracking(string key)
        {
            lock (_lock)
            {
                _keys.Remove(key);
            }
        }
    }
}
