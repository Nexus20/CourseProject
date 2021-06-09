using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseProject.Data;
using CourseProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace CourseProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin, manager")]
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

            var carModels = _context.CarModels
                .Include(cm => cm.Brand)
                .Where(cm => cm.ParentId == null)
                .Include(cm => cm.Children);


            return View(await carModels.ToListAsync());
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

        private List<CarModel> CreateParentModelsList()
        {
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
            if (_context.CarModels.FirstOrDefault(cm => cm.Name == carModel.Name) != null)
            {
                ModelState.AddModelError("Name", "This model already exists");
            }
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
        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? hasChildrenError)
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

            if (hasChildrenError.GetValueOrDefault())
            {
                ViewData["HasChildrenError"] =
                    "The record you attempted to delete has child records. You have to delete child elements first!";
            }

            return View(carModel);
        }

        // POST: Admin/CarModels/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(CarModel carModel)
        {

            try
            {
                if (await _context.CarModels.AnyAsync(cm => cm.Id == carModel.Id))
                {
                    _context.CarModels.Remove(carModel);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Delete), new { id = carModel.Id, hasChildrenError = true });
            }
        }

        private bool CarModelExists(int id)
        {
            return _context.CarModels.Any(e => e.Id == id);
        }

        public IActionResult Brands()
        {
            return View(_context.Brands);
        }

        [HttpGet]
        public IActionResult CreateBrand()
        {
            return View("CreateEditBrand", new Brand());
        }

        [HttpGet]
        public async Task<IActionResult> EditBrand(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null)
            {
                return NotFound();
            }

            return View("CreateEditBrand", brand);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditBrand(Brand brand)
        {

            if (ModelState.IsValid)
            {
                if (brand.Id == 0)
                {
                    _context.Add(brand);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        _context.Update(brand);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_context.Brands.Any(b => b.Id == brand.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return RedirectToAction(nameof(Brands));
            }
            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Brands));
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckBrand(string name)
        {
            return Json(_context.Brands.FirstOrDefault(b => b.Name == name) == null);
        }

    }
}
