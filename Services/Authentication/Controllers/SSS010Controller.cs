using Application.Models;
using Authentication.Constants;
using Authentication.Models;
using Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Utils.Helper;
using Utils.Services;
using static Authentication.Models.NewFolder.UMS010;
using static Authentication.Models.OTP.OTPModels;

namespace Authentication.Controllers
{
    [Route("[controller]")]
    public class SSS010Controller : ControllerBase
    {
        private readonly ApplicationUserManager userManager;

        private readonly ApplicationSignInManager signInManager;

        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly UrlEncoder urlEncoder;
        private readonly ILogger<SSS010Controller> logger;


        private readonly IConnectionMultiplexer _redis;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IEncryptionHelper _encryption;

        private readonly ISSS010Services _sss010Services;
        private readonly IUMS010Service _ums010Services;


        private readonly IOtpServic _otpService;
        public string UserName { get; private set; }

        public SSS010Controller(
            ApplicationUserManager userManager,

            ApplicationSignInManager signInManager,
            IConfiguration configuration,
            IEmailService emailService,
            UrlEncoder urlEncoder,
            ILogger<SSS010Controller> logger,
            IConnectionMultiplexer redis,

            IHttpContextAccessor httpContextAccessor,

            IEncryptionHelper encryption,
            ISSS010Services sss010Services,
            IUMS010Service ums010Services,
            IOtpServic otpService

        )
        {
            this.userManager = userManager;

            this.signInManager = signInManager;
            this.configuration = configuration;
            this.emailService = emailService;
            this.urlEncoder = urlEncoder;
            this.logger = logger;
            this._redis = redis;

            _httpContextAccessor = httpContextAccessor;

            _encryption = encryption;
            _sss010Services = sss010Services;
            _ums010Services = ums010Services;
            _otpService = otpService;


        }


        [HttpPost]
        [Route("Login")]


        public async Task<IActionResult> VerifyLogin([FromBody] Models.UserLoginDo oUser)
        {
            try
            {




                string cookieName = string.Format("{0}:{1}", oUser.UserName, Constants.AUTH.REMEMBER_USER_NAME);
                ApplicationUser? appUser = await this.userManager.FindByNameAsync(oUser.UserName);
                //if (appUser == null)
                //    return NotFound("E0003");

                //Microsoft.AspNetCore.Identity.SignInResult res = await this.signInManager.PasswordSignInAsync(oUser.UserName, oUser.Password, true, false);
                var signInResult = await signInManager.PasswordSignInAsync(oUser.UserName, oUser.Password, false, true);


                int remindPasswordExpired = 0;
                bool isExpired = false;
                if (signInResult.Succeeded)
                {
                    if (appUser.PasswordAge != null)
                    {
                        int diff = 0;
                        if (appUser.LastUpdatePasswordDate == null)
                            isExpired = true;
                        else
                        {
                            diff = appUser.LastUpdatePasswordDate.Value
                                        .AddDays(appUser.PasswordAge.Value)
                                        .CompareTo(Utils.Extensions.IOUtil.GetCurrentDateTime);
                            if (diff < 0)
                                isExpired = true;
                        }

                        if (isExpired == false
                            && appUser.LastLoginDate != null)
                        {
                            if (Utils.Constants.COMMON.REMIND_PASSWORD_DATE != null)
                            {
                                int remind = 0;
                                if (int.TryParse(Utils.Constants.COMMON.REMIND_PASSWORD_DATE, out remind))
                                {
                                    if (diff <= remind)
                                    {
                                        remindPasswordExpired = diff;
                                    }
                                }
                            }
                        }
                    }

                    if (isExpired)
                    {
                        await this.signInManager.SignOutAsync();
                        return Unauthorized("E0008");
                    }
                }
                else if (signInResult.IsLockedOut)
                    return Unauthorized($"E0006:{AUTH.LOGIN_WAITING_TIME:N0}");
                else
                {
                    int accessFailedCount = await this.userManager.GetAccessFailedCountAsync(appUser);
                    int attemptsLeft = AUTH.MAXIMUM_LOGIN_FAIL - accessFailedCount;

                    return Unauthorized($"E0005:{attemptsLeft:N0}");
                }



                bool flagActive = appUser.ActiveFlag;
                if (flagActive == true)
                {
                    //var roles = await this.userManager.GetRolesAsync(appUser);
                    //if (roles.Count > 0)
                    //{
                    //    foreach (string role in roles)
                    //    {
                    //        ApplicationRole? appRole = await this.roleManager.FindByNameAsync(role);
                    //        if (appRole != null)
                    //        {
                    //            flagActive = appRole.IsActive;
                    //            if (flagActive == false)
                    //                break;
                    //        }
                    //    }
                    //}
                }
                if (flagActive == false)
                    return Unauthorized("E0004");

                if (appUser.FirstLoginFlag == true)
                    return Unauthorized("E0073");





                await this.signInManager.SignInAsync(appUser, false);
                await this.userManager.ResetAccessFailedCountAsync(appUser);


                if (appUser.PasswordAge != null)
                {
                    int diff = 0;
                    if (appUser.LastUpdatePasswordDate == null)
                        isExpired = true;
                    else
                    {
                        diff = appUser.LastUpdatePasswordDate.Value
                                    .AddDays(appUser.PasswordAge.Value)
                                    .CompareTo(Utils.Extensions.IOUtil.GetCurrentDateTime);
                        if (diff < 0)
                            isExpired = true;
                    }

                    if (isExpired == false
                        && appUser.LastLoginDate != null)
                    {
                        if (Utils.Constants.COMMON.REMIND_PASSWORD_DATE != null)
                        {
                            int remind = 0;
                            if (int.TryParse(Utils.Constants.COMMON.REMIND_PASSWORD_DATE, out remind))
                            {
                                if (diff <= remind)
                                {
                                    remindPasswordExpired = diff;
                                }
                            }
                        }
                    }
                }

                if (isExpired)
                    return Unauthorized("E0008");

                appUser.LastLoginDate = Utils.Extensions.IOUtil.GetCurrentDateTime;

                if (oUser.LanguageCode != null)
                {
                    this.userManager.UpdateUserLanguageCode(appUser, oUser.LanguageCode);
                }


                var userInfo = this.userManager.GetUserInfo(appUser);

                Dictionary<string, int[]> permissionScreen = await _sss010Services.GetGroupPermissionUser(new Models.SSS010.SSS010.SSS010_GetGroupPermissionUser_Criteria
                {
                    UserId = appUser.Id,

                });
                string jsonPermissionScreen = JsonSerializer.Serialize(permissionScreen);
                string base64PermissionScreen = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonPermissionScreen));


