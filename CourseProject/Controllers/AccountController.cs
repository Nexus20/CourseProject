using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CourseProject.Data;
using CourseProject.Models;
using CourseProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Controllers {
    public class AccountController : Controller {

        private readonly CarContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(CarContext context, UserManager<User> userManager, SignInManager<User> signInManager) {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (ModelState.IsValid) {
                User user = new User {UserName = model.UserName, Email = model.Email};
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded) {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    await _userManager.AddToRoleAsync(user, "user");
                    return RedirectToAction("Index", "Home");
                }
                else {
                    foreach (var error in result.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = null) {
            return View(new LoginViewModel {ReturnUrl = returnUrl});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (ModelState.IsValid) {
                var result =
                    await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded) {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) {
                        return Redirect(model.ReturnUrl);
                    }
                    else {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else {
                    ModelState.AddModelError("", "Incorrect login or password");
                }
            }

            return View(model);
        }

        [Authorize(Roles = "admin, manager, user")]
        public async Task<IActionResult> Cabinet() {

            var user = await _userManager.GetUserAsync(this.User);
            user.FeaturedCars = await _context.FeaturedCars
                .Include(fc => fc.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(cm => cm.Brand)
                .Include(fc => fc.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(cm => cm.Parent)
                .Include(fc => fc.Car)
                    .ThenInclude(c => c.FuelType)
                .Include(fc => fc.Car)
                    .ThenInclude(c => c.BodyType)
                .Include(fc => fc.Car)
                    .ThenInclude(c => c.TransmissionType)
                .Include(fc => fc.Car)
                    .ThenInclude(c => c.CarImages)
                .Where(fc => fc.UserId == user.Id).ToListAsync();

            user.PurchaseRequests = await _context.PurchaseRequests
                .Include(pr => pr.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(cm => cm.Brand)
                .Include(pr => pr.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(cm => cm.Parent)
                .Where(pr => pr.ClientId == user.Id)
                .ToListAsync();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
