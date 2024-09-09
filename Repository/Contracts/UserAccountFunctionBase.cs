using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Helpers;
using AeroFlex.Models;
using AeroFlex.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AeroFlex.Repository.Contracts
{
    public abstract class UserAccountFunctionBase : IUserAccount
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IOptions<JwtSection> _config;
        public UserAccountFunctionBase(ApplicationDbContext context,IOptions<JwtSection> config)
        {
            _context = context;
            _config = config;
        }

        public async Task<T> AddToDatabase<T>(T model)
        {
            var result = _context.Add(model);
            await _context.SaveChangesAsync();
            return (T)result.Entity;
        }



        public async Task<User> FindByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));
        }

        public async Task<User> FindByUserName(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username));
        }

        public async Task<Role?> FindRoleName(int RoleId)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == RoleId);
        }

        public async Task<UserRoleMapping?> FindUserRole(int UserId)
        {
            return await _context.RoleMappings.FirstOrDefaultAsync(rm => rm.UserId == UserId);
        }

        public string GenerateJwtToken(User user, string roleName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Value.Key!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Role,roleName)
            };

            var jwtToken = new JwtSecurityToken(
                issuer: _config.Value.Issuer,
                audience: _config.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
             );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public string GenerateRefreshToken() =>Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        public abstract Task<GeneralResponse> CreateAsync<T>(T register);
        public abstract Task<LoginResponse> SignInAsync(Login login);

    
    }
}
