using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseProject.Data;
using CourseProject.Models;
using CourseProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CourseProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin, manager")]
    [Area("Admin")]
    public class CarsController : Controller
    {
        private readonly CarContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public CarsController(CarContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Admin/Cars
        public async Task<IActionResult> Index(CarSearchViewModel carSearch, int? newSearch, int? page)
        {
            var cars = _context.Cars
                .Include(c => c.BodyType)
                .Include(c => c.FuelType)
                .Include(c => c.Model)
                    .ThenInclude(cm => cm.Brand)
                .Include(c => c.Model)
                    .ThenInclude(cm => cm.Parent)
                .Include(c => c.TransmissionType)
                .Include(c => c.CarImages)
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
                .ToDictionary(t => (int)t, t => t.ToString());

            ViewBag.CarModels = carSearch.BrandId != null ? new SelectList(_context.CarModels.Where(cm => cm.BrandId == carSearch.BrandId), "Id", "Name", carSearch.ModelId) : null;


            int pageSize = 5;

            return View(await PaginatedList<Car>.CreateAsync(cars.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: Admin/Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
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

            if (car == null)
            {
                return NotFound();
            }

            //ViewBag.ImagesDirectory = $"{_appEnvironment.WebRootPath}/img/cars/{car.Id}";

            return View(car);
        }

        // GET: Admin/Cars/Create
        public IActionResult Create() {
            ViewData["State"] = new SelectList(Enum.GetNames(typeof(Car.CarState)));
            ViewData["BodyTypeId"] = new SelectList(_context.BodyTypes, "Id", "Name");
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes, "Id", "Name");

            
            //ViewData["ModelId"] = new SelectList(_context.CarModels, "Id", "Name");
            ViewData["ModelId"] = new SelectList(CreateViewModel(), "ModelId", "ModelNameWithBrand");
            ViewData["TransmissionTypeId"] = new SelectList(_context.TransmissionTypes, "Id", "Name");
            return View();
        }

        // POST: Admin/Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Year,Color,Count,Price,State,ModelId,EngineVolume,Mileage,FuelTypeId,BodyTypeId,TransmissionTypeId")] Car car, IFormFileCollection uploadedImages)
        {
            if (ModelState.IsValid) {
                _context.Add(car);
                await _context.SaveChangesAsync();

                var directoryPath = _appEnvironment.WebRootPath + $"/img/cars/{car.Id}";
                if (!Directory.Exists(directoryPath)) {
                    DirectoryInfo dirInfo = new(directoryPath);
                    dirInfo.Create();
                }

                foreach (var uploadedImage in uploadedImages) {
                    var path = $"/img/cars/{car.Id}/{uploadedImage.FileName}";

                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create)) {
                        await uploadedImage.CopyToAsync(fileStream);
                    }

                    _context.CarImages.Add(new CarImage() {CarId = car.Id, Path = path});
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["State"] = new SelectList(Enum.GetNames(typeof(Car.CarState)), car.State);
            ViewData["BodyTypeId"] = new SelectList(_context.BodyTypes, "Id", "Name", car.BodyTypeId);
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes, "Id", "Name", car.FuelTypeId);

            //ViewData["ModelId"] = new SelectList(_context.CarModels, "Id", "Name", car.ModelId);
            ViewData["ModelId"] = new SelectList(CreateViewModel(), "ModelId", "ModelNameWithBrand", car.ModelId);

            ViewData["TransmissionTypeId"] = new SelectList(_context.TransmissionTypes, "Id", "Name", car.TransmissionTypeId);
            return View(car);
        }

        private IEnumerable<CarBrandModel> CreateViewModel() {

            var carModels = _context.CarModels
                .Include(carModel => carModel.Brand)
                .Include(carModel => carModel.Parent)
                .AsNoTracking();

            var viewModel = new List<CarBrandModel>();
            foreach (var carModel in carModels) {
                viewModel.Add(new CarBrandModel() {
                    ModelId = carModel.Id,
                    ModelName = (carModel.Parent != null ? $"{carModel.Parent.Name} " : "") + carModel.Name,
                    BrandName = carModel.Brand.Name
                });
            }

            return viewModel;
        }

        // GET: Admin/Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }


            ViewData["State"] = new SelectList(Enum.GetNames(typeof(Car.CarState)), car.State);
            ViewData["BodyTypeId"] = new SelectList(_context.BodyTypes, "Id", "Name", car.BodyTypeId);
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes, "Id", "Name", car.FuelTypeId);

            ViewData["ModelId"] = new SelectList(CreateViewModel(), "ModelId", "ModelNameWithBrand", car.ModelId);
            ViewData["TransmissionTypeId"] = new SelectList(_context.TransmissionTypes, "Id", "Name", car.TransmissionTypeId);

            ViewBag.ImagesDirectory = $"{_appEnvironment.WebRootPath}/img/cars/{car.Id}";

            return View(car);
        }

        // POST: Admin/Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Year,Price,State,ModelId,EngineVolume,Mileage,FuelTypeId,BodyTypeId,TransmissionTypeId,ImagesDirectoryPath")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["State"] = new SelectList(Enum.GetNames(typeof(Car.CarState)), car.State);
            ViewData["BodyTypeId"] = new SelectList(_context.BodyTypes, "Id", "Name", car.BodyTypeId);
            ViewData["FuelTypeId"] = new SelectList(_context.FuelTypes, "Id", "Name", car.FuelTypeId);

            ViewData["ModelId"] = new SelectList(CreateViewModel(), "ModelId", "ModelNameWithBrand", car.ModelId);
            ViewData["TransmissionTypeId"] = new SelectList(_context.TransmissionTypes, "Id", "Name", car.TransmissionTypeId);
            return View(car);
        }

        // GET: Admin/Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
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
            if (car == null)
            {
                return NotFound();
            }
            ViewBag.ImagesDirectory = $"{_appEnvironment.WebRootPath}/img/cars/{car.Id}";
            return View(car);
        }

        // POST: Admin/Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            var imagesDirectory = $"{_appEnvironment.WebRootPath}/img/cars/{id}";

            if (Directory.Exists(imagesDirectory)) {
                DirectoryInfo dirInfo = new(imagesDirectory);
                dirInfo.Delete(true);

                var carImages = await _context.CarImages.Where(ci => ci.CarId == id).ToListAsync();

                if (carImages.Count > 0) {
                    _context.RemoveRange(carImages);
                }

            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> AddImages(int carId, IFormFileCollection uploadedImages) {

            var car = _context.Cars.FirstOrDefaultAsync(c => c.Id == carId);

            if (car == null) {
                return NotFound();
            }

            var directoryPath = _appEnvironment.WebRootPath + $"/img/cars/{carId}";
            if (!Directory.Exists(directoryPath)) {
                DirectoryInfo dirInfo = new(directoryPath);
                dirInfo.Create();
            }

            foreach (var uploadedImage in uploadedImages) {
                var path = $"/img/cars/{carId}/{uploadedImage.FileName}";
                
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create)) {
                    await uploadedImage.CopyToAsync(fileStream);

                    _context.CarImages.Add(new CarImage() {CarId = carId, Path = path});
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveImages(int carId, string[] fileNames) {

            foreach (var fileName in fileNames) {
                var path = $"{_appEnvironment.WebRootPath}/img/cars/{carId}/{fileName}";
                if (System.IO.File.Exists(path)) {

                    var carImage = await _context.CarImages.FirstAsync(ci => ci.CarId == carId);
                    _context.CarImages.Remove(carImage);

                    System.IO.File.Delete(path);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        #region BodyTypeHandling

        [HttpGet]
        public async Task<IActionResult> BodyTypes() {
            return View(_context.BodyTypes);
        }

        [HttpGet]
        public async Task<IActionResult> CreateBodyType() {
            return View("CreateEditBodyType", new BodyType());
        }

        [HttpGet]
        public async Task<IActionResult> EditBodyType(int? id) {

            if (id == null) {
                return NotFound();
            }

            var bodyType = await _context.BodyTypes.FirstOrDefaultAsync(b => b.Id == id);

            if (bodyType == null) {
                return NotFound();
            }

            return View("CreateEditBodyType", bodyType);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditBodyType(BodyType bodyType) {

            if (ModelState.IsValid) {
                if (bodyType.Id == 0) {
                    _context.Add(bodyType);
                    await _context.SaveChangesAsync();
                }
                else {
                    try {
                        _context.Update(bodyType);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException) {
                        if (!_context.BodyTypes.Any(b => b.Id == bodyType.Id)) {
                            return NotFound();
                        }
                        else {
                            throw;
                        }
                    }
                }
                return RedirectToAction(nameof(BodyTypes));
            }
            return View(bodyType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBodyType(int id) {
            var bodyType = await _context.BodyTypes.FindAsync(id);
            _context.BodyTypes.Remove(bodyType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(BodyTypes));
        }

        #endregion

        #region FuelTypeHandling

        [HttpGet]
        public async Task<IActionResult> FuelTypes() {
            return View(_context.FuelTypes);
        }

        [HttpGet]
        public async Task<IActionResult> CreateFuelType() {
            return View("CreateEditFuelType", new FuelType());
        }

        [HttpGet]
        public async Task<IActionResult> EditFuelType(int? id) {

            if (id == null) {
                return NotFound();
            }

            var fuelType = await _context.FuelTypes.FirstOrDefaultAsync(f => f.Id == id);

            if (fuelType == null) {
                return NotFound();
            }

            return View("CreateEditFuelType", fuelType);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditFuelType(FuelType fuelType) {

            if (ModelState.IsValid) {
                if (fuelType.Id == 0) {
                    _context.Add(fuelType);
                    await _context.SaveChangesAsync();
                }
                else {
                    try {
                        _context.Update(fuelType);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException) {
                        if (!_context.FuelTypes.Any(f => f.Id == fuelType.Id)) {
                            return NotFound();
                        }
                        else {
                            throw;
                        }
                    }
                }
                return RedirectToAction(nameof(FuelTypes));
            }
            return View(fuelType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFuelType(int id) {
            var fuelType = await _context.FuelTypes.FindAsync(id);
            _context.FuelTypes.Remove(fuelType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(FuelTypes));
        }

        #endregion
        
        #region TransmissionTypeHandling

        [HttpGet]
        public async Task<IActionResult> TransmissionTypes() {
            return View(_context.TransmissionTypes);
        }

        [HttpGet]
        public async Task<IActionResult> CreateTransmissionType() {
            return View("CreateEditTransmissionType", new TransmissionType());
        }

        [HttpGet]
        public async Task<IActionResult> EditTransmissionType(int? id) {

            if (id == null) {
                return NotFound();
            }

            var transmissionType = await _context.TransmissionTypes.FirstOrDefaultAsync(t => t.Id == id);

            if (transmissionType == null) {
                return NotFound();
            }

            return View("CreateEditTransmissionType", transmissionType);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditTransmissionType(TransmissionType transmissionType) {

            if (ModelState.IsValid) {
                if (transmissionType.Id == 0) {
                    _context.Add(transmissionType);
                    await _context.SaveChangesAsync();
                }
                else {
                    try {
                        _context.Update(transmissionType);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException) {
                        if (!_context.TransmissionTypes.Any(t => t.Id == transmissionType.Id)) {
                            return NotFound();
                        }
                        else {
                            throw;
                        }
                    }
                }
                return RedirectToAction(nameof(TransmissionTypes));
            }
            return View(transmissionType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTransmissionType(int id) {
            var transmissionType = await _context.TransmissionTypes.FindAsync(id);
            _context.TransmissionTypes.Remove(transmissionType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TransmissionTypes));
        }

        #endregion


    }
}
