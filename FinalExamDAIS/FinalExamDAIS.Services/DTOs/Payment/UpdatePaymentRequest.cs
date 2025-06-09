namespace FinalExamDAIS.Services.DTOs.Payment
{
    public class UpdatePaymentRequest : BaseResponse
    {
        public int PaymentId { get; set; }
        public string Status { get; set; } // "ИЗЧАКВА", "ОБРАБОТЕН", "ОТКАЗАН"
        public DateTime? ProcessedDate { get; set; }
        public string ErrorMessage { get; set; }
    }
}