using Gym.Data.Entities;
using Gym.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym.Interface
{
    public interface IMemberSubscriptionService
    {
        public List<MemberSubscriptionModel> GetAllMemberSubscriptions();
        public bool MemberSubscriptionExist(MemberSubscriptionModel memberSubscriptionVM);

        public ServiceResult ActivateSubscription(MemberSubscriptionModel vm);
        //public MemberSubscriptionModel GetMemberSubscriptionByDetail(int id, string memberCardID, string subscriptionCode);
        public List<SelectListItem> GetSubscriberCode();
        public List<SelectListItem> GetMembersCardID();
        public bool CheckIn(int subscriptionId);
        public List<MemberSubscriptionModel> SearchMemberSubscriptions(string keyword);
    }
}
