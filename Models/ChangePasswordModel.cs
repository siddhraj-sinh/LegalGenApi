
namespace LegalGenApi.Models
{
    public class ChangePasswordModel
    {
        public string Token { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
