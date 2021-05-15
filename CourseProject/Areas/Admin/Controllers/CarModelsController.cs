using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseProject.Data;
using CourseProject.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CourseProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CarModelsController : Controller
    {
        private readonly CarContext _context;

        public CarModelsController(CarContext context)
        {
            _context = context;
        }

        // GET: Admin/CarModels
        public async Task<IActionResult> Index()
        {
            var carContext = _context.CarModels.Include(c => c.Brand).Include(c => c.Parent);
            return View(await carContext.ToListAsync());
        }

        // GET: Admin/CarModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carModel = await _context.CarModels
                .Include(c => c.Brand)
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carModel == null)
            {
                return NotFound();
            }

            return View(carModel);
        }

        private List<CarModel> CreateParentModelsList() {
            var parentModels = from cm in _context.CarModels
                where cm.ParentId == null
                orderby cm.Name
                select cm;
            return parentModels.ToList();
        }

        // GET: Admin/CarModels/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["ParentId"] = new SelectList(CreateParentModelsList(), "Id", "Name");
            return View();
        }

        // POST: Admin/CarModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,BrandId,ParentId")] CarModel carModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", carModel.BrandId);
            ViewData["ParentId"] = new SelectList(CreateParentModelsList(), "Id", "Name", carModel.ParentId);
            return View(carModel);
        }

        // GET: Admin/CarModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carModel = await _context.CarModels.FindAsync(id);
            if (carModel == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", carModel.BrandId);
            ViewData["ParentId"] = new SelectList(CreateParentModelsList(), "Id", "Name", carModel.ParentId);
            return View(carModel);
        }

        // POST: Admin/CarModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,BrandId,ParentId")] CarModel carModel)
        {
            if (id != carModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarModelExists(carModel.Id))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", carModel.BrandId);
            ViewData["ParentId"] = new SelectList(CreateParentModelsList(), "Id", "Name", carModel.ParentId);
            return View(carModel);
        }

        // GET: Admin/CarModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carModel = await _context.CarModels
                .Include(c => c.Brand)
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carModel == null)
            {
                return NotFound();
            }

            return View(carModel);
        }

        // POST: Admin/CarModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carModel = await _context.CarModels.FindAsync(id);
            _context.CarModels.Remove(carModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarModelExists(int id)
        {
            return _context.CarModels.Any(e => e.Id == id);
        }
    }
}
