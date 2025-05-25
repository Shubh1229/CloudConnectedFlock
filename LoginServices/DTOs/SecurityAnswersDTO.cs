

namespace LoginServices.DTOs
{
    public class SecurityAnswersDTO
    {
        public required string Username { get; set; }
        public required string Answer { get; set; }
        public required int SecurityQuestion { get; set; }
    }
}