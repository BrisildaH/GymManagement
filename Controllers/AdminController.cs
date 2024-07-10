using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Gym.Models;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Gym.Service;

public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [Authorize]
    public async Task<IActionResult> Index(string filterTerm)
    {
        var usersQuery = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(filterTerm))
        {
            usersQuery = usersQuery.Where(u => u.Email.Contains(filterTerm));
            ViewBag.FilterTerm = filterTerm;
        }

        var usersWithRoles = await usersQuery
            .Select(u => new UserViewModel
            {
                ID = u.Id,
                Email = u.Email,
                Role = ""
            })
            .ToListAsync();

        foreach (var user in usersWithRoles)
        {
            var identityUser = await _userManager.FindByIdAsync(user.ID);
            var roles = await _userManager.GetRolesAsync(identityUser);
            user.Role = roles.FirstOrDefault();
        }

        return View(usersWithRoles);
    }

    [Authorize]
    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);

        var userViewModel = new UserViewModel
        {
            ID = user.Id,
            Email = user.Email,
            Role = roles.FirstOrDefault()
        };

        return View(userViewModel);
    }

    [Authorize]
    public IActionResult Create()
    {
        ViewData["Roles"] = GetRolesSelectList();
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("", "Email address is required.");
                return View(model);
            }

            var newUser = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.Role))
                {
                    var roleExists = await _roleManager.RoleExistsAsync(model.Role);
                    if (roleExists)
                    {
                        await _userManager.AddToRoleAsync(newUser, model.Role);
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Role '{model.Role}' does not exist.");
                        ViewBag.Roles = GetRolesSelectList();
                        return View(model);
                    }
                }

                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }

        ViewBag.Roles = GetRolesSelectList();
        return View(model);
    }

    private SelectList GetRolesSelectList()
    {
        var roles = _roleManager.Roles.Select(r => r.Name).ToList();
        return new SelectList(roles);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        var userToDelete = await _userManager.FindByIdAsync(id);

        if (userToDelete == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);

        if (userToDelete.Email == currentUser.Email)
        {
            TempData["ErrorMessage"] = "You cannot delete yourself.";
            return RedirectToAction("Index");
        }

        var result = await _userManager.DeleteAsync(userToDelete);

        if (result.Succeeded)
        {
            return RedirectToAction("Index");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return RedirectToAction("Index");
        }
    }
}
