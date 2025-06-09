using FinalExamDAIS.Models;
using FinalExamDAIS.Repository.Interfaces.Payment;
using FinalExamDAIS.Services.DTOs.Payment;
using FinalExamDAIS.Services.Interfaces.Payment;
using FinalExamDAIS.Repository.Interfaces.Account;
using FinalExamDAIS.Models.Enums;

namespace FinalExamDAIS.Services.Implementations.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAccountRepository _accountRepository;

        public PaymentService(IPaymentRepository paymentRepository, IAccountRepository accountRepository)
        {
            _paymentRepository = paymentRepository;
            _accountRepository = accountRepository;
        }

        public async Task<PaymentInfo> CreatePaymentAsync(CreatePaymentDto createPaymentDto)
        {
            if (createPaymentDto.Amount <= 0)
            {
                return new PaymentInfo { Success = false, Message = "Сумата трябва да бъде по-голяма от 0" };
            }

            if (Math.Round(createPaymentDto.Amount, 2) != createPaymentDto.Amount)
            {
                return new PaymentInfo { Success = false, Message = "Сумата трябва да има точно 2 десетични знака" };
            }

            if (string.IsNullOrWhiteSpace(createPaymentDto.Reason))
            {
                return new PaymentInfo { Success = false, Message = "Причината е задължителна" };
            }

            if (createPaymentDto.Reason.Length > 32)
            {
                return new PaymentInfo { Success = false, Message = "Причината не може да бъде по-дълга от 32 символа" };
            }

            if (string.IsNullOrWhiteSpace(createPaymentDto.ToAccountNumber) || createPaymentDto.ToAccountNumber.Length != 22)
            {
                return new PaymentInfo { Success = false, Message = "Невалиден номер на акаунт" };
            }

            if (!createPaymentDto.ToAccountNumber.All(c => char.IsLetterOrDigit(c)))
            {
                return new PaymentInfo { Success = false, Message = "Номерът на акаунта трябва да съдържа само букви и цифри" };
            }

            var fromAccount = await _accountRepository.RetrieveAsync(createPaymentDto.FromAccountId);
            if (fromAccount == null)
            {
                return new PaymentInfo { Success = false, Message = "Акаунтът не е намерен" };
            }

            if (!fromAccount.IsActive)
            {
                return new PaymentInfo { Success = false, Message = "Акаунтът не е активен" };
            }

            if (fromAccount.AccountNumber == createPaymentDto.ToAccountNumber)
            {
                return new PaymentInfo { Success = false, Message = "Не може да изпратите плащане към същия акаунт" };
            }

            var userAccounts = await _accountRepository.GetAccountsByUserIdAsync(createPaymentDto.CreatedByUserId);
            if (!userAccounts.Any(a => a.AccountId == createPaymentDto.FromAccountId))
            {
                return new PaymentInfo { Success = false, Message = "Нямате достъп до този акаунт" };
            }

            var payment = new Payments
            {
                FromAccountId = createPaymentDto.FromAccountId,
                ToAccountNumber = createPaymentDto.ToAccountNumber,
                Amount = createPaymentDto.Amount,
                Reason = createPaymentDto.Reason,
                Status = PaymentStatus.Pending.ToString(),
                CreatedDate = DateTime.UtcNow,
                CreatedByUserId = createPaymentDto.CreatedByUserId
            };

            var createdPaymentId = await _paymentRepository.CreateAsync(payment);
            return MapToPaymentInfo(await _paymentRepository.RetrieveAsync(createdPaymentId));
        }

        public async Task<PaymentInfo> GetPaymentByIdAsync(int paymentId)
        {
            if (paymentId <= 0)
            {
                return new PaymentInfo { Success = false, Message = "Невалиден ID на плащане" };
            }

            var payment = await _paymentRepository.RetrieveAsync(paymentId);
            return payment != null ? MapToPaymentInfo(payment) : new PaymentInfo { Success = false, Message = "Плащането не е намерено" };
        }

        public async Task<IEnumerable<PaymentInfo>> GetPaymentsByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                return new List<PaymentInfo> { new PaymentInfo { Success = false, Message = "Невалиден ID на потребител" } };
            }

            var filter = new PaymentFilter { UserId = userId };
            var payments = new List<Payments>();
            await foreach (var payment in _paymentRepository.RetrieveCollectionAsync(filter))
            {
                if(payment.CreatedByUserId == userId)
                {
                    payments.Add(payment);
                }
            }
            return payments.Select(MapToPaymentInfo);
        }

        public async Task<IEnumerable<PaymentInfo>> GetPendingPaymentsAsync()
        {
            var filter = new PaymentFilter { Status = PaymentStatus.Pending };
            var payments = new List<Payments>();
            await foreach (var payment in _paymentRepository.RetrieveCollectionAsync(filter))
            {
                payments.Add(payment);
            }
            return payments.Select(MapToPaymentInfo);
        }

        public async Task<PaymentInfo> ProcessPaymentAsync(int paymentId)
        {
            if (paymentId <= 0)
            {
                return new PaymentInfo { Success = false, Message = "Невалиден ID на плащане" };
            }

            var payment = await _paymentRepository.RetrieveAsync(paymentId);
            if (payment == null)
            {
                return new PaymentInfo { Success = false, Message = "Плащането не е намерено" };
            }

            if (payment.Status != PaymentStatus.Pending.ToString())
            {
                return new PaymentInfo { Success = false, Message = "Плащането не е в статус ИЗЧАКВА" };
            }

            var fromAccount = await _accountRepository.RetrieveAsync(payment.FromAccountId);
            if (fromAccount == null)
            {
                return new PaymentInfo { Success = false, Message = "Акаунтът не е намерен" };
            }

            if (!fromAccount.IsActive)
            {
                return new PaymentInfo { Success = false, Message = "Акаунтът не е активен" };
            }

            if (fromAccount.AvailableAmount < payment.Amount)
            {
                return new PaymentInfo { Success = false, Message = "Недостатъчна наличност по акаунта" };
            }
    
            var toAccount = await _accountRepository.GetAccountByNumberAsync(payment.ToAccountNumber);
            if (toAccount == null)
            {
                return new PaymentInfo { Success = false, Message = "Получателят не е намерен" };
            }

            if (!toAccount.IsActive)
            {
                return new PaymentInfo { Success = false, Message = "Акаунтът на получателя не е активен" };
            }

            fromAccount.AvailableAmount -= payment.Amount;
            var fromAccountUpdate = new AccountUpdate { AvailableAmount = fromAccount.AvailableAmount };
            var fromAccountUpdated = await _accountRepository.UpdateAsync(fromAccount.AccountId, fromAccountUpdate);
            if (!fromAccountUpdated)
            {
                return new PaymentInfo { Success = false, Message = "Грешка при обновяване на баланса на източника" };
            }

            toAccount.AvailableAmount += payment.Amount;
            var toAccountUpdate = new AccountUpdate { AvailableAmount = toAccount.AvailableAmount };
            var toAccountUpdated = await _accountRepository.UpdateAsync(toAccount.AccountId, toAccountUpdate);
            if (!toAccountUpdated)
            {
                fromAccount.AvailableAmount += payment.Amount;
                await _accountRepository.UpdateAsync(fromAccount.AccountId, fromAccountUpdate);
                return new PaymentInfo { Success = false, Message = "Грешка при обновяване на баланса на получателя" };
            }

            payment.Status = PaymentStatus.Processed.ToString();
            payment.ProcessedDate = DateTime.UtcNow;
            await _paymentRepository.UpdateAsync(paymentId, new PaymentUpdate { Status = PaymentStatus.Processed, ProcessedDate = DateTime.UtcNow });
            return MapToPaymentInfo(payment);
        }

        public async Task<PaymentInfo> RejectPaymentAsync(int paymentId)
        {
            if (paymentId <= 0)
            {
                return new PaymentInfo { Success = false, Message = "Невалиден ID на плащане" };
            }

            var payment = await _paymentRepository.RetrieveAsync(paymentId);
            if (payment == null)
            {
                return new PaymentInfo { Success = false, Message = "Плащането не е намерено" };
            }

            if (payment.Status != PaymentStatus.Pending.ToString())
            {
                return new PaymentInfo { Success = false, Message = "Плащането не е в статус ИЗЧАКВА" };
            }

            payment.Status = PaymentStatus.Canceled.ToString();
            await _paymentRepository.UpdateAsync(paymentId, new PaymentUpdate { Status = PaymentStatus.Canceled });
            return MapToPaymentInfo(payment);
        }

        private static PaymentInfo MapToPaymentInfo(Payments payment)
        {
            return new PaymentInfo
            {
                Success = true,
                PaymentId = payment.PaymentId,
                FromAccountId = payment.FromAccountId,
                ToAccountNumber = payment.ToAccountNumber,
                Amount = payment.Amount,
                Reason = payment.Reason,
                Status = payment.Status,
                CreatedDate = payment.CreatedDate,
                ProcessedDate = payment.ProcessedDate,
                CreatedByUserId = payment.CreatedByUserId
            };
        }
    }
}