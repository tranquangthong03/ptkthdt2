using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTEScholarshipSystem.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Mã số sinh viên")]
        public string StudentCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Giới tính")]
        public Gender Gender { get; set; } = Gender.Male;

        [Required]
        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Khóa học")]
        public int AcademicYear { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Ngành học")]
        public string Major { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Lớp")]
        public string Class { get; set; } = string.Empty;

        [NotMapped]
        [Display(Name = "Tên lớp")]
        public string ClassName 
        { 
            get { return Class; } 
        }

        [Required]
        [StringLength(100)]
        [Display(Name = "Khoa")]
        public string Faculty { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Điểm GPA")]
        [Range(0.0, 4.0)]
        public double GPA { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<ScholarshipApplication> ScholarshipApplications { get; set; } = new List<ScholarshipApplication>();
        
        // Liên kết với ApplicationUser
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
} 