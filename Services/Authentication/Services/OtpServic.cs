using Application;
using static Authentication.Models.OTP.OTPModels;

namespace Authentication.Services
{
    public partial interface IOtpServic
    {
        Task<SendOtpResponse> SendOtpAsync(SendOtpRequest request, string requestIp, string userAgent, CancellationToken ct = default);

        Task<ResendOtpResponse> ResendOtpAsync(ResendOtpRequest request, string requestIp, string userAgent, CancellationToken ct = default);

        Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request, string requestIp, string userAgent, CancellationToken ct = default);


    }
    public class OtpServic : IOtpServic
    {


        private readonly IOtpRepository _repository;
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OtpServic(IOtpRepository repository, ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _db = db;

            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<SendOtpResponse> SendOtpAsync(SendOtpRequest request, string requestIp, string userAgent, CancellationToken ct = default)
        {

            return await _repository.SendOtpAsync(request, requestIp, userAgent, ct);
        }

        public async Task<ResendOtpResponse> ResendOtpAsync(ResendOtpRequest request, string requestIp, string userAgent, CancellationToken ct = default)
        {


            return await _repository.ResendOtpAsync(request, requestIp, userAgent, ct);
        }

        public async Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request, string requestIp, string userAgent, CancellationToken ct = default)
        {
            return await _repository.VerifyOtpAsync(request, requestIp, userAgent, ct);
        }
    }
}
