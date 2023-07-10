namespace LegalGenApi.Models
{
    public class SearchQuery
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Keywords { get; set; }
        public DateTime DateTime { get; set; }

        public User User { get; set; }
    }
}
