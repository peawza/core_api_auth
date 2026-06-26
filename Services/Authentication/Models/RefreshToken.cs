namespace Authentication.Models
{
    public class RefreshTokenDo
    {
        //public string AppCode { get; set; }
        public string UserName { get; set; }
        public string RefreshToken { get; set; }
        public string LanguageCode { get; set; }
    }

}
