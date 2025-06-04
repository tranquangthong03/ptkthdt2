using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UTEScholarshipSystem.Models;
using System.Text.Json;

namespace UTEScholarshipSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ScholarshipDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(
            ScholarshipDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Student/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction("Index", "Home");
            }

            var student = await _context.Students
                .Include(s => s.ScholarshipApplications)
                    .ThenInclude(a => a.Scholarship)
                .FirstOrDefaultAsync(s => s.StudentId == user.StudentId);

            if (student == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction("Index", "Home");
            }

            var availableScholarships = await _context.Scholarships
                .Where(s => s.Status == ScholarshipStatus.Active &&
                           s.ApplicationStartDate <= DateTime.Now &&
                           s.ApplicationEndDate >= DateTime.Now &&
                           s.MinGPA <= student.GPA)
                .Where(s => !s.ScholarshipApplications.Any(a => a.StudentId == student.StudentId))
                .ToListAsync();

            ViewBag.Student = student;
            ViewBag.AvailableScholarships = availableScholarships;

            return View();
        }

        // GET: Student/Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction("Dashboard");
            }

            var student = await _context.Students.FindAsync(user.StudentId);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction("Dashboard");
            }

            return View(student);
        }

        // GET: Student/MyApplications
        public async Task<IActionResult> MyApplications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction("Dashboard");
            }

            var applications = await _context.ScholarshipApplications
                .Include(a => a.Scholarship)
                .Where(a => a.StudentId == user.StudentId)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();

            return View(applications);
        }

        // GET: Student/ApplyScholarship/5
        public async Task<IActionResult> ApplyScholarship(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction("Dashboard");
            }

            var scholarship = await _context.Scholarships.FindAsync(id);
            if (scholarship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy học bổng.";
                return RedirectToAction("Dashboard");
            }

            var student = await _context.Students.FindAsync(user.StudentId);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction("Dashboard");
            }

            // Check if scholarship is available
            if (scholarship.Status != ScholarshipStatus.Active ||
                scholarship.ApplicationStartDate > DateTime.Now ||
                scholarship.ApplicationEndDate < DateTime.Now)
            {
                TempData["ErrorMessage"] = "Học bổng này hiện không nhận hồ sơ.";
                return RedirectToAction("Dashboard");
            }

            // Check if student meets GPA requirement
            if (student.GPA < scholarship.MinGPA)
            {
                TempData["ErrorMessage"] = $"GPA của bạn ({student.GPA}) không đạt yêu cầu tối thiểu ({scholarship.MinGPA}).";
                return RedirectToAction("Dashboard");
            }

            // Check if student already applied
            var existingApplication = await _context.ScholarshipApplications
                .FirstOrDefaultAsync(a => a.StudentId == student.StudentId && a.ScholarshipId == scholarship.ScholarshipId);

            if (existingApplication != null)
            {
                TempData["ErrorMessage"] = "Bạn đã nộp đơn xin học bổng này rồi.";
                return RedirectToAction("MyApplications");
            }

            var application = new ScholarshipApplication
            {
                StudentId = student.StudentId,
                ScholarshipId = scholarship.ScholarshipId,
                GPAAtApplication = student.GPA
            };

            ViewBag.Scholarship = scholarship;
            ViewBag.Student = student;

            return View(application);
        }

        // POST: Student/ApplyScholarship
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyScholarship(ScholarshipApplication application)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction("Dashboard");
            }

            var student = await _context.Students.FindAsync(user.StudentId);
            var scholarship = await _context.Scholarships.FindAsync(application.ScholarshipId);

            if (student == null || scholarship == null)
            {
                TempData["ErrorMessage"] = "Thông tin không hợp lệ.";
                return RedirectToAction("Dashboard");
            }

            // Validate application reason
            if (string.IsNullOrWhiteSpace(application.ApplicationReason))
            {
                ModelState.AddModelError("ApplicationReason", "Vui lòng nhập lý do xin học bổng.");
                ViewBag.Scholarship = scholarship;
                ViewBag.Student = student;
                return View(application);
            }

            // Check if student already applied
            var existingApplication = await _context.ScholarshipApplications
                .FirstOrDefaultAsync(a => a.StudentId == student.StudentId && a.ScholarshipId == scholarship.ScholarshipId);

            if (existingApplication != null)
            {
                TempData["ErrorMessage"] = "Bạn đã nộp đơn xin học bổng này rồi.";
                return RedirectToAction("MyApplications");
            }

            application.StudentId = student.StudentId;
            application.GPAAtApplication = student.GPA;
            application.ApplicationDate = DateTime.Now;
            application.Status = ApplicationStatus.Pending;
            application.CreatedAt = DateTime.Now;
            application.UpdatedAt = DateTime.Now;

            _context.ScholarshipApplications.Add(application);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã nộp đơn xin học bổng '{scholarship.Name}' thành công.";
            return RedirectToAction("MyApplications");
        }

        // GET: Student/ApplicationDetails/5
        public async Task<IActionResult> ApplicationDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction("Dashboard");
            }

            var application = await _context.ScholarshipApplications
                .Include(a => a.Scholarship)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.ApplicationId == id && a.StudentId == user.StudentId);

            if (application == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn xin học bổng.";
                return RedirectToAction("MyApplications");
            }

            return View(application);
        }

        // POST: Student/CancelApplication/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelApplication(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction("Dashboard");
            }

            var application = await _context.ScholarshipApplications
                .Include(a => a.Scholarship)
                .FirstOrDefaultAsync(a => a.ApplicationId == id && a.StudentId == user.StudentId);

            if (application == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn xin học bổng.";
                return RedirectToAction("MyApplications");
            }

            if (application.Status != ApplicationStatus.Pending)
            {
                TempData["ErrorMessage"] = "Chỉ có thể hủy đơn đang chờ xét duyệt.";
                return RedirectToAction("MyApplications");
            }

            application.Status = ApplicationStatus.Cancelled;
            application.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã hủy đơn xin học bổng '{application.Scholarship.Name}'.";
            return RedirectToAction("MyApplications");
        }

        // GET: Student/ProgressHistory/5
        public async Task<IActionResult> ProgressHistory(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentId == null)
            {
                return Unauthorized();
            }
            var application = await _context.ScholarshipApplications
                .FirstOrDefaultAsync(a => a.ApplicationId == id && a.StudentId == user.StudentId);
            if (application == null)
            {
                return NotFound();
            }
            var steps = new List<ProgressStep>();
            if (!string.IsNullOrEmpty(application.ProgressSteps))
            {
                steps = JsonSerializer.Deserialize<List<ProgressStep>>(application.ProgressSteps) ?? new List<ProgressStep>();
            }
            return PartialView("_ProgressHistory", steps);
        }
    }
} 