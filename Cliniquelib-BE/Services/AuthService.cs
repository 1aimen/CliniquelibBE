
using Cliniquelib_BE.DTOs.Auth;
using Cliniquelib_BE.Helpers;
using Cliniquelib_BE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cliniquelib_BE.Services
{
    public class AuthService : IAuthService
    {
        private readonly CliniqueDbContext _context;
        private readonly string _secretKey;
        private readonly int _refreshTokenDays = 7;

        public AuthService(CliniqueDbContext context, IConfiguration config)
        {
            _context = context;
            _secretKey = config["Jwt:SecretKey"] ?? throw new Exception("JWT SecretKey not configured");
        }

        public async Task<SigninResponseDto> SignInAsync(SigninRequestDto request)
        {
            // Fetch user with roles
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !HashHelper.VerifyPassword(request.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            // Extract role names
            var roles = user.UserRoles?.Select(ur => ur.Role.Name).ToArray() ?? Array.Empty<string>();

            // Generate JWT
            var jwt = JwtHelper.GenerateToken(user.Id, roles, _secretKey);

            // Create refresh token
            var refreshToken = new Models.RefreshToken
            {
                Token = Guid.NewGuid(),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            // Return response
            return new SigninResponseDto
            {
                JwtToken = jwt,
                RefreshToken = refreshToken.Token.ToString(),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    IsActive = user.IsActive,
                    Roles = roles
                }
            };
        }


        public async Task<SignupResponseDto> SignUpAsync(SignupRequestDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("Email already registered");

            // Create user
            var user = new Models.User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                PasswordHash = HashHelper.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Assign default role (Patient)
            var defaultRoleEnum = RoleEnum.Patient;
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == defaultRoleEnum.ToString());
            if (defaultRole != null)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = defaultRole.Id
                });
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Map roles to strings for DTO
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray();

            return new SignupResponseDto
            {
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    IsActive = user.IsActive,
                    Roles = roles
                }
            };
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            if (!Guid.TryParse(request.RefreshToken, out var tokenGuid))
                throw new Exception("Invalid refresh token format");

            var token = await _context.RefreshTokens
                .Include(t => t.User)
                .ThenInclude(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(t => t.Token == tokenGuid && t.ExpiresAt > DateTime.UtcNow);

            if (token == null)
                throw new Exception("Invalid or expired refresh token");

            var roles = token.User.UserRoles.Select(ur => ur.Role.Name);
            var jwt = JwtHelper.GenerateToken(token.User.Id, roles, _secretKey);

            var newRefreshToken = Guid.NewGuid();
            token.Token = newRefreshToken;
            token.ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays);
            await _context.SaveChangesAsync();

            return new RefreshTokenResponseDto
            {
                JwtToken = jwt,
                RefreshToken = newRefreshToken.ToString()
            };
        }

        public async Task SignOutAsync(Guid userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId)
                .ToListAsync();

            _context.RefreshTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();
        }

        public async Task<Guid?> GetUserIdByRefreshTokenAsync(string refreshToken)
        {
            if (!Guid.TryParse(refreshToken, out var tokenGuid))
                return null;

            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == tokenGuid && t.ExpiresAt > DateTime.UtcNow);

            return token?.UserId;
        }
    }
}
