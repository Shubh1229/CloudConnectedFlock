namespace LoginServices.DTOs
{
    public class AccountRegistration
    {
        public required string Username;

        public required string Password;

        public required string Email;

        public required DateOnly Birthday;
    }
}
