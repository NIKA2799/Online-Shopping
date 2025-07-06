using Dto;
using Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Inject configuration to access JWT settings (Key, Issuer, Audience).
        /// </summary>
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Creates a signed JWT token string containing user and role claims.
        /// </summary>
        /// <param name="user">The authenticated user for whom the token is generated.</param>
        /// <param name="roles">List of user roles to include as claims.</param>
        public string GenerateToken(ApplicationUser user, IList<string> roles)
        {
            // Initialize base claims: username, user ID, and a unique token identifier (JTI)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),              // User's username
                new Claim(ClaimTypes.NameIdentifier, user.Id),           // User's unique ID
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique identifier for this token
            };

            // Append each role as a separate claim
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Retrieve the secret key from configuration and create a symmetric security key
            var secretKey = _configuration["Jwt:Key"];
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);

            // Create signing credentials using HMAC-SHA256 algorithm
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Build the token with issuer, audience, claims, expiry, and signing credentials
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],     // Token issuer (e.g., your API)
                audience: _configuration["Jwt:Audience"], // Intended audience (e.g., your client app)
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),      // Token validity period (2 hours)
                signingCredentials: signingCredentials
            );

            // Serialize token to a compact string and return
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}