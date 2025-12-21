using FitnessCenter.Web.Data;
using FitnessCenter.Web.Models;
using FitnessCenter.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessCenter.Web.Controllers
{
    [Authorize] // members must be logged in
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create()
        {
            // services dropdown
            ViewData["GymServiceId"] = new SelectList(await _context.GymServices.OrderBy(s => s.Name).ToListAsync(), "Id", "Name");

            // trainers dropdown empty at first (we'll fill by selected service later via JavaScript)
            ViewData["TrainerId"] = new SelectList(new List<Trainer>(), "Id", "FullName");

            // Payment methods
            ViewData["PaymentMethods"] = new SelectList(new[]
            {
                new { Value = "Cash", Text = "Cash" },
                new { Value = "Credit Card", Text = "Credit Card" },
                new { Value = "Debit Card", Text = "Debit Card" },
                new { Value = "Online Payment", Text = "Online Payment" }
            }, "Value", "Text", "Cash");

            var tomorrow = DateTime.Now.AddDays(1).Date;
            var defaultTime = new TimeSpan(10, 0, 0); // 10:00 AM

            return View(new AppointmentCreateVM 
            { 
                AppointmentDate = tomorrow,
                AppointmentTime = defaultTime,
                PaymentMethod = "Cash"
            });
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewData["GymServiceId"] = new SelectList(_context.GymServices.OrderBy(s => s.Name), "Id", "Name", vm.GymServiceId);
                
                // Load trainers for the selected service
                var trainersForService = await _context.TrainerServices
                    .Where(ts => ts.GymServiceId == vm.GymServiceId)
                    .Include(ts => ts.Trainer)
                    .Select(ts => ts.Trainer)
                    .OrderBy(t => t.FullName)
                    .ToListAsync();
                
                ViewData["TrainerId"] = new SelectList(trainersForService, "Id", "FullName", vm.TrainerId);
                
                ViewData["PaymentMethods"] = new SelectList(new[]
                {
                    new { Value = "Cash", Text = "Cash" },
                    new { Value = "Credit Card", Text = "Credit Card" },
                    new { Value = "Debit Card", Text = "Debit Card" },
                    new { Value = "Online Payment", Text = "Online Payment" }
                }, "Value", "Text", vm.PaymentMethod);
                
                return View(vm);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // service info
            var service = await _context.GymServices.FirstOrDefaultAsync(s => s.Id == vm.GymServiceId);
            if (service == null) return NotFound("Service not found.");

            // ensure trainer offers this service (TrainerService)
            bool trainerOffersService = await _context.TrainerServices
                .AnyAsync(ts => ts.TrainerId == vm.TrainerId && ts.GymServiceId == vm.GymServiceId);

            if (!trainerOffersService)
            {
                ModelState.AddModelError("", "The selected trainer does not offer this service.");
                ViewData["GymServiceId"] = new SelectList(_context.GymServices.OrderBy(s => s.Name), "Id", "Name", vm.GymServiceId);
                
                var trainersForService = await _context.TrainerServices
                    .Where(ts => ts.GymServiceId == vm.GymServiceId)
                    .Include(ts => ts.Trainer)
                    .Select(ts => ts.Trainer)
                    .OrderBy(t => t.FullName)
                    .ToListAsync();
                
                ViewData["TrainerId"] = new SelectList(trainersForService, "Id", "FullName", vm.TrainerId);
                
                ViewData["PaymentMethods"] = new SelectList(new[]
                {
                    new { Value = "Cash", Text = "Cash" },
                    new { Value = "Credit Card", Text = "Credit Card" },
                    new { Value = "Debit Card", Text = "Debit Card" },
                    new { Value = "Online Payment", Text = "Online Payment" }
                }, "Value", "Text", vm.PaymentMethod);
                
                return View(vm);
            }

            var start = vm.StartDateTime;
            var end = start.AddMinutes(service.DurationMinutes);

            // conflict check: overlap with existing appointments for same trainer (not cancelled/rejected)
            bool hasConflict = await _context.Appointments.AnyAsync(a =>
                a.TrainerId == vm.TrainerId &&
                a.Status != AppointmentStatus.Cancelled &&
                a.Status != AppointmentStatus.Rejected &&
                start < a.EndDateTime && end > a.StartDateTime
            );

            if (hasConflict)
            {
                ModelState.AddModelError("", "The trainer has another appointment at this time.");
                ViewData["GymServiceId"] = new SelectList(_context.GymServices.OrderBy(s => s.Name), "Id", "Name", vm.GymServiceId);
                
                var trainersForService = await _context.TrainerServices
                    .Where(ts => ts.GymServiceId == vm.GymServiceId)
                    .Include(ts => ts.Trainer)
                    .Select(ts => ts.Trainer)
                    .OrderBy(t => t.FullName)
                    .ToListAsync();
                
                ViewData["TrainerId"] = new SelectList(trainersForService, "Id", "FullName", vm.TrainerId);
                
                ViewData["PaymentMethods"] = new SelectList(new[]
                {
                    new { Value = "Cash", Text = "Cash" },
                    new { Value = "Credit Card", Text = "Credit Card" },
                    new { Value = "Debit Card", Text = "Debit Card" },
                    new { Value = "Online Payment", Text = "Online Payment" }
                }, "Value", "Text", vm.PaymentMethod);
                
                return View(vm);
            }

            var appointment = new Appointment
            {
                MemberId = user.Id,
                TrainerId = vm.TrainerId,
                GymServiceId = vm.GymServiceId,
                StartDateTime = start,
                EndDateTime = end,
                Price = service.Price,
                Status = AppointmentStatus.Pending,
                PaymentMethod = vm.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Appointment created successfully!";
            return RedirectToAction("MyAppointments");
        }

        // Member view: list only his appointments
        public async Task<IActionResult> MyAppointments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var list = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.GymService)
                .Where(a => a.MemberId == user.Id)
                .OrderByDescending(a => a.StartDateTime)
                .ToListAsync();

            return View(list);
        }
    }
}