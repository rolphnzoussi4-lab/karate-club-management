using KarateClubManagement.Business.Models;
using KarateClubManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarateClubManagement.Web.Controllers;

[Authorize(Roles = "Admin,Secretary")]
public class PaymentsController : Controller
{
    private readonly KarateClubDbContext _context;

    public PaymentsController(KarateClubDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var payments = await _context.Payments
            .Include(p => p.Member)
            .OrderByDescending(p => p.DueDate)
            .ToListAsync();

        return View(payments);
    }

    public async Task<IActionResult> Create(int memberId)
    {
        var member = await _context.Members.FindAsync(memberId);
        if (member is null) return NotFound();

        var payment = new Payment
        {
            MemberId = memberId,
            Member = member,
            FeeType = FeeType.Monthly,
            Amount = 40m,
            DueDate = DateTime.Today.AddDays(15),
            Status = PaymentStatus.Pending,
            Notes = "Cotisation"
        };

        return View(payment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Payment payment)
    {
        if (!ModelState.IsValid)
        {
            return View(payment);
        }

        payment.ReceiptNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{payment.MemberId}";
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPaid(int paymentId)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment is null) return NotFound();

        payment.Status = PaymentStatus.Paid;
        payment.PaidDate = DateTime.Today;
        payment.ReceiptNumber = $"RCPT-{DateTime.UtcNow:yyyyMMddHHmmss}-{payment.Id}";

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Receipt(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.Member)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment is null) return NotFound();

        var pdfBytes = ReceiptPdfGenerator.Generate(payment);
        return File(pdfBytes, "application/pdf", $"recu-{payment.Member.FirstName}-{payment.Id}.pdf");
    }
}
