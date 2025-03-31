using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApiCatalogo.DTOs;
using ApiCatalogo.Models;
using ApiCatalogo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenServices _tokenServices;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _configuration;


        public AuthController(
            ITokenServices tokenServices,
            UserManager<ApplicationUser> userManager,
             RoleManager<IdentityRole> roleManager,
             IConfiguration configuration)
        {
            _tokenServices = tokenServices;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelDTO model)
        {

            var user = await _userManager.FindByNameAsync(model.UserName!);

            if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>{
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _tokenServices.GenerateAccessToken(authClaims, _configuration);

                var refreshtoken = _tokenServices.GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValiditInMinutes"],
                out int refreshTokenValiditInMinutes);

                user.RefreshTokenExpiryTime =
                DateTime.Now.AddMinutes(refreshTokenValiditInMinutes);

                user.RefreshToken = refreshtoken;

                await _userManager.UpdateAsync(user);

                return Ok(
                    new{
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        RefreshToken = refreshtoken,
                        Expiration = token.ValidTo
                    }
                );
            }

            return Unauthorized();
        }
    }
}