using System.Data.SqlTypes;
using FinalExamDAIS.Repository.Helpers;

namespace FinalExamDAIS.Repository.Interfaces.User
{
    public class UserFilter
    {
        public SqlString? Username { get; set; }

        public Filter ToFilter()
        {
            var filter = new Filter();
            
            if (Username.HasValue && !Username.Value.IsNull)
                filter.AddCondition("Username", Username.Value.Value);

            return filter;
        }
    }
}
