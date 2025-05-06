namespace LoginServices.DTOs
{
    public class AccountRegistration
    {
        public required string Username { get; set; }

        public required string Password {  get; set; }

        public required string Email {  get; set; }

        public required DateOnly Birthday {  get; set; }
    }
}
