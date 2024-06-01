using System.ComponentModel.DataAnnotations;

namespace Gym.Data.Entities
{
    public class Discount
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public decimal Value { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public bool? DeactivationDate { get; set; } // (If this field is true, it means that the discount is no longer available)
        public bool? IsDeleted { get; set; }
    }
}
   