using FitnessCenter.Web.Data;
using FitnessCenter.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TrainersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrainersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TrainersApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTrainers(
            [FromQuery] int? gymId = null,
            [FromQuery] string? specialty = null,
            [FromQuery] string? search = null)
        {
            // LINQ query with filtering
            var query = _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.TrainerServices)
                    .ThenInclude(ts => ts.GymService)
                .AsQueryable();

            // Filter by gym
            if (gymId.HasValue)
            {
                query = query.Where(t => t.GymId == gymId.Value);
            }

            // Filter by specialty
            if (!string.IsNullOrEmpty(specialty))
            {
                query = query.Where(t => t.Specialty != null && t.Specialty.Contains(specialty));
            }

            // Search by name
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.FullName.Contains(search));
            }

            var trainers = await query
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Specialty,
                    t.Bio,
                    GymName = t.Gym != null ? t.Gym.Name : null,
                    Services = t.TrainerServices.Select(ts => new
                    {
                        ts.GymService.Id,
                        ts.GymService.Name,
                        ts.GymService.Price
                    }).ToList()
                })
                .OrderBy(t => t.FullName)
                .ToListAsync();

            return Ok(trainers);
        }

        // GET: api/TrainersApi/available
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableTrainers(
            [FromQuery] int serviceId,
            [FromQuery] DateTime? date = null)
        {
            if (date == null)
            {
                date = DateTime.Now;
            }

            // LINQ query: Find trainers who offer the service and are available
            var availableTrainers = await _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.TrainerServices)
                .Include(t => t.Availabilities)
                .Where(t => t.TrainerServices.Any(ts => ts.GymServiceId == serviceId))
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Specialty,
                    GymName = t.Gym != null ? t.Gym.Name : null,
                    IsAvailable = !t.Availabilities.Any() || 
                        t.Availabilities.Any(a => 
                            a.DayOfWeek == date.Value.DayOfWeek &&
                            a.StartTime <= date.Value.TimeOfDay &&
                            a.EndTime >= date.Value.TimeOfDay)
                })
                .OrderBy(t => t.FullName)
                .ToListAsync();

            return Ok(availableTrainers);
        }

        // GET: api/TrainersApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTrainer(int id)
        {
            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.TrainerServices)
                    .ThenInclude(ts => ts.GymService)
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Specialty,
                    t.Bio,
                    GymName = t.Gym != null ? t.Gym.Name : null,
                    Services = t.TrainerServices.Select(ts => new
                    {
                        ts.GymService.Id,
                        ts.GymService.Name,
                        ts.GymService.Price,
                        ts.GymService.DurationMinutes
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (trainer == null)
            {
                return NotFound();
            }

            return Ok(trainer);
        }
    }
}

