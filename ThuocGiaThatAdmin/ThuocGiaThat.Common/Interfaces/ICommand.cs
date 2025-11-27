using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Common.Interfaces
{
    public interface ICommand
    {
    }

    public interface ICommand<out TResponse>
    {
    }
}
