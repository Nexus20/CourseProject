using System;
using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CourseProject.Data;
using CourseProject.Extensions;
using CourseProject.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace CourseProject.Controllers {
    public class HomeController : Controller {

        private readonly IWebHostEnvironment _appEnvironment;
        private readonly ILogger<HomeController> _logger;
        private readonly CarContext _context;
        private readonly UserManager<User> _userManager;

        private static readonly Dictionary<int, Car> _carsToCompare = new();

        public HomeController(CarContext context, IWebHostEnvironment appEnvironment, UserManager<User> userManager, ILogger<HomeController> logger) {
            _context = context;
            _appEnvironment = appEnvironment;
            _logger = logger;
            _userManager = userManager;
            //DbInitializer.Initialize2(_context);
        }

        [HttpGet]
        public async Task<IActionResult> Index(CarSearchViewModel carSearch, int? newSearch, int? page) {

            //StringBuilder queryStringBuilder = new StringBuilder("?");

            IQueryable<Car> cars = _context.Cars
                .Include(c => c.Model)
                    .ThenInclude(cm => cm.Brand)
                .Include(c => c.Model)
                    .ThenInclude(cm => cm.Parent)
                .Include(c => c.Model)
                .Include(c => c.BodyType)
                .Include(c => c.FuelType)
                .Include(c => c.TransmissionType)
                .Include(c => c.CarImages)
                .Where(c => c.Presence != Models.Car.CarPresence.Sold)
                .AsNoTracking();

            if (newSearch != null) {
                page = 1;
            }

            if (carSearch.BrandId != null) {
                cars = cars.Where(c => c.Model.BrandId == carSearch.BrandId);
            }

            if (carSearch.ModelId != null) {
                cars = cars.Where(c => c.ModelId == carSearch.ModelId);
            }

            if (carSearch.FuelTypes != null && carSearch.FuelTypes.Length > 0) {
                IQueryable<Car> cars2 = cars.Where(c => c.FuelTypeId == carSearch.FuelTypes[0]);
                for (var i = 1; i < carSearch.FuelTypes.Length; i++) {
                    var i1 = i;
                    cars2 = cars2.Concat(cars.Where(c => c.FuelTypeId == carSearch.FuelTypes[i1]));
                }

                cars = cars2;

                ViewBag.CheckedFuelTypes = Request.Query["fuelTypes"].ToArray();
            }
            else {
                ViewBag.CheckedFuelTypes = null;
            }

            if (carSearch.BodyTypes != null && carSearch.BodyTypes.Length > 0) {

                IQueryable<Car> cars2 = cars.Where(c => c.BodyTypeId == carSearch.BodyTypes[0]);
                for (var i = 1; i < carSearch.BodyTypes.Length; i++) {
                    var i1 = i;
                    cars2 = cars2.Concat(cars.Where(c => c.BodyTypeId == carSearch.BodyTypes[i1]));
                }

                cars = cars2;

                ViewBag.CheckedBodyTypes = Request.Query["bodyTypes"].ToArray();
            }
            else {
                ViewBag.CheckedBodyTypes = null;
            }

            if (carSearch.TransmissionTypes != null && carSearch.TransmissionTypes.Length > 0) {

                IQueryable<Car> cars2 = cars.Where(c => c.TransmissionTypeId == carSearch.TransmissionTypes[0]);
                for (var i = 1; i < carSearch.TransmissionTypes.Length; i++) {
                    var i1 = i;
                    cars2 = cars2.Concat(cars.Where(c => c.TransmissionTypeId == carSearch.TransmissionTypes[i1]));
                }

                cars = cars2;

                ViewBag.CheckedTransmissionTypes = Request.Query["transmissionTypes"].ToArray();
            }
            else {
                ViewBag.CheckedTransmissionTypes = null;
            }

            if (carSearch.CarStates != null && carSearch.CarStates.Length > 0) {

                IQueryable<Car> cars2 = cars.Where(c => c.State == (Car.CarState)carSearch.CarStates[0]);
                for (var i = 1; i < carSearch.CarStates.Length; i++) {
                    var i1 = i;
                    cars2 = cars2.Concat(cars.Where(c => c.State == (Car.CarState)carSearch.CarStates[i1]));
                }

                cars = cars2;

                ViewBag.CheckedCarStates = Request.Query["carStates"].ToArray();
            }
            else {
                ViewBag.CheckedTransmissionTypes = null;
            }

            if ((carSearch.PriceFrom != null && carSearch.PriceTo != null)
                && (carSearch.PriceFrom.Value <= carSearch.PriceTo.Value)
                && (carSearch.PriceFrom.Value >= 0 && carSearch.PriceTo.Value >= 0)) {

                cars = cars.Where(c => c.Price >= carSearch.PriceFrom && c.Price <= carSearch.PriceTo);
            }

            //ViewBag.QueryString = queryStringBuilder.ToString();
            ViewBag.CarSearchModel = carSearch;
            ViewBag.QueryString = carSearch.CreateRequest();
            ViewBag.TransmissionTypes = _context.TransmissionTypes;
            ViewBag.BodyTypes = _context.BodyTypes;
            ViewBag.FuelTypes = _context.FuelTypes;
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name", carSearch.BrandId);

            ViewBag.CarStates = Enum.GetValues(typeof(Car.CarState)).Cast<Car.CarState>()
                .ToDictionary(t => (int) t, t => t.ToString());

            ViewBag.CarModels = carSearch.BrandId != null ? new SelectList(_context.CarModels.Where(cm => cm.BrandId == carSearch.BrandId), "Id", "Name", carSearch.ModelId) : null;

            ViewBag.ComparedCars = _carsToCompare.Keys.ToList();

            if (User.Identity.IsAuthenticated) {
                var userId = _userManager.GetUserId(User);
                ViewBag.FeaturedCars = _context.FeaturedCars.Where(fc => fc.UserId == userId).Select(fc => fc.CarId).ToList();
            }

            int pageSize = 5;

            return View(await PaginatedList<Car>.CreateAsync(cars.AsNoTracking(), page ?? 1, pageSize));
        }

        public IActionResult GetModelsByBrand(int? brandId) {

            if (brandId == null) {
                return Content("<option value=\"\"></option>", "text/html");
            }

            var brand = _context.Brands.FirstOrDefault(b => b.Id == brandId);

            if (brand == null) {
                return Content("<option value=\"\"></option>", "text/html");
            }

            var sb = new StringBuilder("<option value=\"\"></option>");

            var carModels = _context.CarModels
                .Include(cm => cm.Parent)
                .Where(cm => cm.BrandId == brandId);

            foreach (var carModel in carModels) {
                sb.Append($"<option value=\"{carModel.Id}\">" + (carModel.Parent != null ? carModel.Parent.Name + " " : "") + $"{carModel.Name}</option>");
            }

            return Content(sb.ToString(), "text/html");

        }


        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: Admin/Cars/Car/5
        public async Task<IActionResult> Car(int? id) {
            if (id == null) {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.BodyType)
                .Include(c => c.FuelType)
                .Include(c => c.Model)
                    .ThenInclude(cm => cm.Brand)
                .Include(c => c.Model)
                    .ThenInclude(cm => cm.Parent)
                .Include(c => c.TransmissionType)
                .Include(c => c.CarImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null) {
                return NotFound();
            }

            ViewBag.ImagesDirectory = $"{_appEnvironment.WebRootPath}/img/cars/{car.Id}";

            return View(car);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePurchaseRequest(int? id) {
            if (id == null) {
                return NotFound();
            }

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id);

            if (car == null) {
                return NotFound();
            }

            return View(new PurchaseRequest() { CarId = id.Value });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseRequest([Bind("CarId,Firstname,Surname,Phone,Email")] PurchaseRequest request) {

            if (ModelState.IsValid) {

                request.ApplicationDate = DateTime.Now;
                if (User.Identity.IsAuthenticated) {
                    request.ClientId = _userManager.GetUserId(User);
                }

                _context.Add(request);
                //await _context.SaveChangesAsync();

                var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == request.CarId);
                if (car.Count > 0) {
                    request.CarAvailability = true;
                    car.Count--;
                }
                else {
                    request.CarAvailability = false;
                }

                if (car.Count <= 0) {
                    car.Count = 0;
                    car.Presence = Models.Car.CarPresence.BookedOrSold;
                    // TODO add state displaying for second hand cars
                }

                await _context.SaveChangesAsync();

                return User.Identity.IsAuthenticated ? RedirectToAction("Cabinet", "Account") : RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        

        public IActionResult AddRemoveToCompare(int? carId) {
            if (carId == null) {
                return NotFound();
            }

            var car = _context.Cars
                .Include(c => c.BodyType)
                .Include(c => c.FuelType)
                .Include(c => c.Model)
                .ThenInclude(cm => cm.Brand)
                .Include(c => c.Model)
                .ThenInclude(cm => cm.Parent)
                .Include(c => c.TransmissionType)
                .Include(c => c.CarImages)
                .FirstOrDefault(m => m.Id == carId);

            if (car == null) {
                return NotFound();
            }

            string res = "";
            if (_carsToCompare.ContainsKey(car.Id)) {
                _carsToCompare.Remove(car.Id);
                res = "removed";
            }
            else {
                _carsToCompare.Add(car.Id, car);
                res = "added";
            }
            HttpContext.Session.Set("CarsToCompare", _carsToCompare);

            return Content(res);
        }

        [HttpGet]
        public async Task<IActionResult> Compare() {

            var carsToCompare = HttpContext.Session.Get<Dictionary<int, Car>>("CarsToCompare");


            return View(carsToCompare);

        }

        [HttpGet]
        public IActionResult AddRemoveFeatured(int? carId) {

            if (carId == null) {
                return NotFound();
            }

            var car = _context.Cars.FirstOrDefault(c => c.Id == carId);

            if (car == null) {
                return NotFound();
            }

            var featuredCar = _context.FeaturedCars.FirstOrDefault(fc =>
                fc.CarId == carId && fc.UserId == _userManager.GetUserId(this.User));

            string res = "";

            if (featuredCar != null) {
                _context.FeaturedCars.Remove(featuredCar);
                res = "removed";
            }
            else {
                _context.FeaturedCars.Add(new FeaturedCar()
                    {CarId = carId.Value, UserId = _userManager.GetUserId(this.User)});
                res = "added";
            }
            _context.SaveChanges();

            return Content(res);
        }

    }
}
