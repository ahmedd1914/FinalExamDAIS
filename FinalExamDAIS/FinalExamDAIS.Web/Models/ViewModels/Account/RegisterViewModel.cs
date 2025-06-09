using System.ComponentModel.DataAnnotations;

namespace FinalExamDAIS.Web.Models.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Потребителското име е задължително")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Потребителското име трябва да е между 3 и 50 символа")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Потребителското име трябва да съдържа само букви и цифри")]
        [Display(Name = "Потребителско име")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Имейлът е задължителен")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Имейлът трябва да е между 3 и 100 символа")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Невалиден имейл адрес")]
        [Display(Name = "Имейл")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Паролата е задължителна")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Паролата трябва да е поне 8 символа")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Паролата трябва да е поне 8 символа и да съдържа главна буква, малка буква, цифра и специален символ")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Името трябва да е между 2 и 50 символа")]
        [Display(Name = "Име")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилията е задължителна")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Фамилията трябва да е между 2 и 50 символа")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Датата на раждане е задължителна")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата на раждане")]
        public DateTime DateOfBirth { get; set; }
    }
}
