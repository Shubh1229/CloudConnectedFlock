
namespace ProfileService.DTOs
{
    public class ProfileDTO
    {
        public required string Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? PersonalLinks { get; set; }
        public string? ResumeFilePath { get; set; }
        public string? ProfilePicturePath { get; set; }
    }
}