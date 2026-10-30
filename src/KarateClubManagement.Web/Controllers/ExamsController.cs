using KarateClubManagement.Business.Models;
using KarateClubManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KarateClubManagement.Web.Controllers;

[Authorize(Roles = "Admin,Coach,Secretary")]
public class CoursesController : Controller
{
    private readonly KarateClubDbContext _context;

    public CoursesController(KarateClubDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses
            .Include(c => c.Coach)
            .OrderBy(c => c.Date)
            .ToListAsync();

        return View(courses);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Coaches = new SelectList(await _context.Coaches.ToListAsync(), "Id", "FirstName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (!ModelState.IsValid)
            return View(course);

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Register(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Registrations)
            .ThenInclude(r => r.Member)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course is null)
            return NotFound();

        ViewBag.Members = await _context.Members.ToListAsync();
        return View(course);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int id, int memberId)
    {
        var exists = await _context.CourseRegistrations
            .AnyAsync(r => r.CourseId == id && r.MemberId == memberId);

        if (!exists)
        {
            _context.CourseRegistrations.Add(new CourseRegistration
            {
                CourseId = id,
                MemberId = memberId,
                RegisteredAt = DateTime.Now,
                Status = "Registered"
            });
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Register), new { id });
    }

    public async Task<IActionResult> Attendance(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Registrations)
            .ThenInclude(r => r.Member)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course is null)
            return NotFound();

        return View(course);
    }
}
