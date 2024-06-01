using System.ComponentModel.DataAnnotations;

namespace Gym.Data.Entities
{
    public class Subscription
    {
        [Key]
        public int ID { get; set; }

        public string Code { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Number of months must be between 1 and 12")]
        public int NumberOfMonths { get; set; }

        [Required(ErrorMessage = "Week frequency is required")]
        public string WeekFrequency { get; set; }

        public int? TotalNumberOfSessions { get; set; }

        public decimal? TotalPrice { get; set; }

        public bool? IsDeleted { get; set; }
    }
}

