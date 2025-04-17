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

        private readonly ILogger<AuthController> _logger;


        public AuthController(
            ITokenServices tokenServices,
            UserManager<ApplicationUser> userManager,
             RoleManager<IdentityRole> roleManager,
             IConfiguration configuration,
             ILogger<AuthController> logger)
        {
            _tokenServices = tokenServices;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist)
            {
                var roleResult =
                await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation(1, "Roles Added");

                    return StatusCode(StatusCodes.Status200OK,
                    new ResponseDTO
                    {
                        Status = "Success",
                        Message = $"Role {roleName} added successfully"
                    });
                }
                else
                {
                    _logger.LogInformation(2, "Error");

                    return StatusCode(StatusCodes.Status400BadRequest,
                    new ResponseDTO
                    {
                        Status = "Error",
                        Message = $"Issue adding the new {roleName} role"
                    });
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest,
                               new ResponseDTO
                               {
                                   Status = "Error",
                                   Message = $"Role already exist"
                               });
        }

        [HttpPost]
        [Route("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is not null)
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);

                if (result.Succeeded)
                {
                    _logger.LogInformation(1, $"User {user.Email} added to the {roleName} role");

                    return StatusCode(StatusCodes.Status200OK,
                        new ResponseDTO
                        {
                            Status = "Success",
                            Message = $"User {user.Email} added to the {roleName} role"
                        });
                }
                else
                {
                    _logger.LogInformation(1, 
                        $"Error: Unable to add user {user.Email} to the {roleName} role");
                    return StatusCode(StatusCodes.Status400BadRequest, 
                        new ResponseDTO
                        {
                            Status = "Error",
                            Message = $"Error: Unable to add user {user.Email} to the {roleName} role"
                        });
                }
            }

            return BadRequest(new { error = "Unable to find user"});
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
                new Claim("id", user.UserName!),
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

            ////// retirando os valores do model
            string? accessToken = tokenModel.AccessToken
                ?? throw new ArgumentNullException(nameof(tokenModel));

            string? refreshToken =
                            tokenModel.RefreshToken ?? throw new ArgumentNullException(nameof(tokenModel));
            /////

            var principal =
                _tokenServices.GetPrincipalFromExpiredToken(accessToken,
                _configuration);

            if (principal is null)
            {
                return BadRequest("Invalid access token/refresh token");
            }

            string username = principal.Identity!.Name ?? string.Empty;

            var user = await _userManager.FindByNameAsync(username!);

            if (user == null || user.RefreshToken != refreshToken
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