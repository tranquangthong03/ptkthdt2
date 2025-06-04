using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTEScholarshipSystem.Models
{
    public class Scholarship
    {
        [Key]
        public int ScholarshipId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên học bổng")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Số tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Số lượng suất")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "GPA yêu cầu tối thiểu")]
        [Range(0.0, 4.0)]
        public double MinGPA { get; set; }

        [Required]
        [Display(Name = "Ngày bắt đầu ứng tuyển")]
        [DataType(DataType.Date)]
        public DateTime ApplicationStartDate { get; set; }

        [Required]
        [Display(Name = "Ngày kết thúc ứng tuyển")]
        [DataType(DataType.Date)]
        public DateTime ApplicationEndDate { get; set; }

        [Required]
        [Display(Name = "Năm học áp dụng")]
        public int AcademicYear { get; set; }

        [StringLength(200)]
        [Display(Name = "Yêu cầu khác")]
        public string? AdditionalRequirements { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public ScholarshipStatus Status { get; set; } = ScholarshipStatus.Active;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<ScholarshipApplication> ScholarshipApplications { get; set; } = new List<ScholarshipApplication>();
    }

    public enum ScholarshipStatus
    {
        [Display(Name = "Đang mở")]
        Active = 1,
        [Display(Name = "Đã đóng")]
        Closed = 2,
        [Display(Name = "Đã hoàn thành")]
        Completed = 3
    }
} 