                string WarehouseByPass = await _sss010Services.WhereAllData(null);
                WarehouseByPass = (!string.IsNullOrWhiteSpace(WarehouseByPass) && WarehouseByPass.Contains(',')) ? null : WarehouseByPass?.Trim();







                string token = await this.signInManager.GenerateToken(appUser, userInfo.LanguageCode, oUser.Device, WarehouseByPass);
                string rtoken = await this.userManager.GenerateRefreshTokenAsync(appUser);



                #region Set cookie


                if (HttpContext.Request.Cookies.ContainsKey(cookieName))
                {

                    HttpContext.Response.Cookies.Delete(cookieName);
                }



                #endregion



                //var result_info = await _apiAdminBack.API_GetCustomerConfigByUsername(new API_GetCustomerConfigByUsername_Criteria()
                //{
                //    UserName = oUser.UserName,
                //    Device = oUser.Device
                //});

                #region RedisJwt
                // Task<List<Common_SystemConfigs_Result>> SystemConfigs(Common_SystemConfigs_Criteria criteria)






                var jwtInfo = new RedisJwtInfo
                {
                    Token = token,
                    RefreshToken = rtoken,
                    Permission = null,
                    Device = oUser.Device,
                    LoginTime = DateTime.Now,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),

                };

                var ttl = TimeSpan.FromMinutes(Convert.ToDouble(this.configuration["JwtExpireMinutes"]));




