using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using FinalExamDAIS.Services.DTOs.Payment;

namespace FinalExamDAIS.Web.Models.ViewModels.Payment
{
    public class PaymentListViewModel
    {
        public List<PaymentDisplayViewModel> Payments { get; set; }
        public string SortBy { get; set; }
    }
} 