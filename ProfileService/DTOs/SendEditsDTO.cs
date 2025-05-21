namespace ProfileService.DTOs
{
    public class SendEditsDTO
    {
        public required string Username { get; set; }
        public string? NewUsername { get; set; }
        public string? Password { get; set; }
        public required string Email { get; set; }
        public required DateOnly Birthday { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? PersonalLinks { get; set; }
        public string? ResumeFilePath { get; set; }
        public string? ProfilePicturePath { get; set; }
        public required bool Updated { get; set; }

    }
}