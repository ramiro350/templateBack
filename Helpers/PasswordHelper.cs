using System.Security.Cryptography;

namespace ArqPay.Helpers
{

  public class PasswordHelper
  {
    public string HashPassword(string password, out string salt)
    {
      // Generate random salt
      byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
      salt = Convert.ToBase64String(saltBytes);

      // Derive key with PBKDF2
      var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
      byte[] hash = pbkdf2.GetBytes(32);

      return Convert.ToBase64String(hash);
    }

    public bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
      var saltBytes = Convert.FromBase64String(storedSalt);
      var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
      byte[] hash = pbkdf2.GetBytes(32);

      return Convert.ToBase64String(hash) == storedHash;
    }
  }
}
