using KarateClubManagement.Business.Models;
using KarateClubManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarateClubManagement.Web.Controllers;

[Authorize(Roles = "Admin,Secretary")]
public class CoachesController : Controller
{
    private readonly KarateClubDbContext _context;

    public CoachesController(KarateClubDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var coaches = await _context.Coaches.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync();
        return View(coaches);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Coach coach)
    {
        if (!ModelState.IsValid)
            return View(coach);

        _context.Coaches.Add(coach);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var coach = await _context.Coaches.FindAsync(id);
        if (coach is null) return NotFound();
        return View(coach);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Coach coach)
    {
        if (id != coach.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(coach);

        _context.Update(coach);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var coach = await _context.Coaches.FindAsync(id);
        if (coach is null) return NotFound();
        return View(coach);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var coach = await _context.Coaches.FindAsync(id);
        if (coach is null) return NotFound();

        _context.Coaches.Remove(coach);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
