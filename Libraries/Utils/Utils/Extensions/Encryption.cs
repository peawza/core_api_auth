using System.Security.Cryptography;
using System.Text;

namespace Utils.Extensions
{
    public class Encryption
    {
        private string algorithm { get; set; }

        private TripleDES GenerateProvider()
        {
            TripleDES tripleDES = TripleDES.Create();

            MD5 md5 = MD5.Create();
            tripleDES.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(algorithm));
            md5.Clear();

            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;

            return tripleDES;
        }

        public Encryption(string algorithm)
        {
            this.algorithm = algorithm;
        }

        public string? Encrypt(string value)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(value);

            using (TripleDES tripleDES = GenerateProvider())
            {
                ICryptoTransform cTransform = tripleDES.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(byteArray, 0, byteArray.Length);
                tripleDES.Clear();

                //Return the encrypted data into unreadable string format
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
        }
        public string? Decrypt(string encrypt64)
        {
            try
            {
                byte[] byteArray = Convert.FromBase64String(encrypt64.Replace(" ", "+"));

                using (TripleDES tripleDES = GenerateProvider())
                {
                    ICryptoTransform cTransform = tripleDES.CreateDecryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(byteArray, 0, byteArray.Length);
                    tripleDES.Clear();

                    return UTF8Encoding.UTF8.GetString(resultArray);
                }
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}
