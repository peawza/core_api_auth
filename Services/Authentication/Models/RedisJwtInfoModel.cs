namespace Authentication.Models
{
    public class RedisJwtInfo
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public string Permission { get; set; }
        public string Device { get; set; }
        public DateTime LoginTime { get; set; }
        public string IpAddress { get; set; }
        public string ConnectionStringDB { get; set; }
        public int CustomerID { get; set; }
    }


    public class RedisMobileInfo
    {
        public string? version { get; set; }

    }
}

