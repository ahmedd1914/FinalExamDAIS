using FinalExamDAIS.Models;
using FinalExamDAIS.Repository.Base;

namespace FinalExamDAIS.Repository.Interfaces.Account
{
    public interface IAccountRepository : IBaseRepository<Models.Account, AccountFilter, AccountUpdate>
    {
        Task<IEnumerable<Models.Account>> GetAccountsByUserIdAsync(int userId);
        Task<Models.Account> GetAccountByNumberAsync(string accountNumber);
    }
}   