using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Application.Common.Interfaces;

namespace ThuocGiaThatAdmin.Common.Interfaces
{
    public interface IQueryDispatcher
    {
        Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query);
    }
}
