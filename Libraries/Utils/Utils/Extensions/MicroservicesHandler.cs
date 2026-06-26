using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Utils.Extensions
{
    public class MicroservicesHandler : DelegatingHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MicroservicesHandler(IConfiguration configuration, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateToken()
        {
            if (_cache.TryGetValue("MicroserviceToken", out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
            {
                return cachedToken;
            }

            var claims = new List<Claim>
            {
                new Claim("typeService", "micoService"),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtExpireMinutes"]));

            var token = new JwtSecurityToken(
               issuer: _configuration["JwtIssuer"],
               audience: _configuration["JwtIssuer"],
               claims,
               expires: expires,
               signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            // Cache Token จนกว่าจะหมดอายุ
            _cache.Set("MicroserviceToken", tokenString, TimeSpan.FromMinutes(Convert.ToDouble(_configuration["JwtExpireMinutes"]) - 5));
            return tokenString;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = GenerateToken();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var tenantDB = _httpContextAccessor.HttpContext?.Request.Headers["X-ConnectionStringDB"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantDB))
            {
                request.Headers.Add("X-ConnectionStringDB", tenantDB);
            }
            var tenantCustomerID = _httpContextAccessor.HttpContext?.Request.Headers["X-CustomerID"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantCustomerID))
            {
                request.Headers.Add("X-CustomerID", tenantCustomerID);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}

