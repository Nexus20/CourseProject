﻿using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseProject.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Controllers {
    public class HomeController : Controller {

        private readonly ILogger<HomeController> _logger;
        private readonly CarContext _context;

        public HomeController(CarContext context, ILogger<HomeController> logger) {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(int? brandId, int? modelId, int[] fuelTypes, int[] bodyTypes, int[] transmissionTypes) {

            ViewBag.TransmissionTypes = _context.TransmissionTypes;
            ViewBag.BodyTypes = _context.BodyTypes;
            ViewBag.FuelTypes = _context.FuelTypes;
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name");

            //var r = Request.Query;

            IQueryable<Car> cars = _context.Cars
                .Include(c => c.Model)
                    .ThenInclude(cm => cm.Brand)
                .Include(c => c.Model)
                .Include(c => c.BodyType)
                .Include(c => c.FuelType)
                .Include(c => c.TransmissionType)
                .AsNoTracking();

            if (brandId != null) {
                cars = cars.Where(c => c.Model.BrandId == brandId);
            }

            if (modelId != null) {
                cars = cars.Where(c => c.ModelId == modelId);
            }

            if (fuelTypes.Length > 0) {
                //cars = fuelTypes.Select((t, i) => i).Aggregate(cars,
                //    (current, i1) => current.Where(c => c.FuelTypeId == fuelTypes[i1]));
                IQueryable<Car> cars2 = cars.Where(c => c.FuelTypeId == fuelTypes[0]);
                for (var i = 1; i < fuelTypes.Length; i++) {
                    var i1 = i;
                    cars2 = cars2.Concat(cars.Where(c => c.FuelTypeId == fuelTypes[i1]));
                }

                cars = cars2;
            }

            if (bodyTypes.Length > 0) {
                //cars = bodyTypes.Select((t, i) => i).Aggregate(cars,
                //    (current, i1) => current.Where(c => c.BodyTypeId == bodyTypes[i1]));

                IQueryable<Car> cars2 = cars.Where(c => c.BodyTypeId == bodyTypes[0]);
                for (var i = 1; i < bodyTypes.Length; i++) {
                    var i1 = i;
                    cars2 = cars2.Concat(cars.Where(c => c.BodyTypeId == bodyTypes[i1]));
                }

                cars = cars2;

            }

            if (transmissionTypes.Length > 0) {
                //cars = transmissionTypes.Select((t, i) => i).Aggregate(cars,
                //    (current, i1) => current.Where(c => c.TransmissionTypeId == transmissionTypes[i1]));

                IQueryable<Car> cars2 = cars.Where(c => c.TransmissionTypeId == transmissionTypes[0]);
                for (var i = 1; i < transmissionTypes.Length; i++) {
                    var i1 = i;
                    cars2 = cars2.Concat(cars.Where(c => c.TransmissionTypeId == transmissionTypes[i1]));
                }

                cars = cars2;

            }


            //for (var i = 0; i < fuelTypes.Length; i++) {
            //    var i1 = i;
            //    cars = cars.Where(c => c.FuelTypeId == fuelTypes[i1]);
            //}


            return View(cars);
        }

        public IActionResult GetModelsByBrand(int? brandId) {

            if (brandId == null) {
                return Content("<option value=\"\"></option>", "text/html");
            }

            var brand = _context.Brands.FirstOrDefault(b => b.Id == brandId);

            if (brand == null) {
                return Content("<option value=\"\"></option>", "text/html");
            }

            var sb = new StringBuilder();

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
    }
}
