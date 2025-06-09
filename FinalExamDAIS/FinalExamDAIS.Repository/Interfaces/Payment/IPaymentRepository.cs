using FinalExamDAIS.Repository.Base;

namespace FinalExamDAIS.Repository.Interfaces.Payment
{
    public interface IPaymentRepository : IBaseRepository<Models.Payments, PaymentFilter, PaymentUpdate>
    {
        Task<IEnumerable<Models.Payments>> GetPaymentsByUserIdAsync(int userId);
        Task<IEnumerable<Models.Payments>> GetPendingPaymentsAsync();
    }
}