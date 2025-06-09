namespace FinalExamDAIS.Services.DTOs.Account
{
    public class AccountInfo
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public decimal AvailableAmount { get; set; }
        public bool IsActive { get; set; }
    }
}