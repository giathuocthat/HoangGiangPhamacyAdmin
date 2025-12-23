using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly TrueMecContext _context;
        public InvoiceService(TrueMecContext context)
        {
            _context = context;
        }

        public async Task<InvoiceInfoDto?> GetDefault(int customerId)
        {
            IQueryable<CustomerInvoiceInfo> query = _context.CustomerInvoiceInfos.Where(x => x.CustomerId == customerId && x.IsVerfied && x.IsDefault);
            if (await query.AnyAsync())
            {
                return await query.Select(x => new InvoiceInfoDto
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    CompanyName = x.CompanyName,
                    InvoiceAddress = x.InvoiceAddress,
                    IsDefault = x.IsDefault,
                    IsVerified = x.IsVerfied,
                    Note = x.Note,
                    TaxCode = x.TaxCode,
                }).FirstOrDefaultAsync();
            } else
            {
                return await _context.CustomerInvoiceInfos.Where(x => x.CustomerId == customerId && x.IsVerfied).Select(x => new InvoiceInfoDto
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    CompanyName = x.CompanyName,
                    InvoiceAddress = x.InvoiceAddress,
                    IsDefault = x.IsDefault,
                    IsVerified = x.IsVerfied,
                    Note = x.Note,
                    TaxCode = x.TaxCode,
                }).FirstOrDefaultAsync();
            }
        }

        public async Task<List<InvoiceInfoDto>> GetInvoices(int customerId)
        {
            return await _context.CustomerInvoiceInfos.Where(x => x.CustomerId == customerId).Select(x => new InvoiceInfoDto
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                CompanyName = x.CompanyName,
                InvoiceAddress = x.InvoiceAddress,
                IsDefault = x.IsDefault,
                IsVerified = x.IsVerfied,
                Note = x.Note,
                TaxCode = x.TaxCode,
                BuyerName = x.BuyerName
            }).ToListAsync();
        }

        public async Task<InvoiceInfoDto> CreateOrUpdate(InvoiceInfoDto dto)
        {
            CustomerInvoiceInfo entity;
            if (dto.Id.HasValue)
            {
                entity = await _context.CustomerInvoiceInfos.Where(x => x.Id == dto.Id).FirstOrDefaultAsync();
                entity.CompanyName = dto.CompanyName;
                entity.BuyerName = dto.BuyerName;
                entity.CustomerId = dto.CustomerId;
                entity.InvoiceAddress = dto.InvoiceAddress;                
                entity.TaxCode = dto.TaxCode;
                entity.UpdatedDate = DateTime.UtcNow;
                entity.IsVerfied = false;
                
                await _context.SaveChangesAsync();
                    
            } else
            {
                entity = new CustomerInvoiceInfo
                {
                    CompanyName = dto.CompanyName,
                    CreatedDate = DateTime.UtcNow,
                    CustomerId = dto.CustomerId,
                    InvoiceAddress = dto.InvoiceAddress,
                    IsDefault = dto.IsDefault,
                    IsActive = true,
                    IsVerfied = true,
                    TaxCode = dto.TaxCode,
                    UpdatedDate = DateTime.UtcNow,
                    BuyerName = dto.BuyerName
                };

                _context.CustomerInvoiceInfos.Add(entity);
            }

            await _context.SaveChangesAsync();

            return new InvoiceInfoDto
            {
                CompanyName = entity.CompanyName,
                CustomerId = entity.CustomerId,
                InvoiceAddress = entity.InvoiceAddress,
                IsDefault = entity.IsDefault,
                Id = entity.Id,
                TaxCode = entity.TaxCode,
                IsVerified = entity.IsVerfied,
                IsActive = entity.IsActive,
                Note = entity.Note,
            };
        }
    }
}
