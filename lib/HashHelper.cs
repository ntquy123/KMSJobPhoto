using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace erpsolution.lib
{
    public class HashHelper
    {
        private static readonly Encoding Encoding1252 = Encoding.GetEncoding(1252);

        public static byte[] SHA1HashValue(string s)
        {
            byte[] bytes = Encoding1252.GetBytes(s);

            var sha1 = SHA512.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);

            return hashBytes;
        }

        public static string HashPassValueToSHA512(string password)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("X2"));
                }

                return stringBuilder.ToString().ToLower();
            }
        }

        public static bool IsEqualHashValue512(string password, string hashedPassword)
        {
            string hashedInput = HashPassValueToSHA512(password);
            return hashedInput.Equals(hashedPassword);
        }

    }
}
