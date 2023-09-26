using AutoMapper;
using Entities.ConfigurationModel;
using Entities.Exceptions;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DataTranasferObject;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<Users> _userManager;
        private readonly IOptions<JwtConfiguration> _configuration;
        private readonly JwtConfiguration _jwtConfiguration;
        private Users? _users;

        public AuthenticationService(ILoggerManager logger, IMapper mapper, UserManager<Users> userManager, IOptions<JwtConfiguration> configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _jwtConfiguration = _configuration.Value;
            //_jwtConfiguration = new JwtConfiguration();
            //_configuration.Bind(_jwtConfiguration.Section, _jwtConfiguration);
        }
        public async Task<IdentityResult> RegisterUser(UsersForRegistrationDto usersForRegistrationDto)
        {
            var user = _mapper.Map<Users>(usersForRegistrationDto);
            var result = await _userManager.CreateAsync(user, usersForRegistrationDto.Password);
            if (result.Succeeded)
                await _userManager.AddToRolesAsync(user, usersForRegistrationDto.Roles);
            return result; 
        }
        public async Task<bool>ValidateUser(UsersForAuthenticationDto usersForAuthenticationDto)
        {
            _users = await _userManager.FindByNameAsync(usersForAuthenticationDto.UserName);

            var result = (_users != null && await _userManager.CheckPasswordAsync(_users, usersForAuthenticationDto.Password));

            if (!result)
            {
                _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. Wrong username and password.");
            }
            return result;

        }
        public async Task<TokenDto> CreateToken(bool populateExp)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var refreshToken = GenerateFreshToken();
            _users.RefreshToken = refreshToken;
            if (populateExp)
            {
                _users.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
               
            }
            await _userManager.UpdateAsync(_users);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new TokenDto(accessToken, refreshToken);
            //return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
        public SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"));
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _users.UserName)
            };
            var roles = await _userManager.GetRolesAsync(_users);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
        public JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            //var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenOptions = new JwtSecurityToken(
                issuer : _jwtConfiguration.ValidIssuer,
                audience: _jwtConfiguration.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
                signingCredentials: signingCredentials
                );
            return tokenOptions;
        }
        private string GenerateFreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            //var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"))),
                ValidateLifetime = true,
                ValidIssuer = _jwtConfiguration.ValidIssuer,
                ValidAudience = _jwtConfiguration.ValidAudience
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken =securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }
            return principal;
        }
        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

            var user = await _userManager.FindByNameAsync(principal.Identity.Name); 
            if (user == null || user.RefreshToken != tokenDto.RefreshToken||user.RefreshTokenExpiryTime <= DateTime.Now )
            {
                throw new RefreshTokenBadRequest();
            }
            _users = user;
            return await CreateToken(populateExp: false);
        }
    }
}
