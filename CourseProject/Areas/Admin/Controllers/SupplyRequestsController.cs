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
    public class SupplyRequestsController : Controller
    {
        private readonly CarContext _context;

        public SupplyRequestsController(CarContext context)
        {
            _context = context;
        }

        // GET: Admin/SupplyRequests
        public async Task<IActionResult> Index()
        {
            var carContext = _context.SupplyRequests
                .Include(s => s.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(cm => cm.Brand)
                .Include(s => s.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(cm => cm.Parent)
                .Include(s => s.Dealer);
            return View(await carContext.ToListAsync());
        }

        // GET: Admin/SupplyRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyRequest = await _context.SupplyRequests
                .Include(s => s.Car)
                .Include(s => s.Dealer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplyRequest == null)
            {
                return NotFound();
            }

            return View(supplyRequest);
        }

        // GET: Admin/SupplyRequests/Create
        public async Task<IActionResult> Create(int? carId)
        {

            if (carId == null) {
                return NotFound();
            }

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == carId);

            if (car == null) {
                return NotFound();
            }

            ViewData["DealerId"] = new SelectList(_context.Dealers, "Id", "Name");
            return View(new SupplyRequest() {CarId = carId.Value});
        }

        // POST: Admin/SupplyRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DealerId,CarId,Count,State")] SupplyRequest supplyRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplyRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Name", supplyRequest.CarId);
            ViewData["DealerId"] = new SelectList(_context.Dealers, "Id", "Name", supplyRequest.DealerId);
            return View(supplyRequest);
        }

        // GET: Admin/SupplyRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyRequest = await _context.SupplyRequests.FindAsync(id);
            if (supplyRequest == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Id", supplyRequest.CarId);
            ViewData["DealerId"] = new SelectList(_context.Dealers, "Id", "Id", supplyRequest.DealerId);
            return View(supplyRequest);
        }

        // POST: Admin/SupplyRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DealerId,CarId,Count,State")] SupplyRequest supplyRequest)
        {
            if (id != supplyRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplyRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplyRequestExists(supplyRequest.Id))
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
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Id", supplyRequest.CarId);
            ViewData["DealerId"] = new SelectList(_context.Dealers, "Id", "Id", supplyRequest.DealerId);
            return View(supplyRequest);
        }

        // GET: Admin/SupplyRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyRequest = await _context.SupplyRequests
                .Include(s => s.Car)
                .Include(s => s.Dealer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplyRequest == null)
            {
                return NotFound();
            }

            return View(supplyRequest);
        }

        // POST: Admin/SupplyRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplyRequest = await _context.SupplyRequests.FindAsync(id);
            _context.SupplyRequests.Remove(supplyRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplyRequestExists(int id)
        {
            return _context.SupplyRequests.Any(e => e.Id == id);
        }
    }
}
