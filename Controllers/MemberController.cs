using Gym.Interface;
using Gym.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberService memberService;

        public MemberController(IMemberService memberService)
        {
            this.memberService = memberService;

        }
        [Authorize]
        public ActionResult Index(string filterTerm)
        {
            List<MemberViewModel> members;

            try
            {
                if (!string.IsNullOrWhiteSpace(filterTerm))
                {
                    members = memberService.SearchMembers(filterTerm);
                }
                else
                {
                    members = memberService.GetAllMembers();
                }

                ViewBag.FilterTerm = filterTerm;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error occurred while searching members: " + ex.Message;
                members = new List<MemberViewModel>();
            }

            return View(members);
        }


        [Authorize]
        public IActionResult Details(int id)
        {
            var member = memberService.GetAllMembers()
                                        .FirstOrDefault(p => p.ID == id);
            return View(member);
        }
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]

        [HttpPost]
        public ActionResult Create(MemberViewModel memberViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = memberService.CreateMember(memberViewModel);
                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(memberViewModel);
                }
                return RedirectToAction("Index");
            }
            return View(memberViewModel);
        }

        [Authorize]
        public ActionResult Update(int id)
        {
            var member = memberService.GetMemberByID(id);
            if (member == null)
            {
                ViewBag.ErrorMessage = "Member not found";
                return RedirectToAction("Index");
            }
            return View(member);
        }

        [Authorize]

        [HttpPost]
        public ActionResult Update(MemberViewModel memberViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = memberService.UpdateMember(memberViewModel);
                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(memberViewModel);
                }
                return RedirectToAction("Index");
            }
            return View(memberViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var result = memberService.SoftDelete(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                if (result.Message.Contains("Active subscription found"))
                {
                    TempData["ErrorMessage"] = "Cannot delete member. Active subscription found.";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }

            return RedirectToAction("Index");
        }
    }
}
