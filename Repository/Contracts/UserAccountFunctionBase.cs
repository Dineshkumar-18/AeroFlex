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
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public UserAccountFunctionBase(ApplicationDbContext context,IOptions<JwtSection> config,IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _config = config;
            _httpContextAccessor = contextAccessor;
        }

        public async Task<T> AddToDatabase<T>(T model)
        {
            var result = _context.Add(model);
            await _context.SaveChangesAsync();
            return (T)result.Entity;
        }

        public async Task<List<T>> AddToDatabaseRange<T>(IEnumerable<T> entities) where T : class
        {
            _context.Set<T>().AddRange(entities);
            await _context.SaveChangesAsync();
            return (List<T>)entities;
        }



        public async Task<User> FindByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));
        }

        public async Task<User> FindByUserName(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username));
        }

        //public async Task<List<Role>?> FindRoleName(List<UserRoleMapping> roleMappings)
        //{
        //    foreach (UserRoleMapping roleMapping in roleMappings) {

        //    }

        //    return await _context.Roles.Where(r => r.RoleId == roleMappings.);
        //}

        //public async Task<List<UserRoleMapping>?> FindUserRole(int UserId)
        //{
        //    return await _context.RoleMappings.Where(rm => rm.UserId == UserId).ToListAsync();
        //}

        public string GenerateJwtToken(User user, List<String> roleNames)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Value.Key!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name,user.UserName)
            };

            foreach(var role in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }

            var jwtToken = new JwtSecurityToken(
                issuer: _config.Value.Issuer,
                audience: _config.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
             );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public string GenerateRefreshToken() =>Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));


        public void AppendCookie(string jwtToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Set to true in production
                    SameSite = SameSiteMode.None, // Adjust based on your security requirements
                    Expires = DateTime.UtcNow.AddHours(1) // Set expiration
                };

                httpContext.Response.Cookies.Append("AuthToken", jwtToken, cookieOptions);
            }
        }

        public abstract Task<GeneralResponse> CreateAsync(Register register);
        public abstract Task<LoginResponse> SignInAsync(Login login);

    
    }
}
