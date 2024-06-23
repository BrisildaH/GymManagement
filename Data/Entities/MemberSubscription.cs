﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gym.Data.Entities
{
    public class MemberSubscription
    {

        [Key]
        public int ID { get; set; }

        #region Member Relationship
        [ForeignKey("Member")]
        public int MemberID { get; set; }
        public Member Member { get; set; }
        #endregion

        #region Subscriber Relationship
        [ForeignKey("Subscription")]
        public int SubscriptionID { get; set; }
        public Subscription Subscription { get; set; }
        #endregion

        public decimal OriginalPrice { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal PaidPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RemainingSessions { get; set; }
        public bool IsDeleted { get; set; }
    }
}

