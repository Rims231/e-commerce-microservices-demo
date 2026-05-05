using AuthenticationApi.Application.Dtos;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationApiDbContext context, IConfiguration config) : IUser
    {
        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            try
            {
                var getUser = await GetUserByEmail(appUserDTO.Email);
                if (getUser is not null)
                    return new Response(false, "User already registered");

                var user = new AppUser
                {
                    Name = appUserDTO.Name,
                    TelephoneNumber = appUserDTO.TelephoneNumber,
                    Address = appUserDTO.Address,
                    Email = appUserDTO.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
                    Role = appUserDTO.Role
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();
                return new Response(true, "User registered successfully");
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return new Response(false, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            try
            {
                var getUser = await GetUserByEmail(loginDTO.Email);
                if (getUser is null)
                    return new Response(false, "Invalid email or password");

                bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
                if (!verifyPassword)
                    return new Response(false, "Invalid email or password");

                string token = GenerateToken(getUser);
                return new Response(true, token);
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return new Response(false, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<AppUserDTO> GetUser(int id)
        {
            try
            {
                var user = await context.Users.FindAsync(id);
                if (user is null) return null!;

                return new AppUserDTO(
                    user.Id,
                    user.Name,
                    user.TelephoneNumber,
                    user.Address,
                    user.Email,
                    user.Password,
                    user.Role);
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return null!;
            }
        }

        private async Task<AppUser?> GetUserByEmail(string email)
        {
            return await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        private string GenerateToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Authentication:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: config["Authentication:Issuer"],
                audience: config["Authentication:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}