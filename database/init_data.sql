using KarateClubManagement.Business.Models;
using KarateClubManagement.Business.Services;
using KarateClubManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace KarateClubManagement.Tests;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetDashboardSummaryAsync_ShouldReturnMemberCountAndRevenue()
    {
        var options = new DbContextOptionsBuilder<KarateClubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new KarateClubDbContext(options);

        context.Members.AddRange(
            new Member { FirstName = "Alice", LastName = "Martin", Grade = Grade.White, DateOfRegistration = DateTime.Today, Email = "alice@test.com" },
            new Member { FirstName = "Bob", LastName = "Dupont", Grade = Grade.Blue, DateOfRegistration = DateTime.Today, Email = "bob@test.com" }
        );

        context.Payments.Add(new Payment
        {
            MemberId = 1,
            Amount = 50m,
            FeeType = FeeType.Monthly,
            DueDate = DateTime.Today,
            Status = PaymentStatus.Paid,
            PaidDate = DateTime.Today,
            ReceiptNumber = "RCPT-001"
        });

        await context.SaveChangesAsync();

        var service = new DashboardService(context);
        var dashboard = await service.GetDashboardSummaryAsync();

        Assert.Equal(2, dashboard.TotalMembers);
        Assert.Equal(50m, dashboard.MonthlyRevenue);
    }
}

public class PaymentServiceTests
{
    [Fact]
    public async Task MarkAsPaidAsync_ShouldUpdateStatusAndReceiptNumber()
    {
        var options = new DbContextOptionsBuilder<KarateClubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new KarateClubDbContext(options);

        var payment = new Payment
        {
            MemberId = 1,
            Amount = 120m,
            FeeType = FeeType.Annual,
            DueDate = DateTime.Today,
            Status = PaymentStatus.Pending,
            ReceiptNumber = ""
        };

        context.Payments.Add(payment);
        await context.SaveChangesAsync();

        var service = new PaymentService(context);
        await service.MarkAsPaidAsync(payment.Id);

        var updated = await context.Payments.FindAsync(payment.Id);
        Assert.NotNull(updated);
        Assert.Equal(PaymentStatus.Paid, updated.Status);
        Assert.Contains("RCPT-", updated.ReceiptNumber);
    }
}
