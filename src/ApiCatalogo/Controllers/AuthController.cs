using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApiCatalogo.DTOs;
using ApiCatalogo.Models;
using ApiCatalogo.Services;
using Microsoft.AspNetCore.Authorization;
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
                    new
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        RefreshToken = refreshtoken,
                        Expiration = token.ValidTo
                    }
                );
            }

            return Unauthorized();
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelDTO model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username!);

            if (userExists is not null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseDTO { Status = "Error", Message = "User already exists!" });

            }
            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            var result =
                await _userManager.CreateAsync(user, model.Password!);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseDTO { Status = "Error", Message = "User creation failed!" });
            }

            return Ok(new ResponseDTO
            {
                Status = "Success",
                Message = "User created successfully!"
            });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModelDTO tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string? accessToken = tokenModel.AccessToken
                ?? throw new ArgumentNullException(nameof(tokenModel));

            string? refreshToken =
                            tokenModel.RefreshToken ?? throw new ArgumentNullException(nameof(tokenModel));

            var principal =
                _tokenServices.GetPrincipalFromExpiredToken(accessToken,
                _configuration);

            if (principal is null)
            {
                return BadRequest("Invalid access token/refresh token");
            }

            string username = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username!);

            if (user is null || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid access token/refresh token");
            }


            var newAccessToken = _tokenServices.GenerateAccessToken(
                principal.Claims.ToList(), _configuration);

            var newRefreshToken = _tokenServices.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            await _userManager.UpdateAsync(user);

            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken,
            });
        }


        [Authorize]
        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null) { return BadRequest("Invalid user name"); }
            ;

            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);

            return NoContent();
        }
    }
}