using Gym.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym.Models
{
    public class MemberSubscriptionModel
    {

        public int ID { get; set; }
        public int MemberID { get; set; }
        public int SubscriptionID { get; set; }
        public string MemberCardId { get; set; }
        public string FirstName { get; set; }
        public string SubscriptionCode { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal PaidPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RemainingSessions { get; set; }
        public bool IsDeleted { get; set; }
    }
}


