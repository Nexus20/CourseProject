using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProject.Data;
using CourseProject.Models;
using CourseProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Areas.Admin.Controllers
{

    [Authorize(Roles = "admin, manager")]
    [Area("Admin")]
    public class PurchaseRequestsController : Controller
    {

        private readonly CarContext _context;
        private readonly UserManager<User> _userManager;

        public PurchaseRequestsController(CarContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(PurchaseRequestSearchViewModel purchaseRequestSearch, string sortOrder, int? newSearch, int? page)
        {

            ViewBag.IdSort = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSort = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.AppDateSort = sortOrder == "app_date_asc" ? "app_date_desc" : "app_date_asc";
            ViewBag.CurrentSort = sortOrder;
            var managerId = _userManager.GetUserId(User);

            var requests = _context.PurchaseRequests
                .Include(pr => pr.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(cm => cm.Brand)
                .Include(pr => pr.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(cm => cm.Parent)
                .Include(pr => pr.Manager)
                .Where(pr => (User.IsInRole("admin") || pr.ManagerId == managerId || pr.State == PurchaseRequest.RequestState.New))
                .AsNoTracking();

            if (newSearch != null)
            {
                page = 1;
            }

            if (purchaseRequestSearch.Id != null)
            {
                requests = requests.Where(pr => pr.Id == purchaseRequestSearch.Id);
            }
            ViewBag.RequestId = purchaseRequestSearch.Id;

            if (purchaseRequestSearch.CarAvailable != null && purchaseRequestSearch.CarAvailable.Value != 0)
            {
                requests = requests.Where(pr => pr.CarAvailability == (purchaseRequestSearch.CarAvailable == 1));
                ViewBag.CarAvailable = purchaseRequestSearch.CarAvailable;
            }
            else
            {
                ViewBag.CarAvailable = 0;
            }


            if (!string.IsNullOrEmpty(purchaseRequestSearch.Owner))
            {
                requests = requests.Where(pr => pr.ManagerId == purchaseRequestSearch.Owner);
            }

            ViewBag.Owner = purchaseRequestSearch.Owner;

            if (!string.IsNullOrEmpty(purchaseRequestSearch.Surname))
            {
                requests = requests.Where(pr => pr.Surname.Contains(purchaseRequestSearch.Surname));
            }
            ViewBag.Surname = purchaseRequestSearch.Surname;

            if (!string.IsNullOrEmpty(purchaseRequestSearch.Email))
            {
                requests = requests.Where(pr => pr.Email == purchaseRequestSearch.Email);
            }
            ViewBag.Email = purchaseRequestSearch.Email;

            if (purchaseRequestSearch.RequestStates != null && purchaseRequestSearch.RequestStates.Length > 0)
            {

                IQueryable<PurchaseRequest> requests2 = requests.Where(pr => pr.State == (PurchaseRequest.RequestState)purchaseRequestSearch.RequestStates[0]);
                for (var i = 1; i < purchaseRequestSearch.RequestStates.Length; i++)
                {
                    var i1 = i;
                    requests2 = requests2.Concat(requests.Where(pr => pr.State == (PurchaseRequest.RequestState)purchaseRequestSearch.RequestStates[i1]));
                }

                requests = requests2;

                ViewBag.CheckedRequestStates = Request.Query["requestStates"].ToArray();
            }
            else
            {
                ViewBag.CheckedRequestStates = null;
            }

            requests = sortOrder switch
            {
                "id_desc" => requests.OrderByDescending(r => r.Id),
                "name_asc" => requests.OrderBy(r => r.Surname),
                "name_desc" => requests.OrderByDescending(r => r.Surname),
                "app_date_asc" => requests.OrderBy(r => r.ApplicationDate),
                "app_date_desc" => requests.OrderByDescending(r => r.ApplicationDate),
                _ => requests.OrderBy(r => r.Id)
            };

            ViewBag.QueryString = purchaseRequestSearch.CreateRequest();

            ViewBag.RequestStates = Enum.GetValues(typeof(PurchaseRequest.RequestState)).Cast<int>().ToDictionary(key => key, key => Enum.GetName(typeof(PurchaseRequest.RequestState), key));

            var allManagers = new List<User>();

            if (User.IsInRole("admin"))
            {
                allManagers.AddRange(await _userManager.GetUsersInRoleAsync("manager"));
            }
            else
            {
                allManagers.Add(await _userManager.GetUserAsync(User));
            }

            ViewBag.ManagersList = allManagers;

            ViewBag.ManagerId = managerId;

            const int pageSize = 5;

            return View(await PaginatedList<PurchaseRequest>.CreateAsync(requests, page ?? 1, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
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
                .Include(pr => pr.Car)
                    .ThenInclude(c => c.CarImages)
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            ViewBag.PurchaseRequestStates = Enum.GetValues(typeof(PurchaseRequest.RequestState)).Cast<int>().ToDictionary(key => key, key => Enum.GetName(typeof(PurchaseRequest.RequestState), key));

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Details(int id)
        {
            var purchaseRequest = await _context.PurchaseRequests.FindAsync(id);

            purchaseRequest.State = PurchaseRequest.RequestState.Closed;
            _context.Update(purchaseRequest);

            var car = await _context.Cars.FindAsync(purchaseRequest.CarId);
            if (car.State == Car.CarState.SecondHand)
            {
                car.Presence = Car.CarPresence.Sold;
                _context.Update(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var purchaseRequest = await _context.PurchaseRequests.FindAsync(id);

            purchaseRequest.State = PurchaseRequest.RequestState.Canceled;
            _context.Update(purchaseRequest);

            var anotherPurchaseRequest = await _context.PurchaseRequests
                .Where(pr => pr.Id != purchaseRequest.Id
                             && pr.CarId == purchaseRequest.CarId && pr.CarAvailability == false
                             && (pr.State == PurchaseRequest.RequestState.New || pr.State == PurchaseRequest.RequestState.Processing))
                .OrderBy(pr => pr.ApplicationDate)
                .FirstOrDefaultAsync();

            if (anotherPurchaseRequest != null)
            {
                anotherPurchaseRequest.CarAvailability = true;
                _context.Update(anotherPurchaseRequest);
            }
            else
            {
                var car = await _context.Cars.FindAsync(purchaseRequest.CarId);
                car.Count++;
                if (car.Presence == Car.CarPresence.BookedOrSold)
                {
                    car.Presence = Car.CarPresence.InStock;
                }

                _context.Update(car);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AssignManagerToRequest(int? requestId, string managerId)
        {
            if (requestId == null)
            {
                return NotFound();
            }

            var request = _context.PurchaseRequests.FirstOrDefault(pr => pr.Id == requestId);

            if (request == null)
            {
                return NotFound();
            }

            request.ManagerId = string.IsNullOrEmpty(managerId) ? _userManager.GetUserId(User) : managerId;

            request.State = PurchaseRequest.RequestState.Processing;
            _context.Update(request);
            _context.SaveChanges();

            return Content(_userManager.Users.First(u => u.Id == request.ManagerId).UserName);
        }

    }
}
