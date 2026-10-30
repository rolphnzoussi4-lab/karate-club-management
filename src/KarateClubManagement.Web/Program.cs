using KarateClubManagement.Business.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KarateClubManagement.Data;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}

public class KarateClubDbContext : DbContext
{
    public KarateClubDbContext(DbContextOptions<KarateClubDbContext> options) : base(options)
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
            .WithOne(g => g.Member)
            .HasForeignKey(g => g.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Member>()
            .HasMany(m => m.CourseRegistrations)
            .WithOne(c => c.Member)
            .HasForeignKey(c => c.MemberId)
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
            .HasOne(r => r.Member)
            .WithMany(m => m.GradeExamRegistrations)
            .HasForeignKey(r => r.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { "Admin", "Coach", "Secretary" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var defaultUsers = new[]
        {
            new { Email = "admin@karateclub.com", Password = "Admin123!", Role = "Admin" },
            new { Email = "coach@karateclub.com", Password = "Coach123!", Role = "Coach" },
            new { Email = "secretary@karateclub.com", Password = "Secret123!", Role = "Secretary" }
        };

        foreach (var userSeed in defaultUsers)
        {
            var user = await userManager.FindByEmailAsync(userSeed.Email);
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = userSeed.Email,
                    Email = userSeed.Email,
                    FirstName = userSeed.Role,
                    LastName = "Club"
                };

                var result = await userManager.CreateAsync(user, userSeed.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, userSeed.Role);
                }
            }
        }

        var context = services.GetRequiredService<KarateClubDbContext>();

        if (!context.Members.Any())
        {
            context.Members.AddRange(
                new Member { FirstName = "Alice", LastName = "Martin", DateOfBirth = new DateTime(1998, 5, 21), Gender = Gender.Female, Address = "12 rue des Pins", PhoneNumber = "0601020304", Email = "alice@club.fr", Grade = Grade.Blue, DateOfRegistration = DateTime.Today.AddDays(-120) },
                new Member { FirstName = "Jean", LastName = "Dubois", DateOfBirth = new DateTime(2005, 2, 11), Gender = Gender.Male, Address = "5 avenue du Parc", PhoneNumber = "0611223344", Email = "jean@club.fr", Grade = Grade.Green, DateOfRegistration = DateTime.Today.AddDays(-60) },
                new Member { FirstName = "Sophie", LastName = "Legrand", DateOfBirth = new DateTime(1995, 9, 30), Gender = Gender.Female, Address = "8 boulevard Saint-Pierre", PhoneNumber = "0622334455", Email = "sophie@club.fr", Grade = Grade.Brown, DateOfRegistration = DateTime.Today.AddDays(-30) }
            );
        }

        if (!context.Coaches.Any())
        {
            context.Coaches.AddRange(
                new Coach { FirstName = "Pierre", LastName = "Morel", Speciality = "Kata", Grade = Grade.Black, PhoneNumber = "0644556677", Email = "pierre@club.fr", DateOfHire = DateTime.Today.AddYears(-4) },
                new Coach { FirstName = "Nadia", LastName = "Lemaire", Speciality = "Combat", Grade = Grade.Black, PhoneNumber = "0655667788", Email = "nadia@club.fr", DateOfHire = DateTime.Today.AddYears(-2) }
            );
        }

        if (!context.Courses.Any())
        {
            var coach = context.Coaches.First();
            context.Courses.AddRange(
                new Course { Name = "Kata débutants", Date = DateTime.Today.AddDays(2), StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 0, 0), Room = "Salle A", CoachId = coach.Id, Description = "Cours de kata pour débutants" },
                new Course { Name = "Combat intermédiaire", Date = DateTime.Today.AddDays(4), StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20, 0, 0), Room = "Salle B", CoachId = coach.Id, Description = "Travail technique et combat" }
            );
        }

        if (!context.Payments.Any())
        {
            var member = context.Members.First();
            context.Payments.AddRange(
                new Payment { MemberId = member.Id, FeeType = FeeType.Monthly, Amount = 40m, DueDate = DateTime.Today.AddDays(-10), Status = PaymentStatus.Pending, Notes = "Cotisation mensuelle" },
                new Payment { MemberId = member.Id, FeeType = FeeType.Annual, Amount = 250m, DueDate = DateTime.Today.AddDays(5), Status = PaymentStatus.Pending, Notes = "Abonnement annuel" }
            );
        }

        await context.SaveChangesAsync();
    }
}
