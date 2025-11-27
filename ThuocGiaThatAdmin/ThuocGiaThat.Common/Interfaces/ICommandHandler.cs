using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Common.Interfaces
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }

    public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse> 
    {
        Task<TResponse> HandleAsync(TCommand command);
    }
}
