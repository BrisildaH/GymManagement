using Gym.Data;
using Gym.Data.Entities;
using Gym.Interface;
using Gym.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gym.Service
{
    public class MemberSubscriptionService : IMemberSubscriptionService
    {
        private readonly ApplicationDbContext _context;

        public MemberSubscriptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<MemberSubscriptionModel> GetAllMemberSubscriptions()
        {
            try
            {
                var memberSubscriptionModels = new List<MemberSubscriptionModel>();
                var memberSubscriptions = _context.MemberSubscriptions
                    .Include(ms => ms.Subscription)
                    .Include(ms => ms.Member)
                    .Where(ms => ms.IsDeleted == false && ms.Member.IsDeleted == false && ms.Subscription.IsDeleted == false)
                    .ToList();

                foreach (var memberSubscription in memberSubscriptions)
                {
                    memberSubscriptionModels.Add(EntityToViewModel(memberSubscription));
                }

                return memberSubscriptionModels;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all member subscriptions.", ex);
            }
        }

        public bool MemberSubscriptionExist(MemberSubscriptionModel memberSubscriptionVM)
        {
            try
            {
                var member = _context.Members.FirstOrDefault(m => m.ID == memberSubscriptionVM.MemberID);
                return member != null && _context.MemberSubscriptions.Any(ms => ms.MemberID == member.ID && !ms.IsDeleted);
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking if member subscription exists.", ex);
            }
        }

        public ServiceResult ActivateSubscription(MemberSubscriptionModel vm)
        {
            var result = new ServiceResult();

            try
            {
                var memberSubscription = ViewModelToEntity(vm);

                var memberExist = _context.Members.Any(m => m.ID == vm.MemberID && m.IsDeleted == false);
                var subscription = _context.Subscriptions.FirstOrDefault(s => s.Code == vm.SubscriptionCode && s.IsDeleted == false);

                if (!memberExist || subscription == null)
                {
                    result.Success = false;
                    result.Message = "Error: Member or subscription not found.";
                    return result;
                }

                decimal discountValue = CalculateDiscount(subscription.NumberOfMonths, (decimal)subscription.TotalPrice);
                decimal paidPrice = (decimal)(subscription.TotalPrice - discountValue);

                memberSubscription.OriginalPrice = (decimal)subscription.TotalPrice;
                memberSubscription.PaidPrice = paidPrice;
                memberSubscription.DiscountValue = discountValue;

                if (vm.StartDate < DateTime.Today)
                {
                    result.Success = false;
                    result.Message = "Error: Start date cannot be in the past.";
                    return result;
                }

                var existingSubscription = _context.MemberSubscriptions.Any(ms => ms.MemberID == vm.MemberID && ms.StartDate <= vm.StartDate && ms.EndDate >= vm.StartDate && ms.IsDeleted == false);
                if (existingSubscription)
                {
                    result.Success = false;
                    result.Message = "Error: The member already has an active subscription for the selected period.";
                    return result;
                }

                memberSubscription.StartDate = vm.StartDate;
                memberSubscription.EndDate = vm.StartDate.AddMonths(subscription.NumberOfMonths);

                _context.MemberSubscriptions.Add(memberSubscription);
                _context.SaveChanges();

                result.Success = true;
                result.Message = "Subscription activated successfully.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error activating subscription: " + ex.Message;
            }

            return result;
        }


        //public MemberSubscriptionModel GetMemberSubscriptionByDetail(int id, string memberCardID, string subscriptionCode)
        //{
        //    try
        //    {
        //        var member = _context.Members.FirstOrDefault(m => m.IdCardNumber == memberCardID);
        //        var subscription = _context.Subscriptions.FirstOrDefault(s => s.Code == subscriptionCode);

        //        if (member == null || subscription == null)
        //        {
        //            throw new Exception("Error: Member or subscription not found.");
        //        }

        //        var memberSubscription = _context.MemberSubscriptions.FirstOrDefault(ms => ms.MemberID == member.ID && ms.SubscriptionID == subscription.ID);

        //        if (memberSubscription == null)
        //        {
        //            throw new Exception("Error: Member subscription not found.");
        //        }

        //        return EntityToViewModel(memberSubscription);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error getting member subscription.", ex);
        //    }
        //}



        public List<SelectListItem> GetMembersCardID()
        {
            try
            {
                var memberCardID = _context.Members.Where(m => (bool)!m.IsDeleted)
                                                    .Select(mc => new SelectListItem
                                                    {
                                                        Value = mc.IdCardNumber,
                                                        Text = $"Full Name: {mc.FirstName} {mc.LastName}, Card Id: {mc.IdCardNumber}"
                                                    })
                                                    .ToList();
                return memberCardID;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving member card IDs.", ex);
            }
        }

        public List<SelectListItem> GetSubscriberCode()
        {
            try
            {
                var subscriptionsCode = _context.Subscriptions.Where(s => (bool)!s.IsDeleted)
                                                               .Select(sub => new SelectListItem
                                                               {
                                                                   Value = sub.Code,
                                                                   Text = $"Subscription Code: {sub.Code}, Description: {sub.Description}"
                                                               })
                                                               .ToList();
                return subscriptionsCode;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving subscription codes.", ex);
            }
        }
        public List<MemberSubscriptionModel> SearchMemberSubscriptions(string keyword)
        {
            try
            {
                var memberSubscriptions = _context.MemberSubscriptions
                    .Include(ms => ms.Subscription)
                    .Include(ms => ms.Member)
                    .Where(ms => ms.IsDeleted == false && ms.Member.IsDeleted == false && ms.Subscription.IsDeleted == false)
                    .ToList();

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return memberSubscriptions.Select(EntityToViewModel).ToList();
                }

                var filteredMemberSubscriptions = memberSubscriptions.Where(ms =>
                    ms.Subscription.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    ms.Subscription.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    ms.Subscription.NumberOfMonths.ToString().Contains(keyword) ||
                    ms.Subscription.TotalNumberOfSessions.ToString().Contains(keyword) ||
                    ms.Subscription.TotalPrice.ToString().Contains(keyword) ||
                    ms.Subscription.WeekFrequency.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    ms.Member.FirstName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    ms.Member.LastName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                return filteredMemberSubscriptions.Select(EntityToViewModel).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching member subscriptions: " + ex.Message);
            }
        }
        public bool CheckIn(int subscriptionId)
        {
            try
            {
                var subscription = _context.MemberSubscriptions.Find(subscriptionId);

                if (subscription != null && subscription.RemainingSessions > 0)
                {
                    subscription.RemainingSessions--;
                    _context.Update(subscription);
                    _context.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {

                throw new ApplicationException("Error while checking in the member subscription.", ex);
            }
        }

    private decimal CalculateDiscount(int numberOfMonths, decimal totalPrice)
        {
            switch (numberOfMonths)
            {
                case 3:
                    return 0.10m * totalPrice;
                case 6:
                    return 0.20m * totalPrice;
                case 12:
                    return 0.25m * totalPrice;
                default:
                    return 0;
            }
        }
        private MemberSubscription ViewModelToEntity(MemberSubscriptionModel vm)
        {
            var memberSubscription = new MemberSubscription()
            {
                ID = vm.ID,
                MemberID = vm.MemberID,
                SubscriptionID = vm.SubscriptionID,
                OriginalPrice = vm.OriginalPrice,
                DiscountValue = vm.DiscountValue,
                PaidPrice = vm.PaidPrice,
                StartDate = vm.StartDate,
                IsDeleted = vm.IsDeleted,
                RemainingSessions = vm.RemainingSessions,
                EndDate = vm.EndDate,
            };
            return memberSubscription;
        }
        private MemberSubscriptionModel EntityToViewModel(MemberSubscription entity)
        {
            return new MemberSubscriptionModel
            {
                ID = entity.ID,
                MemberID = entity.MemberID,
                SubscriptionID = entity.SubscriptionID,
                OriginalPrice = entity.OriginalPrice,
                DiscountValue = entity.DiscountValue,
                PaidPrice = entity.PaidPrice,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                RemainingSessions = entity.RemainingSessions,
                IsDeleted = entity.IsDeleted,
                SubscriptionCode = entity.Subscription.Code,
                MemberCardId = entity.Member.IdCardNumber,
                FirstName = entity.Member.FirstName
            };
        }
    }
}
