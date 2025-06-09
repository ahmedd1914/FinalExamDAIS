namespace FinalExamDAIS.Services.DTOs.Account
{
    public class AccountFilter
    {
        public bool? IsActive { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

    }
}