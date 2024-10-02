using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Helpers;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;

namespace AeroFlex.Repository.Implementations
{
    public class AdminAcccountRepository : UserAccountFunctionBase, IAdminAccount
    {
        public AdminAcccountRepository(ApplicationDbContext context, IOptions<JwtSection> config, IHttpContextAccessor httpContextAccessor) : base(context, config, httpContextAccessor)
        {
        }

        public async override Task<GeneralResponse> CreateAsync(Register register)
        {
            if (register is null) return new GeneralResponse(false, "Model is invalid");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var checkUserByEmail = await FindByEmail(register.Email);
                if (checkUserByEmail is not null)
                {
                    return new GeneralResponse(false, "Email already exist");
                }
                var checkUserByUserName = await FindByUserName(register.UserName);
                if (checkUserByUserName is not null)
                {
                    return new GeneralResponse(false, "Username already exist");
                }

                var user = await AddToDatabase(new Admin
                {
                    UserName = register.UserName,
                    Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                    Email = register.Email,
                    FirstName = register.FirstName,
                    LastName = register.LastName,
                    PhoneNumber = register.PhoneNumber,
                });

                var roles = await _context.Roles
                    .Where(r => r.RoleName.Equals(Roles.Admin.ToString()) || r.RoleName.Equals(Roles.User.ToString()))
                    .ToListAsync();

                if (!roles.Any()) return new GeneralResponse(false, "Roles not found");

                //ensure both roles are found
                var adminRole = roles.FirstOrDefault(r => r.RoleName.Equals(Roles.Admin.ToString()));
                var UserRole = roles.FirstOrDefault(r => r.RoleName.Equals(Roles.User.ToString()));

                if (adminRole == null) return new GeneralResponse(false, "Admin role not found");
                if (UserRole == null) return new GeneralResponse(false, "User role not found");

                var roleMappings = new List<UserRoleMapping>
                   {
                         new UserRoleMapping
                        {
                            UserId = user.UserId,
                            RoleId = adminRole.RoleId
                        },
                        new UserRoleMapping
                        {
                            UserId = user.UserId,
                            RoleId = UserRole.RoleId
                        }
                   };

                var mappedRoles = await AddToDatabaseRange(roleMappings);
                if (mappedRoles.Count != 2) return new GeneralResponse(false, "Error while role mapping");

                await transaction.CommitAsync();

                return new GeneralResponse(true, "Admin Registered Successfully");
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                return new GeneralResponse(false, $"Error while registering admin: {ex.Message}");
            }
        }

        public override async Task<LoginResponse> SignInAsync(Login login)
        {
            if (login == null) return new LoginResponse(false, "Model is invalid");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {


                var user = await _context.Admins
                          .Include(u => u.RoleMappings)
                          .ThenInclude(urm => urm.Role)
                          .FirstOrDefaultAsync(u => u.Email == login.Email || u.UserName == login.Email);
                if (user == null)
                {
                    return new LoginResponse(false, "Username or Email doesnot exist");
                }
                else if (!BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                {
                    return new LoginResponse(false, "Username or Password is invalid");
                }
                else
                {
                    var roleNames = user.RoleMappings.Select(urm => urm.Role.RoleName).ToList();

                    string jwtToken = GenerateJwtToken(user, roleNames);
                    AppendCookie(jwtToken);
                    string refreshToken = GenerateRefreshToken();

                    var refreshTokenInfo = await _context.RefreshTokenInfos.FirstOrDefaultAsync(rt => rt.UserId == user.UserId);

                    if (refreshTokenInfo is not null)
                    {
                        refreshTokenInfo.RefreshToken = refreshToken;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        await AddToDatabase(new RefreshTokenInfo
                        {
                            RefreshToken = refreshToken,
                            ExpirationTime = DateTime.Now.AddDays(1),
                            UserId = user.UserId,
                        });
                    }
                    await transaction.CommitAsync();

                    return new LoginResponse(true, "Login succeffully", jwtToken, refreshToken);
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log the exception if needed
                return new LoginResponse(false, $"Error while signing in: {ex.Message}");
            }


        }
    }
}
