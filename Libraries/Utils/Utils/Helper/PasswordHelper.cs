using System.Security.Cryptography;

namespace Utils.Helper
{
    public static class PasswordHelper
    {
        public static string GeneratePassword(int length = 12)
        {
            if (length < 8)
                length = 8; // กำหนดความยาวขั้นต่ำ

            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%";
            const string allChars = upperCase + lowerCase + digits + special;

            var passwordChars = new List<char>();

            // ✅ บังคับให้มีครบตาม policy
            passwordChars.Add(GetRandomChar(upperCase));
            passwordChars.Add(GetRandomChar(lowerCase));
            passwordChars.Add(GetRandomChar(digits));
            passwordChars.Add(GetRandomChar(special));

            // ✅ เติมตัวอักษรที่เหลือให้ครบความยาว
            for (int i = passwordChars.Count; i < length; i++)
            {
                passwordChars.Add(GetRandomChar(allChars));
            }

            // ✅ สลับตำแหน่งอักษรให้สุ่มจริง ๆ
            return Shuffle(passwordChars);
        }

        private static char GetRandomChar(string chars)
        {
            int index = RandomNumberGenerator.GetInt32(chars.Length);
            return chars[index];
        }

        private static string Shuffle(List<char> chars)
        {
            for (int i = chars.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]); // swap
            }
            return new string(chars.ToArray());
        }
    }
}
