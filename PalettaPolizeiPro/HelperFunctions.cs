global using static PalettaPolizeiPro.HelperFunctions;
using System.Security.Cryptography;
using System.Text;

namespace PalettaPolizeiPro
{
    public static class HelperFunctions
    {
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
    }
}
