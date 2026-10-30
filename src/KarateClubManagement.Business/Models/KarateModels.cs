using KarateClubManagement.Business.Models;
using Microsoft.EntityFrameworkCore;

namespace KarateClubManagement.Data;

public class KarateClubDbContext : DbContext
{
    public KarateClubDbContext(DbContextOptions<KarateClubDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> Members => Set<Member>();
    public DbSet<Coach> Coaches => Set<Coach>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseRegistration> CourseRegistrations => Set<CourseRegistration>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<GradeHistoryEntry> GradeHistory => Set<GradeHistoryEntry>();
    public DbSet<GradeExam> GradeExams => Set<GradeExam>();
    public DbSet<GradeExamRegistration> GradeExamRegistrations => Set<GradeExamRegistration>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Member>()
            .HasMany(m => m.GradeHistory)
            .WithOne(h => h.Member)
            .HasForeignKey(h => h.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Member>()
            .HasMany(m => m.CourseRegistrations)
            .WithOne(r => r.Member)
            .HasForeignKey(r => r.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
            .HasMany(c => c.Registrations)
            .WithOne(r => r.Course)
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Coach>()
            .HasMany(c => c.Courses)
            .WithOne(c => c.Coach)
            .HasForeignKey(c => c.CoachId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CourseRegistration>()
            .HasIndex(r => new { r.CourseId, r.MemberId })
            .IsUnique();

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Member)
            .WithMany(m => m.Payments)
            .HasForeignKey(p => p.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GradeExamRegistration>()
            .HasOne(g => g.Member)
            .WithMany(m => m.GradeExamRegistrations)
            .HasForeignKey(g => g.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

