using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Interfaces;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class BusinessTypeRepository : Repository<BusinessType>, IBusinessTypeRepository
    {
        public BusinessTypeRepository(TrueMecContext context) : base(context)
        {
        }
    }
}
