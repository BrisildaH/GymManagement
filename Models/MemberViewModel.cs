using System.ComponentModel.DataAnnotations;

namespace Gym.Models
{
    public class MemberViewModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Display(Name = "Birth Date")]
        [Range(typeof(DateTime), "1900-01-01", "2009-12-31", ErrorMessage = "Member must be over 15 years old.")]
        public DateTime Birthdate { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [RegularExpression(@"^[A-Za-z][0-9]{8}[A-Za-z]$",
                ErrorMessage = "ID Card Number must start and end with a letter and contain 8 digits in between.")]
        public string IdCardNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? UpdateDate { get; set; }
    
    }
}
