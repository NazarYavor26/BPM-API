using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Common.Utilities
{
    public static class SecurePasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const string HashIdentifier = "$UMS$";

        public static string Hash(string password, int iterations)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(HashSize);

            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            var base64Hash = Convert.ToBase64String(hashBytes);

            return string.Format("{0}{1}${2}", HashIdentifier, iterations, base64Hash);
        }

        public static string Hash(string password) => Hash(password, 1000);

        public static bool IsHashSupported(string hashString) => hashString.Contains(HashIdentifier);

        public static bool Verify(string password, string hashedPassword)
        {
            if(!IsHashSupported(hashedPassword))
            {
                throw new Exception("The hashtype is not supported");
            }

            var splittedHashString = hashedPassword.Replace(HashIdentifier, "").Split('$');
            var iterations = int.Parse(splittedHashString[0]);
            var base64Hash = splittedHashString[1];

            var hashBytes = Convert.FromBase64String(base64Hash);

            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
