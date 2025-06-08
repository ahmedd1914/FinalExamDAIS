using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalExamDAIS.Repository.Base;

namespace FinalExamDAIS.Repository.Interfaces.User
{
    public interface IUserRepository : IBaseRepository<Models.User, UserFilter, UserUpdate>
    {
        Task<Models.User> GetByEmailAsync(string email);
        Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash);
    }
}
