using Application;
using ApplicationDB.Models.Application;
using Authentication.ApiClients;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Utils.Models;
using Utils.Services;
using static Authentication.ApiClients.ApiClientsModels.SmsSendClientsModels;
using static Authentication.Models.NewFolder.UMS010;
using static Authentication.Models.OTP.OTPModels;

namespace Authentication.Services
{
    public partial interface IOtpRepository
    {

        Task<SendOtpResponse> SendOtpAsync(SendOtpRequest request, string requestIp, string userAgent, CancellationToken ct = default);

        Task<ResendOtpResponse> ResendOtpAsync(ResendOtpRequest request, string requestIp, string userAgent, CancellationToken ct = default);

        Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request, string requestIp, string userAgent, CancellationToken ct = default);



    }


    public class OtpRepository : IOtpRepository
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _db;
        private readonly SystemDbContext _systemDbContext;
        private readonly IEmailService _emailService;

        private readonly ISmsApiClients _smsApiClients;

        private readonly TimeSpan _otpLifetime = TimeSpan.FromMinutes(5);
        private const int DefaultMaxAttempt = 5;

        public OtpRepository(ApplicationUserManager userManager,
            ApplicationDbContext db, SystemDbContext systemDbContext,
            IEmailService emailService,
            ISmsApiClients smsApiClients


            )
        {
            _userManager = userManager;
            _db = db;
            _systemDbContext = systemDbContext;
            _emailService = emailService;
            _smsApiClients = smsApiClients;

        }



        public async Task<SendOtpResponse> SendOtpAsync(
        SendOtpRequest request,
        string requestIp,
        string userAgent,
        CancellationToken ct = default)
        {
            request.Channel = "loginWeb";
            var loginIdentifier = request.UserName;
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                throw new Exception("FindByNameAsync Error");
            }
            Guid? userId = new Guid(user.Id.ToString());
            string destinationMask = MaskDestination(user.Email);

            // 1) ยกเลิก OTP ที่ยัง PENDING เดิม
            await CancelAllPendingAsync(loginIdentifier, request.Channel, ct);

            // 2) Gen OTP ใหม่
            var otpCode = GenerateNumericOtp(6);
            var otpHash = HashOtp(otpCode);
            var now = DateTime.Now;
            var referenceNo = await GenerateReferenceNoAsync(ct);

            var entity = new tb_LoginOtp
            {
                OtpId = Guid.NewGuid(),
                UserId = userId,
                LoginIdentifier = loginIdentifier,
                OtpCodeHash = otpHash,
                Channel = request.Channel,
                ReferenceNo = referenceNo,
                ExpireAt = now.Add(_otpLifetime),
                SentAt = now,
                VerifiedAt = null,
                Status = "PENDING",
                AttemptCount = 0,
                MaxAttempt = DefaultMaxAttempt,
                RequestIp = requestIp,
                UserAgent = userAgent,
                CreatedAt = now,
                CreatedBy = "SYSTEM",
                UpdatedAt = null,
                UpdatedBy = null
            };

            await _db.LoginOtps.AddAsync(entity, ct);
            await _db.SaveChangesAsync(ct);

            var result = await _userManager.Users
            .Where(x => x.UserName == request.UserName)
            .Select(x => new
            {
                x.SendSMSOTP,
                x.SendEmailOTP
            })
            .FirstOrDefaultAsync();


            if (result?.SendEmailOTP == true)
            {
                await SendEmailOTPLogin(new SendOTPLogin_Criteria
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    OTPCode = otpCode,
                    RefNo = referenceNo
                });

            }

            // 3) ตรงนี้ค่อยเอา otpCode ไปส่ง SMS/Email จริง

            if (result?.SendSMSOTP == true)
            {
                var configCodes = new List<string> { "SMSSendOTP" };

                var systemConfigs = await SystemConfigs(new SystemConfigs_Criteria
                {
                    ConfigCodes = configCodes
                });

                var smsSendOtpTemplate = systemConfigs.FirstOrDefault(x => x.ConfigCode == "SMSSendOTP")?.ValueVarchar;
                var messageSMSSendOTP = string.Format(smsSendOtpTemplate ?? "Login OTP : {0}", otpCode);

                await _smsApiClients.SendSmsAsync(new SmsSendHtml_Criteria
                {

                    To = user.PhoneNumber,
                    Text = messageSMSSendOTP
                });

            }




            return new SendOtpResponse
            {
                Token = entity.OtpId,
                ExpireAt = entity.ExpireAt,
                ReferenceNo = referenceNo,
                DestinationMask = destinationMask
            };
        }

        // -------------------------------------------------------
        // RESEND OTP
        // -------------------------------------------------------
        public async Task<ResendOtpResponse> ResendOtpAsync(
            ResendOtpRequest request,
            string requestIp,
            string userAgent,
            CancellationToken ct = default)
        {
            var entity = await _db.LoginOtps
                .FirstOrDefaultAsync(x => x.OtpId == request.Token, ct);

            if (entity == null || entity.Status != "PENDING")
            {
                throw new InvalidOperationException("OTP_NOT_FOUND_OR_NOT_PENDING");
            }
            var now = DateTime.Now;

            // Gen OTP ใหม่บน record เดิม (Token เดิม)
            var newCode = GenerateNumericOtp(6);
            var newHash = HashOtp(newCode);
            var referenceNo = await GenerateReferenceNoAsync(ct);
            var user = await _userManager.FindByIdAsync(entity.UserId.ToString());

            entity.OtpCodeHash = newHash;
            entity.ReferenceNo = referenceNo;
            entity.SentAt = now;
            entity.ExpireAt = now.Add(_otpLifetime);
            entity.AttemptCount = 0;
            entity.Status = "PENDING";
            entity.RequestIp = requestIp;
            entity.UserAgent = userAgent;
            entity.UpdatedAt = now;
            entity.UpdatedBy = "SYSTEM";

            _db.LoginOtps.Update(entity);
            await _db.SaveChangesAsync(ct);

            // ส่ง OTP ใหม่จริง ๆ ตรงนี้
            //// await _otpSender.SendAsync(entity.Channel, entity.LoginIdentifier, newCode, ct);


            var result = await _userManager.Users
              .Where(x => x.UserName == user.UserName)
              .Select(x => new
              {
                  x.SendSMSOTP,
                  x.SendEmailOTP
              })
              .FirstOrDefaultAsync();


            if (result?.SendEmailOTP == true)
            {
                await SendEmailOTPLogin(new SendOTPLogin_Criteria
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    OTPCode = newCode,
                    RefNo = referenceNo
                });

            }

            // 3) ตรงนี้ค่อยเอา otpCode ไปส่ง SMS/Email จริง

            if (result?.SendSMSOTP == true)
            {
                var configCodes = new List<string> { "SMSSendOTP" };

                var systemConfigs = await SystemConfigs(new SystemConfigs_Criteria
                {
                    ConfigCodes = configCodes
                });

                var smsSendOtpTemplate = systemConfigs.FirstOrDefault(x => x.ConfigCode == "SMSSendOTP")?.ValueVarchar;
                var messageSMSSendOTP = string.Format(smsSendOtpTemplate ?? "Login OTP : {0}", newCode);

                await _smsApiClients.SendSmsAsync(new SmsSendHtml_Criteria
                {

                    To = user.PhoneNumber,
                    Text = messageSMSSendOTP
                });

            }



            return new ResendOtpResponse
            {
                Token = entity.OtpId,
                ExpireAt = entity.ExpireAt,
                RefNo = referenceNo
            };
        }

        // -------------------------------------------------------
        // VERIFY OTP
        // -------------------------------------------------------
        public async Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request, string requestIp, string userAgent, CancellationToken ct = default)
        {
            var now = DateTime.Now;

            var entity = await _db.LoginOtps
                .FirstOrDefaultAsync(x => x.OtpId == request.Token, ct);

            if (entity == null || entity.Status != "PENDING")
            {
                return new VerifyOtpResponse
                {
                    Verified = false,
                    UserName = entity?.LoginIdentifier ?? string.Empty,
                    StatusCode = "400",
                    StatusName = "BadRequest",
                    MessageCode = "OTP_USED_OR_EXPIRED",
                    MessageName = "OTP has already been used or expired."
                };
            }

            if (entity.ExpireAt <= now)
            {
                entity.Status = "EXPIRED";
                entity.UpdatedAt = now;
                entity.UpdatedBy = "SYSTEM";

                _db.LoginOtps.Update(entity);
                await _db.SaveChangesAsync(ct);

                return new VerifyOtpResponse
                {
                    Verified = false,
                    UserName = entity.LoginIdentifier,
                    StatusCode = "400",
                    StatusName = "BadRequest",
                    MessageCode = "OTP_USED_OR_EXPIRED",
                    MessageName = "OTP has already been used or expired."
                };
            }

            if (entity.AttemptCount >= entity.MaxAttempt)
            {
                entity.Status = "EXPIRED";
                entity.UpdatedAt = now;
                entity.UpdatedBy = "SYSTEM";

                _db.LoginOtps.Update(entity);
                await _db.SaveChangesAsync(ct);

                return new VerifyOtpResponse
                {
                    Verified = false,
                    UserName = entity.LoginIdentifier,
                    StatusCode = "400",
                    StatusName = "BadRequest",
                    MessageCode = "OTP_MAX_ATTEMPT",
                    MessageName = "Too many failed OTP attempts. Please try again later."
                };
            }

            var hash = HashOtp(request.OtpCode);


            if (!string.Equals(hash, entity.OtpCodeHash, StringComparison.OrdinalIgnoreCase))
            {
                entity.AttemptCount += 1;
                entity.UpdatedAt = now;
                entity.UpdatedBy = "SYSTEM";

                if (entity.AttemptCount >= entity.MaxAttempt)
                {
                    entity.Status = "EXPIRED";

                    _db.LoginOtps.Update(entity);
                    await _db.SaveChangesAsync(ct);

                    return new VerifyOtpResponse
                    {
                        Verified = false,
                        UserName = entity.LoginIdentifier,
                        StatusCode = "400",
                        StatusName = "BadRequest",
                        MessageCode = "OTP_MAX_ATTEMPT",
                        MessageName = "Too many failed OTP attempts. Please try again later."
                    };
                }

                _db.LoginOtps.Update(entity);
                await _db.SaveChangesAsync(ct);

                return new VerifyOtpResponse
                {
                    Verified = false,
                    UserName = entity.LoginIdentifier,
                    StatusCode = "400",
                    StatusName = "BadRequest",
                    MessageCode = "OTP_INVALID",
                    MessageName = "OTP is incorrect."
                };
            }

            entity.Status = "VERIFIED";
            entity.VerifiedAt = now;
            entity.AttemptCount += 1;
            entity.RequestIp = requestIp;
            entity.UserAgent = userAgent;
            entity.UpdatedAt = now;
            entity.UpdatedBy = "SYSTEM";

            _db.LoginOtps.Update(entity);
            await _db.SaveChangesAsync(ct);

            return new VerifyOtpResponse
            {
                Verified = true,
                UserName = entity.LoginIdentifier,
                StatusCode = "200",
                StatusName = "SUCCESS",
                MessageCode = "OTP_VERIFIED",
                MessageName = "OTP verified successfully."
            };
        }


        private async Task CancelAllPendingAsync(
            string loginIdentifier,
            string channel,
            CancellationToken ct)
        {
            var now = DateTime.Now;


            var pendingList = await _db.LoginOtps
                .Where(x =>
                    x.LoginIdentifier == loginIdentifier &&
                    x.Channel == channel &&
                    x.Status == "PENDING")
                .ToListAsync(ct);

            foreach (var item in pendingList)
            {
                item.Status = "CANCELLED";
                item.UpdatedAt = now;
                item.UpdatedBy = "SYSTEM";
            }


            var cutoff = now.AddDays(-1);

            var expiredToDelete = await _db.LoginOtps
                .Where(x =>
                    x.ExpireAt < cutoff &&
                    (x.Status == "EXPIRED" || x.Status == "CANCELLED"))
                .ToListAsync(ct);

            if (expiredToDelete.Count > 0)
            {
                _db.LoginOtps.RemoveRange(expiredToDelete);
            }

            await _db.SaveChangesAsync(ct);
        }

        private string GenerateNumericOtp(int length)
        {
            var bytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            var sb = new StringBuilder(length);
            foreach (var b in bytes)
            {
                sb.Append((b % 10).ToString());
            }
            return sb.ToString();
        }

        private string HashOtp(string otpCode)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(otpCode);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        private string MaskDestination(string loginIdentifier)
        {
            // ตัวอย่าง Mask email
            if (loginIdentifier.Contains("@"))
            {
                var parts = loginIdentifier.Split('@');
                var name = parts[0];
                if (name.Length <= 2) return "***@" + parts[1];
                return name[0] + "***" + name[^1] + "@" + parts[1];
            }

            // สมมติเป็นเบอร์
            if (loginIdentifier.Length >= 4)
            {
                return new string('x', loginIdentifier.Length - 4) + loginIdentifier[^4..];
            }

            return loginIdentifier;
        }

        private async Task<string> GenerateReferenceNoAsync(CancellationToken ct)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string refNo;

            do
            {
                // สุ่ม 6 ตัว
                var bytes = new byte[6];
                RandomNumberGenerator.Fill(bytes);

                var chars = new char[6];
                for (int i = 0; i < 6; i++)
                {
                    // map 0–255 -> 0–25
                    var idx = bytes[i] % alphabet.Length;
                    chars[i] = alphabet[idx];
                }

                refNo = new string(chars);   // เช่น "AQZKLM"
            }
            while (await _db.LoginOtps.AnyAsync(x => x.ReferenceNo == refNo, ct));

            return refNo;
        }

        public async Task<List<SystemConfigs_Result>> SystemConfigs(SystemConfigs_Criteria criteria)
        {



            var result = await _systemDbContext.TsSystemConfigs
                .Where(x => criteria.ConfigCodes.Contains(x.ConfigCode))
                .Select(x => new SystemConfigs_Result
                {
                    ConfigCode = x.ConfigCode,
                    ConfigDesc = x.ConfigDesc,
                    ValueVarchar = x.ValueVarchar,
                    ValueVarchar2 = x.ValueVarchar2,
                    ValueVarchar3 = x.ValueVarchar3,
                    ValueInt = x.ValueInt,
                    ValueDate = x.ValueDate
                })
                .ToListAsync();

            return result;
        }
        public async Task<SendOTPLogin_Result> SendEmailOTPLogin(SendOTPLogin_Criteria criteria)
        {
            try
            {
                var configCodes = new List<string>
                {
                    "PhoneNo",
                    "Software",
                    "Branding",
                    "Email",
                    "Address",
                    "CcToEmail",
                    "URL_Login",
                    "URL_FirstLogin",
                    "EmailSendOTP"

                };

                var systemConfigs = await SystemConfigs(new SystemConfigs_Criteria
                {
                    ConfigCodes = configCodes
                });

                var phoneNo = systemConfigs.FirstOrDefault(x => x.ConfigCode == "PhoneNo")?.ValueVarchar;
                var software = systemConfigs.FirstOrDefault(x => x.ConfigCode == "Software")?.ValueVarchar;
                var branding = systemConfigs.FirstOrDefault(x => x.ConfigCode == "Branding")?.ValueVarchar;
                var email = systemConfigs.FirstOrDefault(x => x.ConfigCode == "Email")?.ValueVarchar;
                var address = systemConfigs.FirstOrDefault(x => x.ConfigCode == "Address")?.ValueVarchar;


                string TemplateEmail = systemConfigs.FirstOrDefault(x => x.ConfigCode == "EmailSendOTP").ValueVarchar;
                string templateSubject = systemConfigs.FirstOrDefault(x => x.ConfigCode == "EmailSendOTP").ValueVarchar2;

                templateSubject = templateSubject
                        .Replace("{Branding}", branding)

                        .Replace("{SupportEmail}", email)
                        .Replace("{RecipientEmail}", email)
                        .Replace("{SupportPhone}", phoneNo)
                        .Replace("{OTPCode}", criteria.OTPCode)
                        .Replace("{RefNo}", criteria.RefNo)
                        .Replace("{UserName}", criteria.UserName);

                var emailMessage = new EmailMessageDo
                {
                    Subject = TemplateEmail?.Replace("{Branding}", branding),
                    To = criteria.Email,
                    Body = templateSubject,
                };
                bool result = await _emailService.SendAsync(emailMessage);

                if (result == true)
                {

                    return new SendOTPLogin_Result
                    {
                        StatusCode = "200",
                        StatusName = "Success",
                        MessageCode = "",
                        MessageName = "Send Email Success"
                    };
                }
                return new SendOTPLogin_Result
                {
                    StatusCode = "400",
                    StatusName = "error",
                    MessageCode = "",
                    MessageName = "Send Email error"
                };

            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
