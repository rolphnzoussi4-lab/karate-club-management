using KarateClubManagement.Business.Models;
using KarateClubManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarateClubManagement.Web.Controllers;

[Authorize(Roles = "Admin,Coach,Secretary")]
public class ExamsController : Controller
{
    private readonly KarateClubDbContext _context;

    public ExamsController(KarateClubDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var exams = await _context.GradeExams.OrderBy(e => e.ExamDate).ToListAsync();
        return View(exams);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GradeExam exam)
    {
        if (!ModelState.IsValid)
            return View(exam);

        _context.GradeExams.Add(exam);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Candidates(int id)
    {
        var exam = await _context.GradeExams
            .Include(e => e.Registrations)
            .ThenInclude(r => r.Member)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (exam is null) return NotFound();

        ViewBag.Members = await _context.Members.OrderBy(m => m.LastName).ToListAsync();
        return View(exam);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterCandidate(int examId, int memberId)
    {
        var exists = await _context.GradeExamRegistrations.AnyAsync(r => r.ExamId == examId && r.MemberId == memberId);
        if (!exists)
        {
            _context.GradeExamRegistrations.Add(new GradeExamRegistration
            {
                ExamId = examId,
                MemberId = memberId,
                RegistrationDate = DateTime.Today
            });
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Candidates), new { id = examId });
    }
}
