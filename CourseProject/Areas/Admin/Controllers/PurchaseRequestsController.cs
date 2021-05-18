using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Areas.Admin.Controllers {

    [Authorize(Roles = "admin, manager")]
    [Area("Admin")]
    public class PurchaseRequestsController : Controller {

        private readonly CarContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public PurchaseRequestsController(CarContext context, IWebHostEnvironment appEnvironment) {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public async Task<IActionResult> Index() {

            var requests = _context.PurchaseRequests
                .Include(pr => pr.Car)
                .ThenInclude(c => c.Model)
                .ThenInclude(cm => cm.Brand)
                .Include(pr => pr.Car)
                .ThenInclude(c => c.Model)
                .ThenInclude(cm => cm.Parent)
                .AsNoTracking();

            return View(await requests.ToListAsync());
        }
    }
}