                /*
                var redisKey = $"jwt:{appUser.UserName}:{oUser.Device}";
                var redisValue = JsonSerializer.Serialize(jwtInfo);
                var db = _redis.GetDatabase();

                if (oUser.Device == "mobile")
                {
                    var getConfig = await _apiAdminBack.SystemConfigs(new Common_SystemConfigs_Criteria()
                    {
                        ConfigCodes = new List<string>
                        {
                            "MobileOSVersion_android",
                            "MobileOSVersion_ios",
                        }
                    });
                    var redisKey_MobileOSVersion_Android = $"mobile:android";


                    var redisInfo_Android = new RedisMobileInfo
                    {
                        version = getConfig.FirstOrDefault(x => x.ConfigCode == "MobileOSVersion_android")?.ValueVarchar,

                    };

                    var redisKey_MobileOSVersion_IOS = $"mobile:ios";
                    var redisInfo_IOS = new RedisMobileInfo
                    {
                        version = getConfig.FirstOrDefault(x => x.ConfigCode == "MobileOSVersion_ios")?.ValueVarchar,

                    };
                    await db.StringSetAsync(new[]
                    {
                        new KeyValuePair<RedisKey, RedisValue>(redisKey_MobileOSVersion_Android, JsonSerializer.Serialize(redisInfo_Android)),
                        new KeyValuePair<RedisKey, RedisValue>(redisKey_MobileOSVersion_IOS,     JsonSerializer.Serialize(redisInfo_IOS)),
                    });
                }

                     await db.StringSetAsync(redisKey, redisValue, ttl);
                */


                #endregion


