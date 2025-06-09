using FinalExamDAIS.Models;
using FinalExamDAIS.Repository.Interfaces.Account;
using FinalExamDAIS.Services.DTOs.Account;
using FinalExamDAIS.Services.Interfaces.Account;
using FinalExamDAIS.Repository.Interfaces.Account;

namespace FinalExamDAIS.Services.Implementations.Account
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        public async Task<AccountGetAllResponse> GetAccountsByUserIdAsync(int userId)
        {
            try
            {
                var accounts = await _accountRepository.GetAccountsByUserIdAsync(userId);
                var accountInfos = accounts.Select(MapToAccountInfo).ToList();

                return new AccountGetAllResponse
                {
                    Success = true,
                    Accounts = accountInfos
                };
            }
            catch (Exception ex)
            {
                return new AccountGetAllResponse
                {
                    Success = false,
                    Message = $"Грешка при извличане на акаунти: {ex.Message}"
                };
            }
        }

        public async Task<AccountInfo> GetAccountByIdAsync(int accountId)
        {
            var account = await _accountRepository.RetrieveAsync(accountId);
            return account != null ? MapToAccountInfo(account) : null;
        }

        public async Task<AccountInfo> GetAccountByNumberAsync(string accountNumber)
        {
            var filter = new FinalExamDAIS.Repository.Interfaces.Account.AccountFilter { AccountNumber = accountNumber };
            var accounts = new List<Models.Account>();
            
            await foreach (var account in _accountRepository.RetrieveCollectionAsync(filter))
            {
                if (account.AccountNumber == accountNumber)
                {
                    return MapToAccountInfo(account);
                }
            }
            
            return null;
        }

        public async Task<bool> UpdateAccountBalanceAsync(AccountAmountRequest request)
        {
            try
            {
                var account = await _accountRepository.RetrieveAsync(request.AccountId);
                if (account == null)
                    return false;

                var update = new AccountUpdate
                {
                    AvailableAmount = request.Amount
                };

                return await _accountRepository.UpdateAsync(request.AccountId, update);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HasSufficientFundsAsync(AccountAmountRequest request)
        {
            var account = await _accountRepository.RetrieveAsync(request.AccountId);
            return account != null && account.AvailableAmount >= request.Amount;
        }

        private static AccountInfo MapToAccountInfo(Models.Account account)
        {
            return new AccountInfo
            {
                AccountId = account.AccountId,
                AccountNumber = account.AccountNumber,
                AvailableAmount = account.AvailableAmount,
                IsActive = account.IsActive
            };
        }
    }
}
