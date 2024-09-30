global using static PalettaPolizeiPro.HelperFunctions;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PalettaPolizeiPro
{
    public static class HelperFunctions
    {
        private static Random random = new Random();
        public static string HashString(String value)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                Byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
                foreach (Byte b in result)
                {
                    Sb.Append(b.ToString("x2"));
                }
            }
            return Sb.ToString();
        }
        public static bool AllZero(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != 0)
                {
                    return false;
                }
            }
            return true;
        }
        public static string GeneratePassword(int length = 12)
        {
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            string allChars = uppercase + lowercase + numbers + specialChars;

            char[] password = new char[length];
            password[0] = uppercase[random.Next(uppercase.Length)];
            password[1] = lowercase[random.Next(lowercase.Length)];
            password[2] = numbers[random.Next(numbers.Length)];
            password[3] = specialChars[random.Next(specialChars.Length)];

            for (int i = 4; i < length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            return new string(password.OrderBy(x => random.Next()).ToArray());
        }
    }
}
