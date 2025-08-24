using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Cliniquelib_BE.Helpers;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Cliniquelib_BE.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CliniqueDbContext _context;

        public JwtMiddleware(RequestDelegate next, CliniqueDbContext context)
        {
            _next = next;
            _context = context;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
                await AttachUserToContext(context, token);

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var principal = JwtHelper.ValidateToken(token, "YOUR_VERY_SECURE_SECRET_KEY_HERE"); // replace with config
                if (principal == null) return;

                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.Sub);
                if (userIdClaim == null) return;

                if (!Guid.TryParse(userIdClaim.Value, out var userId)) return;

                // Fetch user from DB
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

                if (user == null) return;

                // Attach user info to HttpContext for controllers
                context.Items["User"] = user;
            }
            catch
            {
                // Do nothing if token validation fails
                // user is not attached to context so request won't have access
            }
        }
    }

    // Extension method to add middleware easily
    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }
}
