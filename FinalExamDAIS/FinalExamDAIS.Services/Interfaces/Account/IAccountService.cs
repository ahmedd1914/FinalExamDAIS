using FinalExamDAIS.Services.DTOs.Account;
using System.Threading.Tasks;

namespace FinalExamDAIS.Services.Interfaces.Account
{
    public interface IAccountService
    {
        Task<AccountGetAllResponse> GetAccountsByUserIdAsync(int userId);
        Task<AccountInfo> GetAccountByIdAsync(int accountId);
        Task<AccountInfo> GetAccountByNumberAsync(string accountNumber);
        Task<bool> UpdateAccountBalanceAsync(AccountAmountRequest request);
        Task<bool> HasSufficientFundsAsync(AccountAmountRequest request);
    }
}