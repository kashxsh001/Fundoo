using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Token is required")]
        public string token { get; set; }

        [Required(ErrorMessage = "New password is required")]
        public string newPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("newPassword", ErrorMessage = "Passwords do not match")]
        public string confirmPassword { get; set; }
    }
}
