using Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using Utils.Extensions;
using static Authentication.Models.OTP.OTPModels;

namespace Authentication.Controllers
{

    [Route("otp")]
    public class OtpController : AppControllerBase
    {
        private readonly ILogger<OtpController> logger;
        private readonly IOtpServic _otpService;


        public OtpController(ILogger<OtpController> logger, IOtpServic otpService)
        {
            this.logger = logger;
            _otpService = otpService;
        }


        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(
        [FromBody] SendOtpRequest request,
        CancellationToken ct)
        {


            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                var ua = Request.Headers["User-Agent"].ToString();

                var data = await _otpService.SendOtpAsync(request, ip, ua, ct);


                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request, CancellationToken ct)
        {


            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                var ua = Request.Headers["User-Agent"].ToString();

                var data = await _otpService.ResendOtpAsync(request, ip, ua, ct);



                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request, CancellationToken ct = default)
        {


            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                var ua = Request.Headers["User-Agent"].ToString();
                var data = await _otpService.VerifyOtpAsync(request, ip, ua, ct);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
