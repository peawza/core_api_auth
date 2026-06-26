using System.Security.Cryptography;
using System.Text;

namespace Utils.Helper
{
    public interface IEncryptionHelper
    {
        string Encrypt(string plainText);
        string Decrypt(string encryptedText);
    }

    public class EncryptionHelper : IEncryptionHelper
    {
        private readonly byte[] _key;

        public EncryptionHelper(string inputKey, bool isBase64Encoded = true)
        {
            if (isBase64Encoded)
            {
                _key = Convert.FromBase64String(inputKey);
            }
            else
            {
                // Padding key จนครบ 16 bytes (AES-128)
                string paddedKey = inputKey.PadRight(32, '0'); // เติม '0' จนครบ 32 ตัว
                _key = Encoding.UTF8.GetBytes(paddedKey.Substring(0, 32)); // ตัดเกิน
            }

            if (_key.Length != 16 && _key.Length != 24 && _key.Length != 32)
            {
                throw new ArgumentException("Invalid AES key size. Key must be 16, 24, or 32 bytes.");
            }
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                return "";
            if (string.IsNullOrEmpty(plainText))
                return "";

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV(); // Random IV every time
            using var encryptor = aes.CreateEncryptor();

            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // Combine IV (16 bytes) + cipher text
            var combined = aes.IV.Concat(cipherBytes).ToArray();

            return Convert.ToBase64String(combined);
        }

        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
                return "";

            byte[] fullCipher;

            try
            {
                fullCipher = Convert.FromBase64String(encryptedText);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Input is not valid Base64.", ex);
            }

            if (fullCipher.Length <= 16)
                throw new CryptographicException("Encrypted data is too short to contain valid IV and cipher.");

            var iv = fullCipher.Take(16).ToArray();
            var cipher = fullCipher.Skip(16).ToArray();

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
    }


}
