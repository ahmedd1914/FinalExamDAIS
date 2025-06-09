using FinalExamDAIS.Services.DTOs;
using System.Collections.Generic;

namespace FinalExamDAIS.Services.DTOs.Account
{
    public class AccountGetAllResponse : BaseResponse<List<AccountInfo>>
    {
        public List<AccountInfo> Accounts
        {
            get => Data;
            set => Data = value;
        }
    }
}