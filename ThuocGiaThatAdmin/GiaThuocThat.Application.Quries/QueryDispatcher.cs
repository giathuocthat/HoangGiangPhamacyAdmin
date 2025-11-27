using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Application.Common.Interfaces;
using ThuocGiaThatAdmin.Common.Interfaces;

namespace ThuocGiaThatAdmin.Queries
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query)
        {
            var queryType = query.GetType();
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));

            var handler = _serviceProvider.GetService(handlerType)
                ?? throw new InvalidOperationException($"No handler registered for query {queryType.Name}");

            var method = handlerType.GetMethod("HandleAsync", BindingFlags.Public | BindingFlags.Instance);
            var result = await (Task<TResponse>)method!.Invoke(handler, new object[] { query })!;

            return result;
        }
    }
}
