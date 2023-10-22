using System.Security.Cryptography;
namespace User_Management_System.AuthenticationService
{
    public class TokenGenerator
    { 
        public static string GenerateRandomToken(int length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes)
                    .Substring(0, length) 
                    .Replace('/', '_')  
                    .Replace('+', '-'); 
            }
        }
    }

}
