using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service implementation for CustomerPaymentAccount business logic
    /// </summary>
    public class CustomerPaymentAccountService : ICustomerPaymentAccountService
    {
        private readonly ICustomerPaymentAccountRepository _accountRepository;
        private readonly ICustomerRepository _customerRepository;

        public CustomerPaymentAccountService(
            ICustomerPaymentAccountRepository accountRepository,
            ICustomerRepository customerRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        public async Task<IEnumerable<CustomerPaymentAccountDto>> GetAllAsync()
        {
            return await _accountRepository.GetAllAsync();
        }

        public async Task<CustomerPaymentAccountDto?> GetByIdAsync(int id)
        {
            return await _accountRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<CustomerPaymentAccountDto>> GetByCustomerIdAsync(int customerId)
        {
            return await _accountRepository.GetByCustomerIdAsync(customerId);
        }

        public async Task<CustomerPaymentAccountDto?> GetDefaultAccountAsync(int customerId)
        {
            return await _accountRepository.GetDefaultAccountAsync(customerId);
        }

        public async Task<CustomerPaymentAccountDto> CreateAsync(CreateCustomerPaymentAccountDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Verify customer exists
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {dto.CustomerId} not found");

            // Check if this is the first account for the customer
            var existingAccounts = await _accountRepository.GetByCustomerIdAsync(dto.CustomerId);
            var isFirstAccount = !existingAccounts.Any();

            var accountDto = new CustomerPaymentAccountDto
            {
                CustomerId = dto.CustomerId,
                AccountType = dto.AccountType,
                BankId = dto.BankId,
                AccountNumber = dto.AccountNumber,
                AccountHolder = dto.AccountHolder,
                BankBranch = dto.BankBranch,
                SwiftCode = dto.SwiftCode,
                IsDefault = isFirstAccount || dto.IsDefault, // First account is always default
                IsActive = true,
                Notes = dto.Notes
            };

            var createdAccount = await _accountRepository.CreateAsync(accountDto);

            // If setting as default, unset other defaults
            if (createdAccount.IsDefault && !isFirstAccount)
            {
                await _accountRepository.SetDefaultAccountAsync(createdAccount.Id, dto.CustomerId);
            }

            return createdAccount;
        }

        public async Task<CustomerPaymentAccountDto> UpdateAsync(int id, UpdateCustomerPaymentAccountDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existingAccount = await _accountRepository.GetByIdAsync(id);
            if (existingAccount == null)
                throw new InvalidOperationException($"CustomerPaymentAccount with ID {id} not found");

            var updatedDto = new CustomerPaymentAccountDto
            {
                Id = id,
                CustomerId = existingAccount.CustomerId,
                AccountType = dto.AccountType,
                BankId = dto.BankId,
                AccountNumber = dto.AccountNumber,
                AccountHolder = dto.AccountHolder,
                BankBranch = dto.BankBranch,
                SwiftCode = dto.SwiftCode,
                IsDefault = existingAccount.IsDefault, // Cannot change default via update
                IsActive = dto.IsActive,
                Notes = dto.Notes
            };

            return await _accountRepository.UpdateAsync(updatedDto);
        }

        public async Task DeleteAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
                throw new InvalidOperationException($"CustomerPaymentAccount with ID {id} not found");

            // If deleting default account, set another account as default
            if (account.IsDefault)
            {
                var otherAccounts = await _accountRepository.GetByCustomerIdAsync(account.CustomerId);
                var nextDefault = otherAccounts.FirstOrDefault(a => a.Id != id && a.IsActive);

                if (nextDefault != null)
                {
                    await _accountRepository.SetDefaultAccountAsync(nextDefault.Id, account.CustomerId);
                }
            }

            await _accountRepository.DeleteAsync(id);
        }

        public async Task SetDefaultAccountAsync(int accountId, int customerId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
                throw new InvalidOperationException($"CustomerPaymentAccount with ID {accountId} not found");

            if (account.CustomerId != customerId)
                throw new InvalidOperationException($"Account {accountId} does not belong to customer {customerId}");

            if (!account.IsActive)
                throw new InvalidOperationException($"Cannot set inactive account as default");

            await _accountRepository.SetDefaultAccountAsync(accountId, customerId);
        }
    }
}
