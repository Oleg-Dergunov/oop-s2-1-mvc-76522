using Library.Domain;
namespace Library.MVC.ViewModels;
public class BookFilterViewModel
{
    public string? Search { get; set; }
    public string? Category { get; set; }
    public string? Availability { get; set; }
    public string? SortOrder { get; set; }

    public List<string> Categories { get; set; } = new();
    public IEnumerable<Book> Books { get; set; } = Enumerable.Empty<Book>();
}
