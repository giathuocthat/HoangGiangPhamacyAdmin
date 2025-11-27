using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Common.Interfaces
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync(ICommand command);
        Task<TResponse> DispatchAsync<TResponse>(ICommand<TResponse> command);
    }
}
