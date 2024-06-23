using Gym.Data;
using Gym.Data.Entities;
using Gym.Interface;
using Gym.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gym.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<SubscriptionViewModel> GetAllSubscriptions()
        {
            var subscriptionVm = new List<SubscriptionViewModel>();
            var subscriptions = _context.Subscriptions.Where(s => s.IsDeleted==false).ToList();

            foreach (var subscription in subscriptions)
            {
                subscriptionVm.Add(EntityToViewModel(subscription));
            }
            return subscriptionVm;
        }

        public ServiceResult CreateSubscription(SubscriptionViewModel vm)
        {
            var result = new ServiceResult();

            try
            {
                var subscription = ViewModelToEntity(vm);
                var subscriptionExist = _context.Subscriptions.Any(s => s.Code == subscription.Code);

                if (subscriptionExist)
                {
                    result.Success = false;
                    result.Message = "The subscription with this Code already exists.";
                }
                else
                {
                    CalculateTotalNumberOfSessions(subscription);
                    subscription.IsDeleted = false;
                    _context.Add(subscription);
                    _context.SaveChanges();

                    result.Success = true;
                    result.Message = "Subscription created successfully.";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error creating subscription: " + ex.Message;
            }

            return result;
        }

        public ServiceResult UpdateSubscription(SubscriptionViewModel vm)
        {
            var result = new ServiceResult();

            try
            {
                var existingSubscription = _context.Subscriptions.FirstOrDefault(s => s.ID == vm.ID);

                if (existingSubscription == null)
                {
                    result.Success = false;
                    result.Message = "Subscription not found.";
                }
                else
                {
                    var subscriptionWithSameCode = _context.Subscriptions.FirstOrDefault(s => s.Code == vm.Code && s.ID != vm.ID);

                    if (subscriptionWithSameCode != null)
                    {
                        result.Success = false;
                        result.Message = "The subscription with this Code already exists.";
                    }
                    else
                    {
                        existingSubscription.Code = vm.Code;
                        existingSubscription.Description = vm.Description;
                        existingSubscription.WeekFrequency = vm.WeekFrequency;
                        existingSubscription.NumberOfMonths = vm.NumberOfMonths;
                        existingSubscription.TotalPrice = vm.TotalPrice;
                        CalculateTotalNumberOfSessions(existingSubscription);

                        _context.SaveChanges();
                        result.Success = true;
                        result.Message = "Subscription updated successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error updating subscription: " + ex.Message;
            }

            return result;
        }

        public ServiceResult DeleteSubscription(int id)
        {
            var result = new ServiceResult();

            try
            {
                var subscriptionExist = _context.Subscriptions.FirstOrDefault(s => s.ID == id);

                if (subscriptionExist != null)
                {
                    var activeSubscription = _context.MemberSubscriptions.Any(ms => ms.SubscriptionID == subscriptionExist.ID && ms.EndDate >= DateTime.Today);
                    if (activeSubscription)
                    {
                        return new ServiceResult { Success = false, Message = "Cannot delete subscription. Active subscription found." };
                    }
                    
                        subscriptionExist.IsDeleted = true;
                        _context.SaveChanges();

                    return new ServiceResult { Success = true, Message = "Subscription deleted successfully." };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Subscription not found." };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = "Error deleting subscription: " + ex.Message };
            }
        }

        public SubscriptionViewModel GetById(int id)
        {
            try
            {
                var gymSubscription = _context.Subscriptions.FirstOrDefault(m => m.ID == id);

                if (gymSubscription == null)
                {
                    throw new Exception("Subscription not found");
                }

                return EntityToViewModel(gymSubscription);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Subscription by ID : " + id, ex);
            }
        }

        public SubscriptionViewModel GetByCode(string code)
        {
            try
            {
                if (code == null)
                {
                    throw new Exception("Code cannot be null");
                }

                var gymSubscription = _context.Subscriptions.FirstOrDefault(s => s.Code == code);

                if (gymSubscription == null)
                {
                    throw new Exception("Subscription not found");
                }

                return EntityToViewModel(gymSubscription);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Subscription by Code : " + code, ex);
            }
        }

        public List<SubscriptionViewModel> SearchSubscriptions(string keyword)
        {
            try
            {
                var subscriptions = _context.Subscriptions.Where(s => s.IsDeleted == false).ToList();

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return subscriptions.Select(EntityToViewModel).ToList();
                }

                var filteredSubscriptions = subscriptions.Where(s =>
                    s.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    s.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    s.NumberOfMonths.ToString().Contains(keyword) ||
                    s.TotalNumberOfSessions.ToString().Contains(keyword) ||
                    s.TotalPrice.ToString().Contains(keyword) ||
                    s.WeekFrequency.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                return filteredSubscriptions.Select(EntityToViewModel).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching subscriptions: " + ex.Message);
            }
        }

        private void CalculateTotalNumberOfSessions(Subscription subscription)
        {
            if (subscription.WeekFrequency == "Everyday")
            {
                subscription.TotalNumberOfSessions = subscription.NumberOfMonths * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            }
            else
            {
                int frequency = int.Parse(subscription.WeekFrequency);
                subscription.TotalNumberOfSessions = subscription.NumberOfMonths * frequency * 4;
            }
        }

        private Subscription ViewModelToEntity(SubscriptionViewModel vm)
        {
            return new Subscription
            {
                ID = vm.ID,
                Code = vm.Code,
                Description = vm.Description,
                NumberOfMonths = vm.NumberOfMonths,
                WeekFrequency = vm.WeekFrequency,
                TotalNumberOfSessions = vm.TotalNumberOfSessions,
                TotalPrice = vm.TotalPrice,
                IsDeleted = vm.IsDeleted
            };
        }

        private SubscriptionViewModel EntityToViewModel(Subscription entity)
        {
            return new SubscriptionViewModel
            {
                ID = entity.ID,
                Code = entity.Code,
                Description = entity.Description,
                NumberOfMonths = entity.NumberOfMonths,
                WeekFrequency = entity.WeekFrequency,
                TotalNumberOfSessions = entity.TotalNumberOfSessions,
                TotalPrice = (decimal)entity.TotalPrice,
                IsDeleted = entity.IsDeleted
            };
        }
    }
}
