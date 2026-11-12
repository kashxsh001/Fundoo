using System.ComponentModel.DataAnnotations;
namespace ModelLayer
{
    public class RegisterationModel
    {
        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[A-Za-z]{2,50}$",ErrorMessage = "First name: only letters, 2 to 50 characters")]
        public string fname { get; set; }

        [RegularExpression(@"^[A-Za-z]{2,50}$", ErrorMessage = "Last name: only letters, 2 to 50 characters")]
        public string lname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string contactno { get; set; }
    }
}
