using System.Collections.Generic;
using Gym.Data.Entities;
using Gym.Models;

namespace Gym.Services
{
    public interface ISubscriptionService
    {
        SubscriptionViewModel GetById(int id);
        SubscriptionViewModel GetByCode(string code);
        List<SubscriptionViewModel> GetAllSubscriptions();
        public ServiceResult CreateSubscription(SubscriptionViewModel vm);
       ServiceResult UpdateSubscription(SubscriptionViewModel vm);
        public ServiceResult DeleteSubscription(int id);
        public List<SubscriptionViewModel> SearchSubscriptions(string keyword);
    }
}

