using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProject.Data;
using CourseProject.Models;
using CourseProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Areas.Admin.Controllers {

    [Authorize(Roles = "admin, manager")]
    [Area("Admin")]
    public class PurchaseRequestsController : Controller {

        private readonly CarContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PurchaseRequestsController(CarContext context, IWebHostEnvironment appEnvironment, UserManager<User> userManager, RoleManager<IdentityRole> roleManager) {
            _context = context;
            _appEnvironment = appEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(PurchaseRequestSearchViewModel purchaseRequestSearch, string sortOrder) {

            ViewBag.IdSort = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSort = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.AppDateSort = sortOrder == "app_date_asc" ? "app_date_desc" : "app_date_asc";

            var requests = _context.PurchaseRequests
                .Include(pr => pr.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(cm => cm.Brand)
                .Include(pr => pr.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(cm => cm.Parent)
                .Include(pr => pr.Manager)
                .AsNoTracking();


            if (purchaseRequestSearch.Id != null) {
                requests = requests.Where(pr => pr.Id == purchaseRequestSearch.Id);
            }
            ViewBag.RequestId = purchaseRequestSearch.Id;

            if (purchaseRequestSearch.CarAvailable != null && purchaseRequestSearch.CarAvailable.Value != 0) {
                requests = requests.Where(pr => pr.CarAvailability == (purchaseRequestSearch.CarAvailable == 1));
            }
            ViewBag.CarAvailable = purchaseRequestSearch.CarAvailable;

            if (!string.IsNullOrEmpty(purchaseRequestSearch.Owner)) {
                requests = requests.Where(pr => pr.ManagerId == purchaseRequestSearch.Owner);
            }

            if (!string.IsNullOrEmpty(purchaseRequestSearch.Surname)) {
                requests = requests.Where(pr => pr.Surname.Contains(purchaseRequestSearch.Surname));
            }
            ViewBag.Surname = purchaseRequestSearch.Surname;

            if (!string.IsNullOrEmpty(purchaseRequestSearch.Email)) {
                requests = requests.Where(pr => pr.Email == purchaseRequestSearch.Email);
            }
            ViewBag.Email = purchaseRequestSearch.Email;

            if (purchaseRequestSearch.RequestStates != null && purchaseRequestSearch.RequestStates.Length > 0) {

                IQueryable<PurchaseRequest> requests2 = requests.Where(pr => pr.State == (PurchaseRequest.RequestState)purchaseRequestSearch.RequestStates[0]);
                for (var i = 1; i < purchaseRequestSearch.RequestStates.Length; i++) {
                    var i1 = i;
                    requests2 = requests2.Concat(requests.Where(pr => pr.State == (PurchaseRequest.RequestState)purchaseRequestSearch.RequestStates[i1]));
                }

                requests = requests2;

                ViewBag.CheckedRequestStates = Request.Query["requestStates"].ToArray();
            }
            else {
                ViewBag.CheckedRequestStates = null;
            }

            requests = sortOrder switch {
                "id_desc" => requests.OrderByDescending(r => r.Id),
                "name_asc" => requests.OrderBy(r => r.Surname),
                "name_desc" => requests.OrderByDescending(r => r.Surname),
                "app_date_asc" => requests.OrderBy(r => r.ApplicationDate),
                "app_date_desc" => requests.OrderByDescending(r => r.ApplicationDate),
                _ => requests.OrderBy(r => r.Id)
            };

            ViewBag.QueryString = purchaseRequestSearch.CreateRequest();

            var managerId = _userManager.GetUserId(User);

            ViewBag.RequestStates = Enum.GetValues(typeof(PurchaseRequest.RequestState)).Cast<int>().ToDictionary(key => key, key => Enum.GetName(typeof(PurchaseRequest.RequestState), key));

            if (User.IsInRole("admin")) {
                var allManagers = await _userManager.GetUsersInRoleAsync("manager");
                allManagers.Remove(await _userManager.GetUserAsync(User));
                ViewBag.ManagersList = allManagers;
            }
            else {
                ViewBag.ManagersList = null;
            }

            ViewBag.ManagerId = managerId;

            return View(await requests.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id) {

            if (id == null) {
                return NotFound();
            }

            var request = await _context.PurchaseRequests
                .Include(pr => pr.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(cm => cm.Brand)
                .Include(pr => pr.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(cm => cm.Parent)
                .Include(pr => pr.Car)
                    .ThenInclude(c => c.BodyType)
                .Include(pr => pr.Car)
                    .ThenInclude(c => c.FuelType)
                .Include(pr => pr.Car)
                    .ThenInclude(c => c.TransmissionType)
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (request == null) {
                return NotFound();
            }

            ViewBag.PurchaseRequestStates = Enum.GetValues(typeof(PurchaseRequest.RequestState)).Cast<int>().ToDictionary(key => key, key => Enum.GetName(typeof(PurchaseRequest.RequestState), key));

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Details(int id) {
            var purchaseRequest = await _context.PurchaseRequests.FindAsync(id);

            purchaseRequest.State = PurchaseRequest.RequestState.Closed;
            _context.Update(purchaseRequest);

            var car = await _context.Cars.FindAsync(purchaseRequest.CarId);
            if (car.State == Car.CarState.SecondHand) {
                car.Presence = Car.CarPresence.Sold;
                _context.Update(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CancelRequest(int id) {
            var purchaseRequest = await _context.PurchaseRequests.FindAsync(id);

            purchaseRequest.State = PurchaseRequest.RequestState.Canceled;
            _context.Update(purchaseRequest);

            var anotherPurchaseRequest = await _context.PurchaseRequests
                .Where(pr => pr.Id != purchaseRequest.Id 
                             && pr.CarId == purchaseRequest.CarId && pr.CarAvailability == false
                             && (pr.State == PurchaseRequest.RequestState.New || pr.State == PurchaseRequest.RequestState.Processing))
                .OrderBy(pr => pr.ApplicationDate)
                .FirstOrDefaultAsync();

            if (anotherPurchaseRequest != null) {
                anotherPurchaseRequest.CarAvailability = true;
                _context.Update(anotherPurchaseRequest);
            }
            else {
                var car = await _context.Cars.FindAsync(purchaseRequest.CarId);
                car.Count++;
                if (car.Presence == Car.CarPresence.BookedOrSold) {
                    car.Presence = Car.CarPresence.InStock;
                }

                _context.Update(car);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AssignManagerToRequest(int? requestId, string managerId) {
            if (requestId == null) {
                return NotFound();
            }

            var request = _context.PurchaseRequests.FirstOrDefault(pr => pr.Id == requestId);

            if (request == null) {
                return NotFound();
            }

            if (User.IsInRole("admin")) {
                if (string.IsNullOrEmpty(managerId)) {
                    return NotFound();
                }
                request.ManagerId = managerId;
            }
            else {
                request.ManagerId = _userManager.GetUserId(User);
            }

            request.State = PurchaseRequest.RequestState.Processing;
            _context.Update(request);
            _context.SaveChanges();

            return Content("OK");
        }

        //public IActionResult ChangeRequestState(int? requestId, int? newState) {

        //    if (requestId == null || newState == null) {
        //        return NotFound();
        //    }

        //    var request = _context.PurchaseRequests.FirstOrDefault(pr => pr.Id == requestId);

        //    if (request == null) {
        //        return NotFound();
        //    }

        //    request.State = (PurchaseRequest.RequestState)newState.Value;
        //    _context.Update(request);
        //    _context.SaveChanges();

        //    return Content(Enum.GetName(typeof(PurchaseRequest.RequestState), (PurchaseRequest.RequestState)newState.Value));
        //}

    }
}
