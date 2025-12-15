using FitnessCenter.Web.Data;
using FitnessCenter.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin/Appointments/Pending
        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var list = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.GymService)
                .Include(a => a.Member) // إذا عندك navigation Member
                .Where(a => a.Status == AppointmentStatus.Pending)
                .OrderBy(a => a.StartDateTime)
                .ToListAsync();

            return View(list);
        }

        // Admin/Appointments/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var app = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
            if (app == null) return NotFound();

            app.Status = AppointmentStatus.Approved;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Pending));
        }

        // Admin/Appointments/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var app = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
            if (app == null) return NotFound();

            app.Status = AppointmentStatus.Rejected;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Pending));
        }
    }
}
