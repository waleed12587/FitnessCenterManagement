using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Web.Data;
using FitnessCenter.Web.Models;
using FitnessCenter.Web.ViewModels;

namespace FitnessCenter.Web.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            var trainers = _context.Trainers.Include(t => t.Gym);
            return View(await trainers.ToListAsync());

        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // GET: Trainers/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewData["GymId"] = new SelectList(await _context.Gyms.OrderBy(g => g.Name).ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Trainers/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Specialty,Bio,GymId")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GymId"] = new SelectList(await _context.Gyms.OrderBy(g => g.Name).ToListAsync(), "Id", "Name", trainer.GymId);
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer == null) return NotFound();

            var allServices = await _context.GymServices
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToListAsync();

            var selectedServiceIds = await _context.TrainerServices
                .Where(ts => ts.TrainerId == trainer.Id)
                .Select(ts => ts.GymServiceId)
                .ToListAsync();

            var vm = new TrainerEditVM
            {
                Id = trainer.Id,
                FullName = trainer.FullName,
                Specialty = trainer.Specialty,
                Bio = trainer.Bio,
                GymId = trainer.GymId,
                Services = allServices.Select(s => new ServiceCheckboxVM
                {
                    GymServiceId = s.Id,
                    Name = s.Name,
                    IsSelected = selectedServiceIds.Contains(s.Id)
                }).ToList()
            };

            // لو عندك Dropdown للجيم بالـ Edit (اختياري)
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", trainer.GymId);

            return View(vm);
        }


        // POST: Trainers/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerEditVM vm)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", vm.GymId);
                return View(vm);
            }

            // حدّث بيانات المدرّب الأساسية
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Id == vm.Id);
            if (trainer == null) return NotFound();

            trainer.FullName = vm.FullName;
            trainer.Specialty = vm.Specialty;
            trainer.Bio = vm.Bio;
            trainer.GymId = vm.GymId;

            // حدّث روابط الخدمات (TrainerServices)
            var existingLinks = await _context.TrainerServices
                .Where(ts => ts.TrainerId == trainer.Id)
                .ToListAsync();

            _context.TrainerServices.RemoveRange(existingLinks);

            var selectedIds = vm.Services
                .Where(s => s.IsSelected)
                .Select(s => s.GymServiceId)
                .ToList();

            foreach (var serviceId in selectedIds)
            {
                _context.TrainerServices.Add(new TrainerService
                {
                    TrainerId = trainer.Id,
                    GymServiceId = serviceId
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Trainers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
        }
    }
}
