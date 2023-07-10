namespace LegalGenApi.Models
{
    public class ChatInteraction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }

        public User User { get; set; }

    }
}
