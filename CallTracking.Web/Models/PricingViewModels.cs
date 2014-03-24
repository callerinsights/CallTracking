using CallTracking.Web.Validation;
using System.ComponentModel.DataAnnotations;

namespace CallTracking.Web.Models
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [DataType(DataType.EmailAddress)]
        [Email(ErrorMessage = "The email address is invalid.")]
        [Display(Name = "Email address")]
        public string Email { get; set; }
        public string ReferredBy { get; set; }
        public string Message { get; set; }
    }
}
