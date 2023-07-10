namespace LegalGenApi.Models
{
    public class LegalInformation
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Document { get; set; }
        public DateTime DateAdded { get; set; }

        public int ResearchBookId { get; set; }
        public ResearchBook ResearchBook { get; set; }

    }
}
