using AutoMapper;
using CatalogServiceSecurityApp.Models.DbModels;
using CatalogServiceSecurityApp.Models.InputModels;
using CatalogServiceSecurityApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CatalogServiceSecurityApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IMapper mapper;
        private readonly ILogger logger;
        private readonly AuthService authService;
        private readonly IConfiguration configuration;

        public AuthController(ILogger logger, AuthService authService, IMapper mapper, IConfiguration configuration)
        {
            this.authService = authService;
            this.mapper = mapper;
            this.logger = logger;
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public ActionResult<string> Login(LoginInputModel userModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (this.authService.IsAuthenticated(userModel.Email, userModel.Password))
                    {
                        var user = this.authService.GetByEmail(userModel.Email);
                        var accessToken = this.authService.GenerateJwtToken(userModel.Email, user.Role);
                        var refreshToken = this.authService.GenerateRefreshToken();

                        return Ok(Json(new
                        {
                            AccessToken = accessToken,
                            RefreshToken = refreshToken
                        }));
                    }
                    return BadRequest("Email or password are not correct!");
                }

                return BadRequest(ModelState);
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
                return StatusCode(500);
            }
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public ActionResult<string> Register(RegisterInputModel userModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (userModel.Password != userModel.ConfirmedPassword)
                    {
                        return BadRequest("Passwords does not match!");
                    }

                    if (this.authService.DoesUserExists(userModel.Email))
                    {
                        return BadRequest("User already exists!");
                    }

                    var mappedModel = this.mapper.Map<RegisterInputModel, User>(userModel);
                    mappedModel.Role = "User";
                    var user = this.authService.RegisterUser(mappedModel);

                    if (user != null)
                    {
                        var token = this.authService.GenerateJwtToken(user.Email, mappedModel.Role);
                        var refreshToken = this.authService.GenerateRefreshToken();

                        return Ok(Json(new
                        {
                            AccessToken = token,
                            RefreshToken = refreshToken
                        }));

                    }

                    return BadRequest("Email or password are not correct!");
                }

                return BadRequest(ModelState);
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            string username = principal.Identity.Name;

            var user = authService.GetByUserName(username);

            if (user == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            var newAccessToken = authService.GenerateJwtToken(user.Email,user.Role);
            var newRefreshToken = authService.GenerateRefreshToken();


            return new ObjectResult(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }
    }
}
