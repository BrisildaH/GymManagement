using System.ComponentModel.DataAnnotations;

namespace Gym.Models
{
    public class MemberSubscriptionCreateViewModel
    {
        public SubscriptionViewModel SubscriptionViewModel { get; set; }

        [Display(Name = "Member")]
        [Required(ErrorMessage = "Please select a member.")]
        public int MemberId { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Please enter a start date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        public decimal Discount{ get; set; }
    }
}
