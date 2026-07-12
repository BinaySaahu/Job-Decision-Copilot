using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JobDecisionEngine.Data;
using JobDecisionEngine.DTOs;
using JobDecisionEngine.Models;
using BC = BCrypt.Net.BCrypt;

namespace JobDecisionEngine.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

            if (existingUser != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email already registered"
                };
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Password must be at least 6 characters"
                };
            }

            // Create new user
            var user = new User
            {
                Email = request.Email.ToLower(),
                PasswordHash = BC.HashPassword(request.Password),
                FullName = request.FullName ?? "User",
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = await GenerateRefreshToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsOnboarded = user.IsOnBoarded
                }
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

            if (user == null || !BC.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            if (!user.IsActive)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Account is inactive"
                };
            }

            // Generate tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = await GenerateRefreshToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsOnboarded = user.IsOnBoarded
                }
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiryDate < DateTime.UtcNow)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid or expired refresh token"
                };
            }

            var user = refreshToken.User;
            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = await GenerateRefreshToken(user);

            // Revoke old refresh token
            refreshToken.IsRevoked = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Token refreshed",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName
                }
            };
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return validatedToken != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null)
                return false;

            token.IsRevoked = true;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();

            return true;
        }

        private string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FullName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> GenerateRefreshToken(User user)
        {
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken.Token;
        }
    }
}