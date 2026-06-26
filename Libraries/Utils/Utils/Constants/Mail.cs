using System.Text;

namespace Utils.Constants
{
    public class MAIL
    {
        public static string MAIL_SERVER { get; private set; }
        public static int? MAIL_PORT { get; private set; }
        public static bool MAIL_USE_DEFAULT_CREDENTIALS { get; private set; }
        public static string MAIL_CREDENTAIL_USERNAME { get; private set; }
        public static string MAIL_CREDENTAIL_PASSWORD { get; private set; }
        public static bool MAIL_SSL { get; private set; }
        public static string MAIL_SENDER { get; private set; }
        public static string MAIL_SENDER_NAME { get; private set; }
        public static void SetConfig(IConfiguration config)
        {
            var section = config.GetSection("Constants");

            MAIL_SERVER = section["MAIL_SERVER"];
            MAIL_PORT = int.TryParse(section["MAIL_PORT"], out var port) ? port : null;
            MAIL_SSL = bool.TryParse(section["MAIL_SSL"], out var ssl) && ssl;
            MAIL_USE_DEFAULT_CREDENTIALS = bool.TryParse(section["MAIL_USE_DEFAULT_CREDENTIALS"], out var creds) && creds;
            MAIL_CREDENTAIL_USERNAME = section["MAIL_CREDENTAIL_USERNAME"];
            MAIL_CREDENTAIL_PASSWORD = section["MAIL_CREDENTAIL_PASSWORD"];
            //var encodedPassword = section["MAIL_CREDENTAIL_PASSWORD"];
            //MAIL_CREDENTAIL_PASSWORD = !string.IsNullOrEmpty(encodedPassword)
            //    ? DecodeBase64(encodedPassword)
            //    : null;
            MAIL_SENDER = section["MAIL_SENDER"];
            MAIL_SENDER_NAME = section["MAIL_SENDER_NAME"];
        }


        private static string DecodeBase64(string encoded)
        {
            try
            {
                var bytes = Convert.FromBase64String(encoded);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                // ถ้า base64 ไม่ถูกต้อง ก็คืนค่าเดิม (หรือ return null ก็ได้)
                return encoded;
            }
        }
    }
}
