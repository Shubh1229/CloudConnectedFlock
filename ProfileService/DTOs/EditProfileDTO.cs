namespace ProfileService.DTOs
{
    public class EditUserProfileDTO
    {
        public required string Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Birthday { get; set; }

    }
}