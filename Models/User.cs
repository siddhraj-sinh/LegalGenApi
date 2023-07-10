namespace LegalGenApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }
        public string ContactDetails { get; set; }

        public string ResetToken { get; set; }
        public ICollection<ResearchBook> ResearchBooks { get; set; }
        public ICollection<SearchQuery> SearchQueries { get; set; }
        public ICollection<ChatInteraction> ChatInteractions { get; set; }



    }
}
