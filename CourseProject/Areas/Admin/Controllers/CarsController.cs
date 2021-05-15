﻿using System;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CourseProject.Areas.Admin.Controllers
{
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
        public async Task<IActionResult> Index()
        {
            var carContext = _context.Cars
                .Include(c => c.BodyType)
                .Include(c => c.FuelType)
                .Include(c => c.Model)
                    .ThenInclude(cm => cm.Brand)
                .Include(c => c.TransmissionType);


            ViewBag.ImagesDirectory = $"{_appEnvironment.WebRootPath}/img/cars/";

            return View(await carContext.ToListAsync());
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
                .Include(c => c.TransmissionType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Create([Bind("Id,Year,Price,State,ModelId,EngineVolume,Mileage,FuelTypeId,BodyTypeId,TransmissionTypeId")] Car car)
        {
            if (ModelState.IsValid) {
                _context.Add(car);
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
                .AsNoTracking();

            var viewModel = new List<CarBrandModel>();
            foreach (var carModel in carModels) {
                viewModel.Add(new CarBrandModel() {
                    ModelId = carModel.Id,
                    ModelName = carModel.Name,
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
                .Include(c => c.TransmissionType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Admin/Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
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

            var car = _context.Cars.FirstOrDefaultAsync(car => car.Id == carId);

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
                }

            }

            return RedirectToAction(nameof(Index));
        }

    }
}
