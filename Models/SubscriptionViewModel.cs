using System.ComponentModel.DataAnnotations;

namespace Gym.Models
{
    public class SubscriptionViewModel
    {

        public int ID { get; set; }

        public string Code { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Number of months is required")]
        [Range(1, 12, ErrorMessage = "Number of months must be between 1 and 12")]
        public int NumberOfMonths { get; set; }

        [Required(ErrorMessage = "Week frequency is required")]
        public string WeekFrequency { get; set; }

        public int? TotalNumberOfSessions { get; set; }

        [Required(ErrorMessage = "Total price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Total price must be a positive number")]
        public decimal TotalPrice { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
    