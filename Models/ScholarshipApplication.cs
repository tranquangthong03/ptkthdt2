using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTEScholarshipSystem.Models
{
    public class ScholarshipApplication
    {
        [Key]
        public int ApplicationId { get; set; }

        [Required]
        [Display(Name = "Mã sinh viên")]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "Mã học bổng")]
        [ForeignKey("Scholarship")]
        public int ScholarshipId { get; set; }

        [Required]
        [StringLength(1000)]
        [Display(Name = "Lý do xin học bổng")]
        public string ApplicationReason { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Thành tích nổi bật")]
        public string? Achievements { get; set; }

        [StringLength(500)]
        [Display(Name = "Tình hình kinh tế gia đình")]
        public string? FamilyEconomicStatus { get; set; }

        [Display(Name = "GPA tại thời điểm nộp đơn")]
        [Range(0.0, 4.0)]
        public double GPAAtApplication { get; set; }

        [Display(Name = "Ngày nộp đơn")]
        public DateTime ApplicationDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Trạng thái")]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        [StringLength(500)]
        [Display(Name = "Ghi chú từ ban tuyển sinh")]
        public string? ReviewNotes { get; set; }

        [Display(Name = "Ngày xét duyệt")]
        public DateTime? ReviewDate { get; set; }

        [Display(Name = "Người xét duyệt")]
        public string? ReviewedBy { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Tiến trình xử lý")]
        public string? ProgressSteps { get; set; } // JSON lưu các bước tiến trình

        [Display(Name = "Bước duyệt hiện tại")]
        public string? ApprovalStep { get; set; } // VD: PreReview, Faculty, Training, Board

        [Display(Name = "Lịch sử duyệt")]
        public string? ApprovalHistory { get; set; } // JSON lưu các bước duyệt

        // Navigation properties
        public virtual Student Student { get; set; } = null!;
        public virtual Scholarship Scholarship { get; set; } = null!;
    }

    public enum ApplicationStatus
    {
        [Display(Name = "Đang chờ xét duyệt")]
        Pending = 1,
        [Display(Name = "Đang xem xét")]
        UnderReview = 2,
        [Display(Name = "Đã chấp nhận")]
        Approved = 3,
        [Display(Name = "Đã từ chối")]
        Rejected = 4,
        [Display(Name = "Đã hủy")]
        Cancelled = 5
    }

    public class ProgressStep
    {
        public string Status { get; set; } = string.Empty; // Ví dụ: "Đã nộp", "Đang xét duyệt", ...
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? Note { get; set; } // Ghi chú từng bước
    }

    public class ApprovalStepHistory
    {
        public string Step { get; set; } = string.Empty; // VD: PreReview, Faculty, Training, Board
        public string? ReviewedBy { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? Note { get; set; }
        public string? Status { get; set; } // Approved/Rejected/Pending
    }
} 