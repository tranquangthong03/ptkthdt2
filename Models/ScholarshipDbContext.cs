using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UTEScholarshipSystem.Models;

namespace UTEScholarshipSystem.Models
{
    public class ScholarshipDbContext : IdentityDbContext<ApplicationUser>
    {
        public ScholarshipDbContext(DbContextOptions<ScholarshipDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Scholarship> Scholarships { get; set; }
        public DbSet<ScholarshipApplication> ScholarshipApplications { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ApplicationUser entity
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                
                // Configure relationship with Student
                entity.HasOne(e => e.Student)
                      .WithOne(s => s.ApplicationUser)
                      .HasForeignKey<ApplicationUser>(e => e.StudentId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Student entity
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);
                entity.HasIndex(e => e.StudentCode).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.Property(e => e.StudentCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Major).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Class).IsRequired().HasMaxLength(50);
                entity.Property(e => e.GPA).HasColumnType("decimal(3,2)");
            });

            // Configure Scholarship entity
            modelBuilder.Entity<Scholarship>(entity =>
            {
                entity.HasKey(e => e.ScholarshipId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MinGPA).HasColumnType("decimal(3,2)");
                entity.Property(e => e.AdditionalRequirements).HasMaxLength(200);
            });

            // Configure ScholarshipApplication entity and relationships
            modelBuilder.Entity<ScholarshipApplication>(entity =>
            {
                entity.HasKey(e => e.ApplicationId);
                entity.Property(e => e.ApplicationReason).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Achievements).HasMaxLength(500);
                entity.Property(e => e.FamilyEconomicStatus).HasMaxLength(500);
                entity.Property(e => e.GPAAtApplication).HasColumnType("decimal(3,2)");
                entity.Property(e => e.ReviewNotes).HasMaxLength(500);
                entity.Property(e => e.ReviewedBy).HasMaxLength(100);

                // Configure relationships
                entity.HasOne(e => e.Student)
                      .WithMany(s => s.ScholarshipApplications)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Scholarship)
                      .WithMany(s => s.ScholarshipApplications)
                      .HasForeignKey(e => e.ScholarshipId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Ensure one student can only apply once for each scholarship
                entity.HasIndex(e => new { e.StudentId, e.ScholarshipId }).IsUnique();
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2024, 1, 1);
            
            // Seed Scholarships
            modelBuilder.Entity<Scholarship>().HasData(
                new Scholarship
                {
                    ScholarshipId = 1,
                    Name = "Học bổng Khuyến khích học tập",
                    Description = "Học bổng dành cho sinh viên có kết quả học tập xuất sắc",
                    Amount = 2000000,
                    Quantity = 50,
                    MinGPA = 3.2,
                    ApplicationStartDate = new DateTime(2024, 9, 1),
                    ApplicationEndDate = new DateTime(2024, 10, 15),
                    AcademicYear = 2024,
                    Status = ScholarshipStatus.Active,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Scholarship
                {
                    ScholarshipId = 2,
                    Name = "Học bổng Hỗ trợ sinh viên khó khăn",
                    Description = "Học bổng dành cho sinh viên có hoàn cảnh khó khăn",
                    Amount = 1500000,
                    Quantity = 100,
                    MinGPA = 2.5,
                    ApplicationStartDate = new DateTime(2024, 9, 1),
                    ApplicationEndDate = new DateTime(2024, 11, 30),
                    AcademicYear = 2024,
                    AdditionalRequirements = "Có giấy xác nhận hộ nghèo hoặc cận nghèo",
                    Status = ScholarshipStatus.Active,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Scholarship
                {
                    ScholarshipId = 3,
                    Name = "Học bổng Tài năng",
                    Description = "Học bổng dành cho sinh viên có tài năng đặc biệt",
                    Amount = 5000000,
                    Quantity = 10,
                    MinGPA = 3.7,
                    ApplicationStartDate = new DateTime(2024, 8, 15),
                    ApplicationEndDate = new DateTime(2024, 9, 30),
                    AcademicYear = 2024,
                    AdditionalRequirements = "Có giải thưởng cấp quốc gia hoặc quốc tế",
                    Status = ScholarshipStatus.Active,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );

            // Seed sample students
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    StudentId = 1,
                    StudentCode = "20110001",
                    FullName = "Nguyễn Văn An",
                    Email = "20110001@student.hcmute.edu.vn",
                    PhoneNumber = "0123456789",
                    DateOfBirth = new DateTime(2003, 1, 15),
                    Address = "123 Võ Văn Ngân, Thủ Đức, TP.HCM",
                    AcademicYear = 2021,
                    Major = "Công nghệ thông tin",
                    Class = "ĐHCNTT21A",
                    GPA = 3.5,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Student
                {
                    StudentId = 2,
                    StudentCode = "20110002",
                    FullName = "Trần Thị Bình",
                    Email = "20110002@student.hcmute.edu.vn",
                    PhoneNumber = "0987654321",
                    DateOfBirth = new DateTime(2003, 3, 20),
                    Address = "456 Nguyễn Văn Cừ, Quận 5, TP.HCM",
                    AcademicYear = 2021,
                    Major = "Kỹ thuật điện tử",
                    Class = "ĐHKĐT21B",
                    GPA = 3.8,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );
        }
    }
} 