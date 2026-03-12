using Bogus;
using Library.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Data
{
    public static class SeedData
    {
        private static readonly string[] BookCategories =
        {
            "Fantasy",
            "Science Fiction",
            "Mystery",
            "Thriller",
            "Romance",
            "Non-Fiction",
            "Biography",
            "History",
            "Philosophy",
            "Programming"
        };

        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            if (await context.Books.AnyAsync() ||
                await context.Members.AnyAsync() ||
                await context.Loans.AnyAsync())
            {
                return;
            }

            // Books

            var bookFaker = new Faker<Book>()
                .RuleFor(b => b.Title, f => f.Lorem.Sentence(3))
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.Isbn, f => f.Random.Replace("###-##########"))
                .RuleFor(b => b.Category, f => f.PickRandom(BookCategories))
                .RuleFor(b => b.IsAvailable, f => f.Random.Bool(0.9f)); // 90% available

            var books = bookFaker.Generate(20);
            context.Books.AddRange(books);

            // Members
            var memberFaker = new Faker<Member>()
                .RuleFor(m => m.FullName, f => f.Name.FullName())
                .RuleFor(m => m.Email, f => f.Internet.Email())
                .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber("08# ### ####"));

            var members = memberFaker.Generate(10);
            context.Members.AddRange(members);

            await context.SaveChangesAsync();

            // Loans
            var loanFaker = new Faker<Loan>()
                .RuleFor(l => l.MemberId, f => f.PickRandom(members).Id)
                .RuleFor(l => l.BookId, f => f.PickRandom(books).Id)
                .RuleFor(l => l.LoanDate, f => DateOnly.FromDateTime(f.Date.Past(1)))
                .RuleFor(l => l.DueDate, (f, l) => l.LoanDate.AddDays(14))
                .RuleFor(l => l.ReturnedDate, (f, l) =>
                {
                    // 40% active, 40% returned, 20% overdue
                    var roll = f.Random.Int(1, 100);

                    if (roll <= 40)
                        return null; 

                    if (roll <= 80)
                        return l.LoanDate.AddDays(f.Random.Int(1, 20));

                    return null; 
                });

            var loans = loanFaker.Generate(15);
            context.Loans.AddRange(loans);

            await context.SaveChangesAsync();
        }
    }
}
