using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseProject.Data;
using CourseProject.Models;

namespace CourseProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CarsController : Controller
    {
        private readonly CarContext _context;

        public CarsController(CarContext context)
        {
            _context = context;
        }

        // GET: Admin/Cars
        public async Task<IActionResult> Index()
        {
            var carContext = _context.Cars.Include(c => c.BodyType).Include(c => c.FuelType).Include(c => c.Model).Include(c => c.TransmissionType);
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
                .FirstOrDefaultAsync(m => m.ID == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Admin/Cars/Create
        public IActionResult Create()
        {
            ViewData["BodyTypeID"] = new SelectList(_context.BodyTypes, "ID", "ID");
            ViewData["FuelTypeID"] = new SelectList(_context.FuelTypes, "ID", "ID");
            ViewData["ModelID"] = new SelectList(_context.CarModels, "ID", "ID");
            ViewData["TransmissionTypeID"] = new SelectList(_context.TransmissionTypes, "ID", "ID");
            return View();
        }

        // POST: Admin/Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Year,Price,State,ModelID,EngineVolume,Mileage,FuelTypeID,BodyTypeID,TransmissionTypeID,ImagesDirectoryPath")] Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BodyTypeID"] = new SelectList(_context.BodyTypes, "ID", "ID", car.BodyTypeID);
            ViewData["FuelTypeID"] = new SelectList(_context.FuelTypes, "ID", "ID", car.FuelTypeID);
            ViewData["ModelID"] = new SelectList(_context.CarModels, "ID", "ID", car.ModelID);
            ViewData["TransmissionTypeID"] = new SelectList(_context.TransmissionTypes, "ID", "ID", car.TransmissionTypeID);
            return View(car);
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
            ViewData["BodyTypeID"] = new SelectList(_context.BodyTypes, "ID", "ID", car.BodyTypeID);
            ViewData["FuelTypeID"] = new SelectList(_context.FuelTypes, "ID", "ID", car.FuelTypeID);
            ViewData["ModelID"] = new SelectList(_context.CarModels, "ID", "ID", car.ModelID);
            ViewData["TransmissionTypeID"] = new SelectList(_context.TransmissionTypes, "ID", "ID", car.TransmissionTypeID);
            return View(car);
        }

        // POST: Admin/Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Year,Price,State,ModelID,EngineVolume,Mileage,FuelTypeID,BodyTypeID,TransmissionTypeID,ImagesDirectoryPath")] Car car)
        {
            if (id != car.ID)
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
                    if (!CarExists(car.ID))
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
            ViewData["BodyTypeID"] = new SelectList(_context.BodyTypes, "ID", "ID", car.BodyTypeID);
            ViewData["FuelTypeID"] = new SelectList(_context.FuelTypes, "ID", "ID", car.FuelTypeID);
            ViewData["ModelID"] = new SelectList(_context.CarModels, "ID", "ID", car.ModelID);
            ViewData["TransmissionTypeID"] = new SelectList(_context.TransmissionTypes, "ID", "ID", car.TransmissionTypeID);
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
                .FirstOrDefaultAsync(m => m.ID == id);
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
            return _context.Cars.Any(e => e.ID == id);
        }
    }
}
