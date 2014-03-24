using CallTracking.Web.Validation;
using System.ComponentModel.DataAnnotations;

namespace CallTracking.Web.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Please tell us your email address so we can reply.")]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Company")]
        public string Company { get; set; }

        [Display(Name = "Contact Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage= "Please let us know some details about your enquiry.")]
        [Display(Name = "How can we help?")]
        public string Enquiry { get; set; }
        //        @Html.HiddenFor(model => model.ID)
    }
}