using System.Security.Claims;

namespace Utils.Extensions
{
    public class ClaimHelper
    {
        public static string? GetClaim(IEnumerable<Claim> claims, string type)
        {
            return claims
                    .Where(x => x.Type == type)
                    .Select(x => x.Value).FirstOrDefault();
        }
        public static int GetUserNumber(IEnumerable<Claim> claims)
        {
            int number = 0;

            string? val = GetClaim(claims, "UserNumber");
            if (val != null)
                int.TryParse(val, out number);

            return number;
        }
    }
}
