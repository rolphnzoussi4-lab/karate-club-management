using KarateClubManagement.Business.Models;
using KarateClubManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarateClubManagement.Web.Controllers;

[Authorize(Roles = "Admin,Secretary")]
public class MembersController : Controller
{
    private readonly KarateClubDbContext _context;

    public MembersController(KarateClubDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Members.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim();
            query = query.Where(m => m.FirstName.Contains(q) || m.LastName.Contains(q) || m.Email.Contains(q));
        }

        var members = await query.OrderBy(m => m.LastName).ThenBy(m => m.FirstName).ToListAsync();
        return View(members);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        member.DateOfRegistration = DateTime.Today;
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member is null) return NotFound();
        return View(member);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Member member)
    {
        if (id != member.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(member);

        _context.Update(member);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member is null) return NotFound();
        return View(member);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member is null) return NotFound();

        _context.Members.Remove(member);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
