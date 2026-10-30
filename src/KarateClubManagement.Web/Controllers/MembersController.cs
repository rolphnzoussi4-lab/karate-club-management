using KarateClubManagement.Business.Models;
using KarateClubManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarateClubManagement.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly KarateClubDbContext _context;

    public DashboardController(KarateClubDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var members = await _context.Members.ToListAsync();
        var totalMembers = members.Count;

        var gradeDistribution = members
            .GroupBy(m => m.Grade)
            .Select(g => new GradeDistributionItem { Grade = g.Key.ToString(), Count = g.Count() })
            .OrderBy(g => g.Grade)
            .ToList();

        var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var monthlyRevenue = await _context.Payments
            .Where(p => p.PaidDate.HasValue && p.PaidDate.Value >= currentMonth)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

        var upcomingCourses = await _context.Courses
            .Include(c => c.Coach)
            .Where(c => c.Date >= DateTime.Today)
            .OrderBy(c => c.Date)
            .Take(5)
            .ToListAsync();

        var latePayments = await _context.Payments
            .Include(p => p.Member)
            .Where(p => p.Status == PaymentStatus.Late || (p.Status == PaymentStatus.Pending && p.DueDate < DateTime.Today))
            .OrderBy(p => p.DueDate)
            .Take(10)
            .ToListAsync();

        var viewModel = new DashboardSummaryViewModel
        {
            TotalMembers = totalMembers,
            MonthlyRevenue = monthlyRevenue,
            GradeDistribution = gradeDistribution,
            UpcomingCourses = upcomingCourses,
            LatePayments = latePayments
        };

        return View(viewModel);
    }
}
