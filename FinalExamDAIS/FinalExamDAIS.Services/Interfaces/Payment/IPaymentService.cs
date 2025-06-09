using FinalExamDAIS.Services.DTOs.Payment;

namespace FinalExamDAIS.Services.Interfaces.Payment
{
    public interface IPaymentService
    {
        Task<PaymentInfo> CreatePaymentAsync(CreatePaymentDto createPaymentDto);

        Task<PaymentInfo> GetPaymentByIdAsync(int paymentId);

        Task<IEnumerable<PaymentInfo>> GetPaymentsByUserIdAsync(int userId);

        Task<IEnumerable<PaymentInfo>> GetPendingPaymentsAsync();

        Task<PaymentInfo> ProcessPaymentAsync(int paymentId);

        Task<PaymentInfo> RejectPaymentAsync(int paymentId);
    }
}