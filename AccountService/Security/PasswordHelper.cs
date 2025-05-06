using System.Security.Cryptography;
using System.Text;

namespace AccountService.Security
{
    public static class PasswordHelper
    {
        public static (byte[] Hash, byte[] Key) HashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            var key = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return (hash, key);
        }

        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedKey)
        {
            using var hmac = new HMACSHA512(storedKey);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        } 
    }
}
