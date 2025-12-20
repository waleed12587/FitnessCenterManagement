using FitnessCenter.Web.Data;
using FitnessCenter.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Web.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Payment/Pay/{appointmentId}
        public async Task<IActionResult> Pay(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var appointment = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.GymService)
                .Include(a => a.Member)
                .FirstOrDefaultAsync(a => a.Id == id && a.MemberId == user.Id);

            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.PaymentStatus == PaymentStatus.Paid)
            {
                TempData["Message"] = "This appointment has already been paid.";
                return RedirectToAction("MyAppointments", "Appointments");
            }

            return View(appointment);
        }

        // POST: Payment/ProcessPayment/{appointmentId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int id, string paymentMethod)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.MemberId == user.Id);

            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.PaymentStatus == PaymentStatus.Paid)
            {
                TempData["Error"] = "This appointment has already been paid.";
                return RedirectToAction("MyAppointments", "Appointments");
            }

            // Simple payment simulation - in real app, integrate with payment gateway
            // For demo purposes, we'll just mark it as paid
            appointment.PaymentStatus = PaymentStatus.Paid;
            appointment.PaymentDate = DateTime.Now;
            appointment.PaymentMethod = paymentMethod ?? "Cash"; // Store payment method

            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment completed successfully!";
            return RedirectToAction("MyAppointments", "Appointments");
        }
    }
}

