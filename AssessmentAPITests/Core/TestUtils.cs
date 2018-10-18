using System;
using System.Linq;

namespace Core
{
    public class TestUtils
    {
        private static string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GenerateString(int length)
        {
            var random = new Random();
            var result = new string(Enumerable.Repeat(_chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return result.ToLower();
        }
    }
}
