using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Application.Common.Interfaces;
using ThuocGiaThatAdmin.Contracts.Responses;

namespace ThuocGiaThatAdmin.Queries.Categories
{
    public class GetCategoryFlatQuery : IQuery<IEnumerable<CategoryFlatResponse>>
    {
    }
}
