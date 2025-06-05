using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UTEScholarshipSystem.Models;

namespace UTEScholarshipSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ScholarshipDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ScholarshipDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var stats = new
            {
                TotalStudents = await _context.Students.CountAsync(),
                TotalScholarships = await _context.Scholarships.CountAsync(),
                ActiveScholarships = await _context.Scholarships.CountAsync(s => s.Status == ScholarshipStatus.Active),
                TotalApplications = await _context.ScholarshipApplications.CountAsync(),
                PendingApplications = await _context.ScholarshipApplications.CountAsync(a => a.Status == ApplicationStatus.Pending),
                ApprovedApplications = await _context.ScholarshipApplications.CountAsync(a => a.Status == ApplicationStatus.Approved),
                StudentAccounts = await _userManager.GetUsersInRoleAsync("Student")
            };

            ViewBag.Stats = stats;

            // Recent applications
            var recentApplications = await _context.ScholarshipApplications
                .Include(a => a.Student)
                .Include(a => a.Scholarship)
                .OrderByDescending(a => a.ApplicationDate)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentApplications = recentApplications;

            return View();
        }

        // GET: Admin/ManageRoles
        public async Task<IActionResult> ManageRoles()
        {
            var roles = _roleManager.Roles.ToList();
            var users = _userManager.Users.ToList();
            var userRoles = new List<List<string>>();
            foreach (var user in users)
            {
                var rolesOfUser = await _userManager.GetRolesAsync(user);
                userRoles.Add(rolesOfUser.ToList());
            }
            ViewBag.Users = users;
            ViewBag.Roles = roles;
            ViewBag.UserRoles = userRoles;
            return View(roles);
        }

        // GET: Admin/ManageUsers
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users
                .Include(u => u.Student)
                .ToListAsync();

            var usersWithRoles = new List<object>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new
                {
                    User = user,
                    Roles = roles,
                    Student = user.Student
                });
            }

            return View(usersWithRoles);
        }

        // GET: Admin/ScholarshipApplications
        public async Task<IActionResult> ScholarshipApplications(string status = "")
        {
            var query = _context.ScholarshipApplications
                .Include(a => a.Student)
                .Include(a => a.Scholarship)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ApplicationStatus>(status, out var statusEnum))
            {
                query = query.Where(a => a.Status == statusEnum);
            }

            var applications = await query
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();

            ViewBag.StatusFilter = status;
            return View(applications);
        }

        // POST: Admin/ReviewApplication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewApplication(int id, string status, string? reviewNotes)
        {
            var application = await _context.ScholarshipApplications
                .Include(a => a.Student)
                .Include(a => a.Scholarship)
                .FirstOrDefaultAsync(a => a.ApplicationId == id);
            if (application == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn xin học bổng.";
                return RedirectToAction("ScholarshipApplications");
            }

            if (!Enum.TryParse<ApplicationStatus>(status, out var parsedStatus))
            {
                TempData["ErrorMessage"] = "Trạng thái không hợp lệ.";
                return RedirectToAction("ScholarshipApplications");
            }

            application.Status = parsedStatus;
            application.ReviewNotes = reviewNotes;
            application.ReviewDate = DateTime.Now;
            application.ReviewedBy = User.Identity!.Name;
            application.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn xin học bổng thành '{parsedStatus}'.";
            return RedirectToAction("ScholarshipApplications");
        }

        // POST: Admin/ToggleUserStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("ManageUsers");
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Đã {(user.IsActive ? "kích hoạt" : "vô hiệu hóa")} tài khoản {user.UserName}.";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật trạng thái tài khoản.";
            }

            return RedirectToAction("ManageUsers");
        }

        // GET: Admin/Statistics
        public async Task<IActionResult> Statistics()
        {
            var scholarshipStats = await _context.Scholarships
                .Select(s => new
                {
                    s.Name,
                    s.Quantity,
                    ApplicationCount = s.ScholarshipApplications.Count,
                    ApprovedCount = s.ScholarshipApplications.Count(a => a.Status == ApplicationStatus.Approved),
                    s.Amount
                })
                .ToListAsync();

            var monthlyApplications = await _context.ScholarshipApplications
                .Where(a => a.ApplicationDate >= DateTime.Now.AddMonths(-12))
                .GroupBy(a => new { a.ApplicationDate.Year, a.ApplicationDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            ViewBag.ScholarshipStats = scholarshipStats;
            ViewBag.MonthlyApplications = monthlyApplications;

            return View();
        }

        // POST: Admin/DeleteApplication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteApplication(int applicationId)
        {
            var application = await _context.ScholarshipApplications.FindAsync(applicationId);
            if (application == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn xin học bổng.";
                return RedirectToAction("ScholarshipApplications");
            }
            _context.ScholarshipApplications.Remove(application);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa đơn xin học bổng thành công.";
            return RedirectToAction("ScholarshipApplications");
        }

        // POST: Admin/CreateRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                TempData["ErrorMessage"] = "Tên vai trò không được để trống.";
                return RedirectToAction("ManageRoles");
            }
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                TempData["ErrorMessage"] = "Vai trò đã tồn tại.";
                return RedirectToAction("ManageRoles");
            }
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Tạo vai trò thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join("; ", result.Errors.Select(e => e.Description));
            }
            return RedirectToAction("ManageRoles");
        }

        // POST: Admin/EditRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string roleId, string newRoleName)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy vai trò.";
                return RedirectToAction("ManageRoles");
            }
            role.Name = newRoleName;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Cập nhật vai trò thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join("; ", result.Errors.Select(e => e.Description));
            }
            return RedirectToAction("ManageRoles");
        }

        // POST: Admin/DeleteRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy vai trò.";
                return RedirectToAction("ManageRoles");
            }
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Xóa vai trò thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join("; ", result.Errors.Select(e => e.Description));
            }
            return RedirectToAction("ManageRoles");
        }

        // GET: Admin/AssignRole
        public async Task<IActionResult> AssignRole()
        {
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.ToList();
            ViewBag.Users = users;
            ViewBag.Roles = roles;
            return View();
        }

        // POST: Admin/AssignRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("AssignRole");
            }
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Gán vai trò thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join("; ", result.Errors.Select(e => e.Description));
            }
            return RedirectToAction("AssignRole");
        }

        // GET: Admin/AdvancedStatistics
        public async Task<IActionResult> AdvancedStatistics()
        {
            // Thống kê số lượng đơn theo học bổng
            var byScholarship = await _context.ScholarshipApplications
                .GroupBy(a => a.Scholarship.Name)
                .Select(g => new { Scholarship = g.Key, Count = g.Count() })
                .ToListAsync();

            // Thống kê số lượng đơn theo khoa
            var byFaculty = await _context.ScholarshipApplications
                .Include(a => a.Student)
                .GroupBy(a => a.Student.Faculty)
                .Select(g => new { Faculty = g.Key, Count = g.Count() })
                .ToListAsync();

            // Thống kê số lượng đơn theo trạng thái
            var byStatus = await _context.ScholarshipApplications
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            ViewBag.ByScholarship = byScholarship;
            ViewBag.ByFaculty = byFaculty;
            ViewBag.ByStatus = byStatus;
            return View();
        }

        // GET: Admin/AdvancedSearchApplications
        public async Task<IActionResult> AdvancedSearchApplications(string? step, string? reviewer, string? faculty, string? status, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.ScholarshipApplications
                .Include(a => a.Student)
                .Include(a => a.Scholarship)
                .AsQueryable();

            if (!string.IsNullOrEmpty(step))
                query = query.Where(a => a.ApprovalStep == step);
            if (!string.IsNullOrEmpty(reviewer))
                query = query.Where(a => a.ReviewedBy == reviewer);
            if (!string.IsNullOrEmpty(faculty))
                query = query.Where(a => a.Student.Faculty == faculty);
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ApplicationStatus>(status, out var statusEnum))
                query = query.Where(a => a.Status == statusEnum);
            if (fromDate.HasValue)
                query = query.Where(a => a.ApplicationDate >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(a => a.ApplicationDate <= toDate.Value);

            var applications = await query.OrderByDescending(a => a.ApplicationDate).ToListAsync();

            // Dữ liệu cho dropdown filter
            ViewBag.AllSteps = _context.ScholarshipApplications.Select(a => a.ApprovalStep).Distinct().ToList();
            ViewBag.AllReviewers = _context.ScholarshipApplications.Select(a => a.ReviewedBy).Distinct().ToList();
            ViewBag.AllFaculties = _context.Students.Select(s => s.Faculty).Distinct().ToList();
            ViewBag.AllStatuses = Enum.GetNames(typeof(ApplicationStatus));
            ViewBag.Step = step;
            ViewBag.Reviewer = reviewer;
            ViewBag.Faculty = faculty;
            ViewBag.Status = status;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View(applications);
        }

        // POST: Admin/RemoveRoleFromUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("ManageRoles");
            }
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Đã xóa vai trò '{roleName}' khỏi người dùng {user.UserName}.";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join("; ", result.Errors.Select(e => e.Description));
            }
            return RedirectToAction("ManageRoles");
        }
    }
} 