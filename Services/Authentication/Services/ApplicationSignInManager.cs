using Application.Models;
using Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Services
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        private readonly ApplicationUserManager userManager;
        private readonly IConfiguration configuration;


        public ApplicationSignInManager(UserManager<ApplicationUser> userManager,
                                        IHttpContextAccessor contextAccessor,
                                        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
                                        IOptions<IdentityOptions> optionsAccessor,
                                        ILogger<SignInManager<ApplicationUser>> logger,
                                        Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider schemes,
                                        IUserConfirmation<ApplicationUser> confirmation,
                                        IConfiguration configuration


            )
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            this.userManager = userManager as ApplicationUserManager;
            this.configuration = configuration;

        }

        public async Task<string> GenerateToken(ApplicationUser user, string LanguageCode, string Device, string WhereAllData)
        {
            UserInfoDo userInfo = this.userManager.GetUserInfo(user);
            string? ApplyWarehouseBypass(string? warehouseByPassCsv, string? input)
            {
                if (string.IsNullOrWhiteSpace(input)) return null;

                // ถ้าไม่มี bypass list ก็คืน input ไปเลย
                if (string.IsNullOrWhiteSpace(warehouseByPassCsv))
                    return input.Trim();

                var bypassSet = warehouseByPassCsv
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                // ถ้า input อยู่ใน bypass list => null
                return bypassSet.Contains(input.Trim()) ? "" : input.Trim();
            }

            if (ApplyWarehouseBypass(WhereAllData, userInfo.PositionCode) == "")
            {
                userInfo.DepartmentCode = "";
            }




            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("UserNumber", userInfo?.UserNumber != null ? userInfo?.UserNumber.ToString() : ""),
                new Claim("FlagSystemAdmin", user.SystemAdminFlag == true ? "true" : "false"),
                new Claim("username", user.UserName),
                new Claim("Warehouse", userInfo.DepartmentCode),
                new Claim("Company", userInfo.PositionCode),
                new Claim("LanguageCode", LanguageCode),
                new Claim("typeService", "API"),
                new Claim("device",Device),
            };

            //var roles = await this.userManager.GetRolesAsync(user);
            //foreach (string roleName in roles)
            //{
            //    var role = await this.roleManager.FindByNameAsync(roleName);
            //    if (role.AppCode == appCode
            //        && role.ActiveFlag == true)
            //    {
            //        claims.Add(new Claim(ClaimTypes.Role, roleName));

            //        foreach (var claim in await this.roleManager.GetClaimsAsync(role))
            //        {
            //            if (claims.Exists(x => x.Value == claim.Value) == false)
            //                claims.Add(claim);
            //        }
            //    }
            //}

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(this.configuration["JwtExpireMinutes"]));

            var token = new JwtSecurityToken(
                this.configuration["JwtIssuer"],
                this.configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
