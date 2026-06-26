namespace Authentication.Models
{
    public class MobileModels
    {
        public class getVersionMobile_Criteria
        {
            public string? os { get; set; }
            public string? version { get; set; }
        }

        public class getVersionMobile_Result
        {
            public bool? status { get; set; }
            public string? minVersion { get; set; }
        }
    }
}
