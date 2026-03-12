namespace Library.Domain
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public string Category { get; set; }
        // IsAvailable indicates whether this book can be loaned out in principle.
        // This does NOT reflect the current loan status.
        // Actual availability is determined by checking active Loans (ReturnDate == null).
        public bool IsAvailable { get; set; }
        public List<Loan> Loans { get; set; } = new();

    }
}
