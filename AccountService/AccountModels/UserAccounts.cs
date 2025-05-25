

namespace AccountService.AccountModels
{
    public class UserAccount
    {
        public int Id { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }

        public byte[]? PasswordHash { get; set; }

        public byte[]? PasswordKey { get; set; }

        public required DateOnly Birthday { get; set; }

        public required List<byte[]> SecurityAnswersHash { get; set; }

        public required List<byte[]> SecurityAnswerKey { get; set; }

    }
}