using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Domain;
using Library.MVC.Data;
using Library.MVC.ViewModels;

namespace Library.MVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string? search, string? category, string? availability, string? sortOrder)
        {
            var query = _context.Books.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b =>
                    b.Title.Contains(search) ||
                    b.Author.Contains(search));
            }

            // Category filter
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(b => b.Category == category);
            }

            // Availability filter
            // "Available" → the book can be borrowed (IsAvailable == true) and is not currently on hand
            // "Inavailable" → the book is not borrowed at all (IsAvailable == false), regardless of Loans
            // "OnLoan" → the book is currently on hand (there is a Loan with ReturnDate == null)
            if (!string.IsNullOrWhiteSpace(availability))
            {
                if (availability == "Available")
                {
                    query = query.Where(b =>
                        b.IsAvailable &&
                        !_context.Loans.Any(l => l.BookId == b.Id && l.ReturnedDate == null));
                }

                if (availability == "Inavailable")
                {
                    query = query.Where(b =>
                        !b.IsAvailable);
                }

                if (availability == "OnLoan")
                {
                    query = query.Where(b =>
                        _context.Loans.Any(l => l.BookId == b.Id && l.ReturnedDate == null));
                }
            }


            // Sorting
            query = sortOrder switch
            {
                "title_asc" => query.OrderBy(b => b.Title),
                "title_desc" => query.OrderByDescending(b => b.Title),
                _ => query.OrderBy(b => b.Id)
            };

            var vm = new BookFilterViewModel
            {
                Search = search,
                Category = category,
                Availability = availability,
                SortOrder = sortOrder,
                Categories = await _context.Books
                    .Select(b => b.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync(),
                Books = await query.ToListAsync()
            };

            return View(vm);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Isbn,Category,IsAvailable")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Isbn,Category,IsAvailable")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }


    }
}
