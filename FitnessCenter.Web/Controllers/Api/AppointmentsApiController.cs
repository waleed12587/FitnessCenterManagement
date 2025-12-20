using FitnessCenter.Web.Data;
using FitnessCenter.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/AppointmentsApi/mine
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<object>>> GetMyAppointments(
            [FromQuery] AppointmentStatus? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // LINQ query with multiple filters
            var query = _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.GymService)
                .Include(a => a.Member)
                .Where(a => a.MemberId == user.Id);

            // Filter by status
            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            // Filter by date range
            if (fromDate.HasValue)
            {
                query = query.Where(a => a.StartDateTime >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.StartDateTime <= toDate.Value);
            }

            var appointments = await query
                .OrderByDescending(a => a.StartDateTime)
                .Select(a => new
                {
                    a.Id,
                    a.StartDateTime,
                    a.EndDateTime,
                    a.Price,
                    a.Status,
                    TrainerName = a.Trainer.FullName,
                    ServiceName = a.GymService.Name,
                    MemberName = a.Member.UserName
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // GET: api/AppointmentsApi/trainer/5
        [HttpGet("trainer/{trainerId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTrainerAppointments(
            int trainerId,
            [FromQuery] DateTime? date = null)
        {
            var query = _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.GymService)
                .Where(a => a.TrainerId == trainerId && a.Status != AppointmentStatus.Cancelled);

            if (date.HasValue)
            {
                var startOfDay = date.Value.Date;
                var endOfDay = startOfDay.AddDays(1);
                query = query.Where(a => a.StartDateTime >= startOfDay && a.StartDateTime < endOfDay);
            }

            var appointments = await query
                .OrderBy(a => a.StartDateTime)
                .Select(a => new
                {
                    a.Id,
                    a.StartDateTime,
                    a.EndDateTime,
                    a.Status,
                    MemberName = a.Member.UserName,
                    ServiceName = a.GymService.Name
                })
                .ToListAsync();

            return Ok(appointments);
        }
    }
}


