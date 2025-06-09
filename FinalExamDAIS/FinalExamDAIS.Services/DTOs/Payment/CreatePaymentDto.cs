using System.ComponentModel.DataAnnotations;

namespace FinalExamDAIS.Services.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Required]
        public int FromAccountId { get; set; }

        [Required]
        [StringLength(22, MinimumLength = 22)]
        public string ToAccountNumber { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(32)]
        public string Reason { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }
    }
}