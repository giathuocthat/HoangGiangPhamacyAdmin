using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service implementation for Bank operations
    /// </summary>
    public class BankService : IBankService
    {
        private readonly IBankRepository _bankRepository;

        public BankService(IBankRepository bankRepository)
        {
            _bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
        }

        public async Task<List<BankDto>> GetAllBanksAsync()
        {
            return await _bankRepository.GetAllBanksAsync();
        }

        public async Task<List<BankDto>> GetActiveBanksAsync()
        {
            return await _bankRepository.GetActiveBanksAsync();
        }

        public async Task<BankDto?> GetByIdAsync(int id)
        {
            return await _bankRepository.GetBankByIdAsync(id);
        }

        public async Task<BankDto?> GetByCodeAsync(string bankCode)
        {
            return await _bankRepository.GetBankByCodeAsync(bankCode);
        }

        public async Task<BankDto> CreateAsync(CreateBankDto dto)
        {
            // Validate bank code uniqueness
            if (await _bankRepository.BankCodeExistsAsync(dto.BankCode))
            {
                throw new InvalidOperationException($"Bank with code '{dto.BankCode}' already exists");
            }

            var bank = new Bank
            {
                BankCode = dto.BankCode,
                BankName = dto.BankName,
                FullName = dto.FullName,
                IsActive = dto.IsActive,
                DisplayOrder = dto.DisplayOrder,
                CreatedDate = DateTime.UtcNow
            };

            var createdBank = await _bankRepository.CreateAsync(bank);

            // Return the created bank as DTO
            return await _bankRepository.GetBankByIdAsync(createdBank.Id)
                ?? throw new InvalidOperationException("Failed to retrieve created bank");
        }

        public async Task<BankDto> UpdateAsync(int id, UpdateBankDto dto)
        {
            var existingBank = await _bankRepository.GetBankByIdAsync(id);
            if (existingBank == null)
            {
                throw new InvalidOperationException($"Bank with ID {id} not found");
            }

            // Validate bank code uniqueness (excluding current bank)
            if (await _bankRepository.BankCodeExistsAsync(dto.BankCode, id))
            {
                throw new InvalidOperationException($"Bank with code '{dto.BankCode}' already exists");
            }

            var bank = new Bank
            {
                Id = id,
                BankCode = dto.BankCode,
                BankName = dto.BankName,
                FullName = dto.FullName,
                IsActive = dto.IsActive,
                DisplayOrder = dto.DisplayOrder,
                CreatedDate = existingBank.CreatedDate,
                UpdatedDate = DateTime.UtcNow
            };

            await _bankRepository.UpdateAsync(bank);

            // Return the updated bank as DTO
            return await _bankRepository.GetBankByIdAsync(id)
                ?? throw new InvalidOperationException("Failed to retrieve updated bank");
        }

        public async Task DeleteAsync(int id)
        {
            var bank = await _bankRepository.GetBankByIdAsync(id);
            if (bank == null)
            {
                throw new InvalidOperationException($"Bank with ID {id} not found");
            }

            await _bankRepository.DeleteAsync(id);
        }
    }
}
