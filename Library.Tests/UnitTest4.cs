using Library.Domain;
using Library.MVC.Controllers;
using Library.MVC.Data;
using Library.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UnitTest4
{
    [Fact]
    public async Task OverdueLogic_FlagsActivePastDueLoanAsOverdue()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new ApplicationDbContext(options);

        var today = DateOnly.FromDateTime(DateTime.Today);

        db.Members.Add(new Member
        {
            Id = 1,
            FullName = "Test User",
            Email = "test@example.com",
            Phone = "089 123 4567"
        });

        db.Books.Add(new Book
        {
            Id = 1,
            Title = "Test Book",
            Author = "Author",
            Category = "Test",
            Isbn = "111",
            IsAvailable = true
        });

        db.Loans.Add(new Loan
        {
            MemberId = 1,
            BookId = 1,
            LoanDate = today.AddDays(-20),
            DueDate = today.AddDays(-10),
            ReturnedDate = null 
        });

        await db.SaveChangesAsync();

        var controller = new LoansController(db);

        var result = await controller.Index();
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Loan>>(view.Model);

        var loanFromModel = model.First();

        var overdueFlag = (bool)view.ViewData[$"Overdue_{loanFromModel.Id}"];

        Assert.True(overdueFlag);
    }

}
