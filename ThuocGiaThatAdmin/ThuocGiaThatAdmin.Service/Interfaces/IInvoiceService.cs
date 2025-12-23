using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceInfoDto?> GetDefault(int customerId);
        Task<List<InvoiceInfoDto>> GetInvoices(int customerId);
        Task<InvoiceInfoDto> CreateOrUpdate(InvoiceInfoDto dto);
    }
}
