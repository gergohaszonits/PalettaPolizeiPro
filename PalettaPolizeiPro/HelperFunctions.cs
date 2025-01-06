global using static PalettaPolizeiPro.HelperFunctions;
using Sharp7;
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
        public static string ExtractNumbersFromString(string input)
        {
            string output = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                {
                    output += input[i];
                }
            }
            return output;
        }
        public static string GetIdentifier(byte[] bytes, int loop)
        {
            string lNummer = "";
            int lLen = 3 - loop.ToString().Length;
            for (int i = 0; i < lLen; i++)
            {
                lNummer += "0";
            }
            lNummer = lNummer + loop;

            string hex = "";
            string wNummer = "";

            for (int i = 0; i < 2; i++)
            {
                byte[] temp = [bytes.GetByteAt(0 + i)];
                hex += BitConverter.ToString(temp);
            }

            int wLen = 4 - hex.Length;
            for (int i = 0; i < wLen; i++)
            {
                wNummer += "0";
            }
            wNummer += hex;

            return "L" + lNummer + "W" + wNummer;
        }
        public static byte[] SetIdentifierNummer(string wNum)
        {
            return Enumerable.Range(0, wNum.Length)
                          .Where(x => x % 2 == 0)
                          .Select(x => Convert.ToByte(wNum.Substring(x, 2), 16))
                          .ToArray();
        }
       
        public static Type GetOriginalClass(Type type)
        {
            while (type.BaseType != null && type.BaseType != typeof(object))
            {
                type = type.BaseType;
            }
            return type;
        }
        public static string GenerateConnectionCode()
        {
            int length = 6;

            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";

            string allChars = uppercase + numbers;

            string password = "";

            for (int i = 0; i < length; i++)
            {
                password += allChars[Random.Shared.Next(allChars.Length)];
            }

            return password;
        }

    }
   
}
