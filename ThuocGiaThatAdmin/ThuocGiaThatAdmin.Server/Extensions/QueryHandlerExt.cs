using System.Reflection;
using ThuocGiaThatAdmin.Common.Interfaces;

namespace ThuocGiaThatAdmin.Server.Extensions
{
    public static class QueryHandlerExt
    {
        public static void RegisterQueryHandlers(this WebApplicationBuilder builder)
        {
            // Register Query Handlers

            string queryDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ThuocGiaThatAdmin.Queries.dll");

            var queryHandlerAssembly = Assembly.LoadFrom(queryDllPath);
            var queryHandlerTypes = queryHandlerAssembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

            foreach (var handlerType in queryHandlerTypes)
            {
                var interfaceType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
                builder.Services.AddScoped(interfaceType, handlerType);
            }
        }
    }
}
