using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiCatalogo.Services
{
    public interface ITokenServices
    {
        JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims,
        IConfiguration _config);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token,
        IConfiguration _config);
    }
}