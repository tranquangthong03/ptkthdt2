using System.ComponentModel.DataAnnotations;

namespace UTEScholarshipSystem.Models.ViewModels
{
    public class CreateStudentAccountViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sinh viên")]
        [Display(Name = "Sinh viên")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Thông tin sinh viên để hiển thị
        [Required(ErrorMessage = "Vui lòng nhập mã sinh viên")]
        [StringLength(20, ErrorMessage = "Mã sinh viên không được vượt quá 20 ký tự")]
        [Display(Name = "Mã sinh viên")]
        public string StudentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [Display(Name = "Giới tính")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập khoa")]
        [StringLength(100, ErrorMessage = "Khoa không được vượt quá 100 ký tự")]
        [Display(Name = "Khoa")]
        public string Faculty { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập ngành học")]
        [StringLength(100, ErrorMessage = "Ngành học không được vượt quá 100 ký tự")]
        [Display(Name = "Ngành học")]
        public string Major { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lớp")]
        [StringLength(50, ErrorMessage = "Lớp không được vượt quá 50 ký tự")]
        [Display(Name = "Lớp")]
        public string Class { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập GPA")]
        [Range(0.0, 4.0, ErrorMessage = "GPA phải từ 0.0 đến 4.0")]
        [Display(Name = "GPA")]
        public double GPA { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;
    }
} 