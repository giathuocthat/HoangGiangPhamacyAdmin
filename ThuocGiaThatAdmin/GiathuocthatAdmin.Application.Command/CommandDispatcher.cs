using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Common.Interfaces;

namespace ThuocGiaThatAdmin.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task DispatchAsync(ICommand command)
        {
            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);

            var handler = _serviceProvider.GetService(handlerType)
                ?? throw new InvalidOperationException($"No handler registered for command {commandType.Name}");

            var method = handlerType.GetMethod("HandleAsync", BindingFlags.Public | BindingFlags.Instance);
            await (Task)method!.Invoke(handler, new object[] { command })!;
        }

        public async Task<TResponse> DispatchAsync<TResponse>(ICommand<TResponse> command)
        {
            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));

            var handler = _serviceProvider.GetService(handlerType)
                ?? throw new InvalidOperationException($"No handler registered for command {commandType.Name}");

            var method = handlerType.GetMethod("HandleAsync", BindingFlags.Public | BindingFlags.Instance);
            var result = await (Task<TResponse>)method!.Invoke(handler, new object[] { command })!;

            return result;
        }
    }
}
