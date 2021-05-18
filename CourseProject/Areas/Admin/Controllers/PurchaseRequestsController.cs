﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProject.Data;
using CourseProject.Models;
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

            ViewBag.PurchaseRequestStates = Enum.GetValues(typeof(PurchaseRequest.RequestState)).Cast<int>().ToDictionary(key => key, key => Enum.GetName(typeof(PurchaseRequest.RequestState), key));
            
            return View(await requests.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id) {

            if (id == null) {
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
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (request == null) {
                return NotFound();
            }

            ViewBag.PurchaseRequestStates = Enum.GetValues(typeof(PurchaseRequest.RequestState)).Cast<int>().ToDictionary(key => key, key => Enum.GetName(typeof(PurchaseRequest.RequestState), key));

            return View(request);
        }

        public IActionResult ChangeRequestState(int? requestId, int? newState) {

            if (requestId == null || newState == null) {
                return NotFound();
            }

            var request = _context.PurchaseRequests.FirstOrDefault(pr => pr.Id == requestId);

            if (request == null) {
                return NotFound();
            }

            request.State = (PurchaseRequest.RequestState)newState.Value;
            _context.Update(request);
            _context.SaveChanges();

            return Content(Enum.GetName(typeof(PurchaseRequest.RequestState), (PurchaseRequest.RequestState)newState.Value));
        }

    }
}
