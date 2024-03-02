using System.Text;

namespace Manage_System.Helper
{
    public static class Utilities
    {
        public static int PAGE_SIZE = 10;

        public static string GetRamdomKey(int length = 5)
        {
            string pattern = @"0123456789zxcvbnmasdfghjklqwertyuiop[]{}:~!@#$%^&*()+";
            Random rd = new Random();

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(pattern[rd.Next(0, pattern.Length)]);

            }
            return stringBuilder.ToString();
        }

        
    }
}
