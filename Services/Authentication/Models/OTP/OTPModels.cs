namespace Authentication.Models.OTP
{
    public class OTPModels
    {

        public class SendOtpRequest
        {
            public string UserName { get; set; } = null!;
            public string Channel { get; set; } = "EMAIL";   // SMS / EMAIL / APP
        }

        public class SendOtpResponse
        {
            public Guid Token { get; set; }                  // ใช้เป็น Token (OtpId)
            public DateTime ExpireAt { get; set; }

            public string ReferenceNo { get; set; } = "";
            public string DestinationMask { get; set; } = "";
        }

        public class ResendOtpRequest
        {
            public Guid Token { get; set; }                  // OtpId เดิม
        }

        public class ResendOtpResponse
        {
            public Guid Token { get; set; }
            public DateTime ExpireAt { get; set; }
            public string RefNo { get; set; }
        }

        public class VerifyOtpRequest
        {
            public Guid Token { get; set; }
            public string OtpCode { get; set; } = null!;

            public string Device { get; set; } = null!;
        }

        public class VerifyOtpResponse
        {

            public bool Verified { get; set; }
            public string UserName { get; set; }

            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }


        }

        #region SendOTPLogin

        public class SendOTPLogin_Criteria
        {

            public string Email { get; set; }
            public string UserName { get; set; }
            public string OTPCode { get; set; }
            public string RefNo { get; set; }

        }


        public class SendOTPLogin_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }
        #endregion


    }
}