                return Ok(new
                {
                    //TwoFactorEnabled = appUser.TwoFactorEnabled,
                    Id = appUser.Id,
                    UserNumber = userInfo.UserNumber,
                    UserName = appUser.UserName,
                    VerifyLogin = appUser.OTPLogin,
                    LanguageCode = userInfo.LanguageCode,
                    DisplayName = userInfo.Name.ToUpper(),
                    Token = token,
                    RefreshToken = rtoken,
                    PermissionScreen = base64PermissionScreen,
                    Timeout = Convert.ToDouble(this.configuration["JwtExpireMinutes"]),
                    RemindPasswordExpired = remindPasswordExpired
                });
            }
            catch (Exception ex)
            {
                return Ok(ex);
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



                return Ok(new
                {
                    status = true,
                    token = data.Token,
                    ExpireAt = data.ExpireAt,
                    RefNo = data.RefNo

                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    error = ex
                });






            }
        }


        [HttpPost]
        [Route("otpverifylogin")]

        [HttpPost]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request, CancellationToken ct)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var ua = Request.Headers["User-Agent"].ToString();
            var data = await _otpService.VerifyOtpAsync(request, ip, ua, ct);

            // ถ้าไม่ผ่าน OTP หรือ StatusCode != 200 -> BadRequest พร้อมส่ง code/message กลับไป
            if (data.Verified != true || data.StatusCode != "200")
            {
                return BadRequest(new
                {
                    Verified = data.Verified,
                    StatusCode = data.StatusCode,
                    StatusName = data.StatusName,
                    MessageCode = data.MessageCode,
                    MessageName = data.MessageName
                });
            }

            // ---------- OTP ผ่านแล้ว ดำเนินการล็อกอินต่อ ----------
            ApplicationUser? appUser = await this.userManager.FindByNameAsync(data.UserName);
            if (appUser == null)
            {
                // กันเคส safety เผื่อ user หาไม่เจอ
                return BadRequest(new
                {
                    Verified = false,
                    StatusCode = 400,
                    StatusName = "BadRequest",
                    MessageCode = "USER_NOT_FOUND",
                    MessageName = "User not found."
                });
            }

            var userInfo = this.userManager.GetUserInfo(appUser);

            Dictionary<string, int[]> permissionScreen =
                await _sss010Services.GetGroupPermissionUser(
                    new Models.SSS010.SSS010.SSS010_GetGroupPermissionUser_Criteria
                    {
                        UserId = appUser.Id,
                    });

            string jsonPermissionScreen = JsonSerializer.Serialize(permissionScreen);
            string base64PermissionScreen = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonPermissionScreen));
            string WarehouseByPass = await _sss010Services.WhereAllData(null);
            WarehouseByPass = (!string.IsNullOrWhiteSpace(WarehouseByPass) && WarehouseByPass.Contains(',')) ? null : WarehouseByPass?.Trim();



            string token = await this.signInManager.GenerateToken(appUser, userInfo.LanguageCode, request.Device, WarehouseByPass);
            string rtoken = await this.userManager.GenerateRefreshTokenAsync(appUser);

            var jwtInfo = new RedisJwtInfo
            {
                Token = token,
                RefreshToken = rtoken,
                Permission = null,
                Device = request.Device,
                LoginTime = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            };

            var ttl = TimeSpan.FromMinutes(
                Convert.ToDouble(this.configuration["JwtExpireMinutes"]));

            // TODO: ถ้าจะเก็บ jwtInfo ลง Redis ก็ทำตรงนี้

            return Ok(new
            {
                // จาก VerifyOtpResponse
                Verified = data.Verified,
                StatusCode = data.StatusCode,     // 200
                StatusName = data.StatusName,     // "OK"
                MessageCode = data.MessageCode,    // "OTP_VERIFIED"
                MessageName = data.MessageName,    // "OTP verified successfully."

                // ข้อมูล user + token เดิม
                Id = appUser.Id,
                UserNumber = userInfo.UserNumber,
                UserName = appUser.UserName,
                VerifyLogin = appUser.OTPLogin,
                LanguageCode = userInfo.LanguageCode,
                DisplayName = userInfo.Name.ToUpper(),
                Token = token,
                RefreshToken = rtoken,
                PermissionScreen = base64PermissionScreen,
                Timeout = Convert.ToDouble(this.configuration["JwtExpireMinutes"]),
                //RemindPasswordExpired = remindPasswordExpired
            });
        }


        [HttpPost]
        [Route("RefreshToken")]

        public async Task<IActionResult> RefreshToken([FromBody] Models.RefreshTokenDo oToken)
        {
            if (string.IsNullOrWhiteSpace(oToken.UserName) ||
                string.IsNullOrWhiteSpace(oToken.RefreshToken))
            {
                return Unauthorized(new { message = "Invalid request." });
            }

            var device = User.Claims.FirstOrDefault(c => c.Type == "device")?.Value;

            ApplicationUser? appUser = await this.userManager.FindByNameAsync(oToken.UserName);
            if (appUser == null)
            {
                return Unauthorized(new { message = "User not found." });
            }

            this.userManager.UpdateUserLanguageCode(appUser, oToken.LanguageCode);
            var userInfo = this.userManager.GetUserInfo(appUser);

            // 👉 flag ว่าจะใช้ Redis ไหม (เช็คทั้ง config และ _redis != null)
            bool useRedis = _redis != null &&
                            this.configuration.GetValue<bool>("Redis:UseRedis", true);

            RedisJwtInfo? existingJwtInfo = null;

            // ====== ส่วนที่ 1: ถ้าใช้ Redis ให้ตรวจจาก Redis ก่อน ======
            if (useRedis && device != null)
            {
                var db = _redis.GetDatabase();
                var redisKey = $"jwt:{appUser.UserName}:{device}";

                var existingValue = await db.StringGetAsync(redisKey);
                if (!string.IsNullOrEmpty(existingValue))
                {
                    existingJwtInfo = JsonSerializer.Deserialize<RedisJwtInfo>(existingValue);

                    // 👉 ตรวจ refresh token ให้ตรงกับใน Redis
                    if (existingJwtInfo?.RefreshToken != oToken.RefreshToken)
                    {
                        return Unauthorized(new { message = "Invalid refresh token (not match Redis)." });
                    }
                }
                else
                {
                    return Unauthorized(new { message = "No session found in Redis." });
                }
            }

            // ====== ส่วนที่ 2: ตรวจสอบกับ DB (ใช้ทั้งกรณีมี / ไม่มี Redis) ======
            //var isValid = await this.userManager.VerifyRefreshTokenAsync(appUser, oToken.RefreshToken);
            //if (!isValid)
            //{
            //    return Unauthorized(new { message = "Invalid refresh token." });
            //}

            // ====== ส่วนที่ 3: ออก Token ใหม่ ======
            string WarehouseByPass = await _sss010Services.WhereAllData(null);
            WarehouseByPass = (!string.IsNullOrWhiteSpace(WarehouseByPass) && WarehouseByPass.Contains(',')) ? null : WarehouseByPass?.Trim();

            string token = await this.signInManager.GenerateToken(appUser, userInfo.LanguageCode, device, WarehouseByPass);
            string rtoken = await this.userManager.GenerateRefreshTokenAsync(appUser);

            // ====== ส่วนที่ 4: ถ้าใช้ Redis ค่อยเขียนกลับเข้า Redis ======
            if (useRedis && device != null && existingJwtInfo != null)
            {
                var jwtInfo = existingJwtInfo;
                jwtInfo.Token = token;
                jwtInfo.RefreshToken = rtoken;
                jwtInfo.Device = device;
                jwtInfo.LoginTime = DateTime.Now;
                jwtInfo.Permission = existingJwtInfo.Permission;
                jwtInfo.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                jwtInfo.ConnectionStringDB = existingJwtInfo.ConnectionStringDB;
                jwtInfo.CustomerID = existingJwtInfo.CustomerID;

                var redisValue = JsonSerializer.Serialize(jwtInfo);
                var ttl = TimeSpan.FromMinutes(
                    Convert.ToDouble(this.configuration["JwtExpireMinutes"])
                );
                var db = _redis.GetDatabase();
                await db.StringSetAsync($"jwt:{appUser.UserName}:{device}", redisValue, ttl);

                return Ok(new
                {
                    Token = token,
                    RefreshToken = rtoken,
                    Timeout = ttl.TotalMinutes
                });
            }

            // ====== กรณีไม่ใช้ Redis (เช่น IIS ไม่มี Redis) ======
            return Ok(new
            {
                Token = token,
                RefreshToken = rtoken,
                // ถ้าไม่ใช้ Redis แต่ยังอยากส่ง timeout ก็ดึงจาก config ได้เหมือนกัน
                Timeout = Convert.ToDouble(this.configuration["JwtExpireMinutes"])
            });
        }

        [HttpPost]
        [Route("FirstLogin")]
        public async Task<IActionResult> FistLogin([FromBody] UMS010_FistLogin_Criteria criteria)
        {
            if (criteria == null || !ModelState.IsValid)
                return BadRequest(ModelState);


            try
            {
                var result = await _ums010Services.FistLogin(criteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Invalid request." });
            }
        }



        [HttpPost]
        #region Forget password

        [HttpPost]
        [Route("ResetPassword")]

        public async Task<IActionResult> ResetPassword([FromBody] UpdateUserDo oUser)
        {
            ApplicationUser? appUser = null;
            if (oUser.Id != null)
                appUser = await this.userManager.FindByIdAsync(oUser.Id);
            else if (oUser.Email != null)
                appUser = await this.userManager.FindByEmailAsync(oUser.Email);

            if (appUser == null)
                return BadRequest("E0070");

            string token = await this.userManager.GeneratePasswordResetTokenAsync(appUser);

            string templatePath = System.IO.Path.Combine("Templates", "Template.ResetPassword.html");
            if (System.IO.File.Exists(templatePath))
            {
                string url = string.Format("{0}?i={1}&t={2}", AUTH.RESET_PASSWORD_URL, appUser.Id, System.Web.HttpUtility.UrlEncode(token));

                string contentHTML = System.IO.File.ReadAllText(templatePath);
                contentHTML = contentHTML.Replace("{Username}", appUser.UserName);
                contentHTML = contentHTML.Replace("{Url}", url);

                await this.emailService.SendAsync(new Utils.Models.EmailMessageDo()
                {
                    Subject = "Reset Password",
                    To = appUser.Email,
                    Body = contentHTML
                });
            }

            return Ok(true);
        }
        [HttpPost]
        [Route("GetUserById")]
        public async Task<IActionResult> GetUserById([FromBody] Models.ConfirmResetPasswordDo oUser)
        {
            ApplicationUser? appUser = await this.userManager.FindByIdAsync(oUser.Id);
            if (appUser == null)
                return Unauthorized("E0074");

            return Ok(new
            {
                FirstLoginFlag = appUser.FirstLoginFlag,
                Email = appUser.Email
            });
        }
        [HttpPost]
        [Route("ConfirmResetPassword")]
        public async Task<IActionResult> ConfirmResetPassword([FromBody] Models.ConfirmResetPasswordDo oUser)
        {
            ApplicationUser? appUser = await this.userManager.FindByEmailAsync(oUser.Email);
            if (appUser != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(appUser);
                var resetRes = await this.userManager.ResetPasswordAsync(appUser, oUser.Token, oUser.Password);
                if (resetRes.Succeeded == true)
                {
                    if (appUser.FirstLoginFlag == true)
                    {
                        appUser.FirstLoginFlag = false;
                        await this.userManager.UpdateAsync(appUser);
                    }
                    return Ok(true);
                }
            }

            return BadRequest("E0069");
        }

        #endregion

    }
}