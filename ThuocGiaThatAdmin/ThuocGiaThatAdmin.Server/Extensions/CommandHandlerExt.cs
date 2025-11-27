using System.Reflection;
using ThuocGiaThatAdmin.Common.Interfaces;

namespace ThuocGiaThatAdmin.Server.Extensions
{
    public static class CommandHandlerExt
    {
        public static void RegisterCommandHandlers(this WebApplicationBuilder builder)
        {
            // Register Command Handlers (non-returning)

            string queryDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ThuocGiaThatAdmin.Commands.dll");
            var commandHandlerAssembly = Assembly.LoadFrom(queryDllPath);

            // Register ICommandHandler<TCommand> handlers
            var commandHandlerTypes = commandHandlerAssembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)));

            foreach (var handlerType in commandHandlerTypes)
            {
                var interfaceType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType &&
                               i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));
                builder.Services.AddScoped(interfaceType, handlerType);
            }

            // Register ICommandHandler<TCommand, TResponse> handlers
            var commandHandlerWithResponseTypes = commandHandlerAssembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));

            foreach (var handlerType in commandHandlerWithResponseTypes)
            {
                var interfaceType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType &&
                               i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));
                builder.Services.AddScoped(interfaceType, handlerType);
            }
        }
    }
}
