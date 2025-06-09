using System.Data.SqlTypes;

namespace FinalExamDAIS.Repository.Interfaces.Account
{
    public class AccountUpdate
    {
        public SqlDecimal? AvailableAmount { get; set; }
    }
}