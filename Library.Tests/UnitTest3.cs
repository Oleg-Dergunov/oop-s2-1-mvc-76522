using Library.Domain;
using Library.MVC.Controllers;
using Library.MVC.Data;
using Library.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UnitTest3
{
    [Fact]
    public async Task BookSearch_ReturnsExpectedMatches()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new ApplicationDbContext(options);

        db.Books.AddRange(
            new Book
            {
                Title = "The Hobbit",
                Author = "J.R.R. Tolkien",
                Category = "Fantasy",
                Isbn = "222",
                IsAvailable = true
            },
            new Book
            {
                Title = "Harry Potter",
                Author = "J.K. Rowling",
                Category = "Fantasy",
                Isbn = "111",
                IsAvailable = true
            },
            new Book
            {
                Title = "C# Programming",
                Author = "John Sharp",
                Category = "Education",
                Isbn = "333",
                IsAvailable = true
            }
        );

        await db.SaveChangesAsync();

        var controller = new BooksController(db);

        var result = await controller.Index("Harry", null, null, null);

        var view = Assert.IsType<ViewResult>(result);

        var vm = Assert.IsType<BookFilterViewModel>(view.Model);

        Assert.Single(vm.Books);

        Assert.Equal("Harry Potter", vm.Books.First().Title);
    }
}
