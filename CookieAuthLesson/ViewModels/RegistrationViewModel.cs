using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CookieAuthLesson.ViewModels
{
    public class RegistrationViewModel
    {
        [EmailAddress(ErrorMessage = "Введен некоректный Email")]
        [Required]
        public string Email { get; set; }

        [MinLength(6)]
        [Required]
        public string Password { get; set; }

        [MinLength(6)]
        [Required]
        public string SecondPassword { get; set; }
    }
}
