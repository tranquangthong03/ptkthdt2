using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UTEScholarshipSystem.Models;
using UTEScholarshipSystem.Models.ViewModels;

namespace UTEScholarshipSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ScholarshipDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ScholarshipDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: Account/Login
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if (user != null && user.IsActive)
                    {
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            // Redirect based on role
                            var roles = await _userManager.GetRolesAsync(user);
                            if (roles.Contains("Admin"))
                            {
                                return RedirectToAction("Dashboard", "Admin");
                            }
                            else if (roles.Contains("Student"))
                            {
                                return RedirectToAction("Dashboard", "Student");
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                    else
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError(string.Empty, "Tài khoản đã bị vô hiệu hóa.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/AccessDenied
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Account/CreateStudentAccount
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStudentAccount()
        {
            // Get students who don't have accounts yet
            var studentsWithoutAccounts = await _context.Students
                .Where(s => s.ApplicationUser == null)
                .Select(s => new SelectListItem
                {
                    Value = s.StudentId.ToString(),
                    Text = $"{s.StudentCode} - {s.FullName}"
                })
                .ToListAsync();

            ViewBag.Students = studentsWithoutAccounts;
            
            // Initialize model with default values
            var model = new CreateStudentAccountViewModel();
            return View(model);
        }

        // POST: Account/CreateStudentAccount
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudentAccount(CreateStudentAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra mã sinh viên, email, username đã tồn tại chưa
                var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.StudentCode == model.StudentCode);
                if (existingStudent != null)
                {
                    ModelState.AddModelError("StudentCode", "Mã sinh viên đã tồn tại.");
                    return View(model);
                }
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }
                existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(model);
                }
                // Tạo mới sinh viên
                var student = new Student
                {
                    StudentCode = model.StudentCode,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Faculty = model.Faculty,
                    Major = model.Major,
                    Class = model.Class,
                    GPA = model.GPA,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Email = model.Email
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                // Tạo tài khoản user
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    StudentId = student.StudentId,
                    EmailConfirmed = true,
                    IsActive = true
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");
                    TempData["SuccessMessage"] = $"Tạo tài khoản sinh viên {student.FullName} thành công.";
                    return RedirectToAction("ManageStudentAccounts");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: Account/ManageStudentAccounts
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageStudentAccounts()
        {
            var studentAccounts = await _userManager.GetUsersInRoleAsync("Student");
            var accountsWithStudentInfo = new List<object>();

            foreach (var account in studentAccounts)
            {
                var student = await _context.Students.FindAsync(account.StudentId);
                accountsWithStudentInfo.Add(new
                {
                    User = account,
                    Student = student
                });
            }

            return View(accountsWithStudentInfo);
        }

        // POST: Account/ResetPassword
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string userId, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin.";
                return RedirectToAction("ManageStudentAccounts");
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToAction("ManageStudentAccounts");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("ManageStudentAccounts");
            }

            // Remove current password and set new one
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Đã đặt lại mật khẩu cho tài khoản {user.UserName} thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Lỗi khi đặt lại mật khẩu: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction("ManageStudentAccounts");
        }

        private async Task LoadStudentsDropdown()
        {
            var studentsWithoutAccounts = await _context.Students
                .Where(s => s.ApplicationUser == null)
                .Select(s => new SelectListItem
                {
                    Value = s.StudentId.ToString(),
                    Text = $"{s.StudentCode} - {s.FullName}"
                })
                .ToListAsync();

            ViewBag.Students = studentsWithoutAccounts;
        }
    }
} 