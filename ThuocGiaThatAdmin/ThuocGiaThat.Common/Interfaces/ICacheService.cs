using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Common.Interfaces
{
    public interface ICacheService
    {
        T GetValue<T>(string key);
        bool SetValue<T>(string key, T value, TimeSpan? expire = null);
        bool Remove(string key);
        bool Exists(string key);
        Task<T> GetValueAsync<T>(string key, CancellationToken cancellationToken = default);
        Task<bool> SetValueAsync<T>(string key, T value, TimeSpan? expire = null, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expire = null, CancellationToken cancellationToken = default);
        Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
        Task<long> IncreamentAsync(string key, long value = 1, CancellationToken cancellationToken = default);
    }
}
