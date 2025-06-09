using System.Data.SqlTypes;
using FinalExamDAIS.Repository.Helpers;

namespace FinalExamDAIS.Repository.Interfaces.Account
{
    public class AccountFilter
    {
        public SqlBoolean? IsActive { get; set; }
        public SqlString AccountNumber { get; set; }    

        public Filter ToFilter()
        {
            var filter = new Filter();
            if (IsActive.HasValue)
                filter.AddCondition("IsActive", IsActive.Value);
            if (AccountNumber != null)
                filter.AddCondition("AccountNumber", AccountNumber);
            return filter;
        }
    }
    
    
}       