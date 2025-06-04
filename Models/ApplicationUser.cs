using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UTEScholarshipSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; } = true;

        // Navigation property - nếu user là sinh viên thì sẽ có thông tin sinh viên
        public int? StudentId { get; set; }
        public virtual Student? Student { get; set; }
    }
} 