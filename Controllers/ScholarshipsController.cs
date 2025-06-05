using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UTEScholarshipSystem.Models;
using System.Linq;

namespace UTEScholarshipSystem.Controllers
{
    public class ScholarshipsController : Controller
    {
        private readonly ScholarshipDbContext _context;

        public ScholarshipsController(ScholarshipDbContext context)
        {
            _context = context;
        }

        // GET: Scholarships
        public async Task<IActionResult> Index()
        {
            var scholarships = await _context.Scholarships.ToListAsync();
            return View(scholarships);
        }

        // GET: Scholarships/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarship = await _context.Scholarships
                .Include(s => s.ScholarshipApplications)
                    .ThenInclude(sa => sa.Student)
                .FirstOrDefaultAsync(m => m.ScholarshipId == id);

            if (scholarship == null)
            {
                return NotFound();
            }

            return View(scholarship);
        }

        // GET: Scholarships/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Scholarships/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Description,Amount,Quantity,MinGPA,ApplicationStartDate,ApplicationEndDate,AcademicYear,AdditionalRequirements,Status")] Scholarship scholarship)
        {
            if (ModelState.IsValid)
            {
                scholarship.CreatedAt = DateTime.Now;
                scholarship.UpdatedAt = DateTime.Now;
                _context.Add(scholarship);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(scholarship);
        }

        // GET: Scholarships/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarship = await _context.Scholarships.FindAsync(id);
            if (scholarship == null)
            {
                return NotFound();
            }
            return View(scholarship);
        }

        // POST: Scholarships/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ScholarshipId,Name,Description,Amount,Quantity,MinGPA,ApplicationStartDate,ApplicationEndDate,AcademicYear,AdditionalRequirements,Status,CreatedAt")] Scholarship scholarship)
        {
            if (id != scholarship.ScholarshipId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    scholarship.UpdatedAt = DateTime.Now;
                    _context.Update(scholarship);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScholarshipExists(scholarship.ScholarshipId))
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
            return View(scholarship);
        }

        // GET: Scholarships/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarship = await _context.Scholarships
                .FirstOrDefaultAsync(m => m.ScholarshipId == id);
            if (scholarship == null)
            {
                return NotFound();
            }

            return View(scholarship);
        }

        // POST: Scholarships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scholarship = await _context.Scholarships.FindAsync(id);
            if (scholarship != null)
            {
                _context.Scholarships.Remove(scholarship);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Available scholarships for application
        public async Task<IActionResult> Available()
        {
            var availableScholarships = await _context.Scholarships
                .Where(s => s.Status == ScholarshipStatus.Active)
                .ToListAsync();

            return View(availableScholarships);
        }

        // GET: Scholarships/Search
        [HttpGet]
        public async Task<IActionResult> Search(string? major, decimal? minAmount, decimal? maxAmount, DateTime? startDate, DateTime? endDate, decimal? minGPA)
        {
            var query = _context.Scholarships.AsQueryable();

            if (!string.IsNullOrEmpty(major))
            {
                query = query.Where(s => s.AdditionalRequirements.Contains(major) || s.Description.Contains(major) || s.Name.Contains(major));
            }
            if (minAmount.HasValue)
            {
                query = query.Where(s => s.Amount >= minAmount.Value);
            }
            if (maxAmount.HasValue)
            {
                query = query.Where(s => s.Amount <= maxAmount.Value);
            }
            if (startDate.HasValue)
            {
                query = query.Where(s => s.ApplicationStartDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(s => s.ApplicationEndDate <= endDate.Value);
            }
            if (minGPA.HasValue)
            {
                query = query.Where(s => s.MinGPA >= (double)minGPA.Value);
            }

            var results = await query.OrderByDescending(s => s.ApplicationEndDate).ToListAsync();
            ViewBag.Major = major;
            ViewBag.MinAmount = minAmount;
            ViewBag.MaxAmount = maxAmount;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.MinGPA = minGPA;
            return View("Search", results);
        }

        private bool ScholarshipExists(int id)
        {
            return _context.Scholarships.Any(e => e.ScholarshipId == id);
        }
    }
} 