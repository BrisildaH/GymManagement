using Gym.Interface;
using Gym.Models;
using Gym.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMemberService _memberService;
        private readonly IMemberSubscriptionService _memberSubscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService, IMemberService memberService, IMemberSubscriptionService memberSubscriptionService)
        {
            _subscriptionService = subscriptionService;
            _memberService = memberService;
            _memberSubscriptionService = memberSubscriptionService;
        }

        [Authorize]
        public ActionResult Index(string filterTerm)
        {
            List<SubscriptionViewModel> subscriptions;

            try
            {
                if (!string.IsNullOrWhiteSpace(filterTerm))
                {
                    subscriptions = _subscriptionService.SearchSubscriptions(filterTerm);
                }
                else
                {
                    subscriptions = _subscriptionService.GetAllSubscriptions();
                }

                ViewBag.FilterTerm = filterTerm;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error occurred while searching subscriptions: " + ex.Message;
                subscriptions = new List<SubscriptionViewModel>();
            }

            return View(subscriptions);
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var subscription = _subscriptionService.GetAllSubscriptions()
                                 .FirstOrDefault(p => p.ID == id);

            return View(subscription);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubscriptionViewModel subscriptionViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _subscriptionService.CreateSubscription(subscriptionViewModel);
                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(subscriptionViewModel);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subscriptionViewModel);
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            var subscription = _subscriptionService.GetById(id);
            if (subscription == null)
            {
                ViewBag.ErrorMessage = "Member not found";
                return RedirectToAction("Index");
            }
            return View(subscription);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SubscriptionViewModel subscriptionViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _subscriptionService.UpdateSubscription(subscriptionViewModel);
                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(subscriptionViewModel);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subscriptionViewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var result = _subscriptionService.DeleteSubscription(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                if (result.Message.Contains("Active subscription found"))
                {
                    TempData["ErrorMessage"] = "Cannot delete subscription. Active subscription found.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Activate(int id)
        {
            var subscription = _subscriptionService.GetById(id);
            var memberList = _memberService.GetAllMembers();
            var memberListItems = new List<SelectListItem>();
            foreach (var member in memberList)
            {
                var listItem = new SelectListItem();
                listItem.Value = member.ID.ToString();
                listItem.Text = member.FirstName + " " + member.LastName;
                memberListItems.Add(listItem);
            }

            ViewBag.MemberList = memberListItems;

            var model = new MemberSubscriptionCreateViewModel();
            model.SubscriptionViewModel = subscription;
            return View(model);
        }

        [HttpPost]
        public ActionResult ActivateSubscription(MemberSubscriptionCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                //convert vm into membersubscriptionmodel
                MemberSubscriptionModel subscriptionModel = new MemberSubscriptionModel
                {

                    MemberID = vm.MemberId,
                    SubscriptionID = vm.SubscriptionViewModel.ID,
                    SubscriptionCode = vm.SubscriptionViewModel.Code,

                    StartDate = vm.StartDate,
                    RemainingSessions = (int)vm.SubscriptionViewModel.TotalNumberOfSessions,
                    IsDeleted = false

                };
                //call activatesubscription from service

                var result = _memberSubscriptionService.ActivateSubscription(subscriptionModel);
                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Message;
                    //return View(subscriptionModel);
                }

                return RedirectToAction("Index");
            }
                return View(vm);
        }
    }
}
