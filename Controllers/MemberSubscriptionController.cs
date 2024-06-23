using Gym.Interface;
using Gym.Models;
using Gym.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym.Controllers
{
    public class MemberSubscriptionController : Controller
    {
        private readonly IMemberSubscriptionService memberSubscriptionService;

        private readonly IMemberService memberService;
        private readonly ISubscriptionService subscriptionService;
        public MemberSubscriptionController(IMemberSubscriptionService memberSubscriptionService, IMemberService memberService, ISubscriptionService subscriptionService)
        {
            this.memberSubscriptionService = memberSubscriptionService;
            this.subscriptionService = subscriptionService;
            this.memberService = memberService;

        }

        [Authorize]
        public IActionResult Index(string keyword)
        {
            var currentDate = DateTime.Now;

            List<MemberSubscriptionModel> memberSubscriptions;

            if (string.IsNullOrEmpty(keyword))
            {
                memberSubscriptions = memberSubscriptionService.GetAllMemberSubscriptions()
                    .Where(s => s.EndDate >= currentDate)
                    .ToList();
            }
            else
            {
                memberSubscriptions = memberSubscriptionService.SearchMemberSubscriptions(keyword);
            }

            return View(memberSubscriptions);
        }

        [Authorize]
		public IActionResult Details(int id)
        {
            var memberSubscription = memberSubscriptionService.GetAllMemberSubscriptions()
                .FirstOrDefault(p => p.ID == id);
            return View(memberSubscription);
        }

        [HttpPost]
        public IActionResult Check(int id)
        {
            try
            {
                var success = memberSubscriptionService.CheckIn(id);

                if (!success)
                {
                    TempData["ErrorMessage"] = "No sessions left to check in or subscription not found.";
                }
            }
            catch (ApplicationException ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
             
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
