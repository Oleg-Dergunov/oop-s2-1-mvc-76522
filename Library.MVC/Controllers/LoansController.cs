using Library.Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class LoansController : Controller
{
    private readonly ApplicationDbContext _context;

    public LoansController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var loans = await _context.Loans
            .Include(l => l.Member)
            .Include(l => l.Book)
            .ToListAsync();

        return View(loans);
    }

    public async Task<IActionResult> Create()
    {
        var members = await _context.Members.ToListAsync();

        var availableBooks = await _context.Books
            .Where(b => b.IsAvailable)
            .Where(b => !_context.Loans.Any(l => l.BookId == b.Id && l.ReturnedDate == null))
            .ToListAsync();

        ViewBag.Members = new SelectList(members, "Id", "FullName");
        ViewBag.Books = new SelectList(availableBooks, "Id", "Title");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int memberId, int bookId)
    {
        bool isOnLoan = await _context.Loans
            .AnyAsync(l => l.BookId == bookId && l.ReturnedDate == null);

        if (isOnLoan)
        {
            ModelState.AddModelError("", "This book is already on loan.");
            return await Create();
        }

        var today = DateOnly.FromDateTime(DateTime.Now);

        var loan = new Loan
        {
            MemberId = memberId,
            BookId = bookId,
            LoanDate = today,
            DueDate = today.AddDays(14),
            ReturnedDate = null
        };

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Return(int id)
    {
        var loan = await _context.Loans.FindAsync(id);
        if (loan == null) return NotFound();

        loan.ReturnedDate = DateOnly.FromDateTime(DateTime.Now);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

}
