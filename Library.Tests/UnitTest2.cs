using Library.Domain;
using Library.MVC.Controllers;
using Library.MVC.Data;
using Library.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UnitTest2
{
    [Fact]
    public async Task ReturnedLoan_MakesBookAvailableForNewLoan()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new ApplicationDbContext(options);

        var book = new Book
        {
            Title = "Test Book",
            Author = "Author",
            Category = "Fiction",
            Isbn = "1234567890",
            IsAvailable = true
        };

        var member = new Member
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Phone = "123456"
        };

        db.Books.Add(book);
        db.Members.Add(member);

        var loan = new Loan
        {
            BookId = book.Id,
            MemberId = member.Id,
            LoanDate = DateOnly.FromDateTime(DateTime.Today),
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(14)),
            ReturnedDate = null
        };

        db.Loans.Add(loan);
        await db.SaveChangesAsync();

        var controller = new LoansController(db);

        await controller.Return(loan.Id);

        Assert.NotNull(db.Loans.First().ReturnedDate);

        var result = await controller.Create(member.Id, book.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        Assert.Equal(2, db.Loans.Count());

        Assert.Null(db.Loans.OrderBy(l => l.Id).Last().ReturnedDate);
    }

    
}
