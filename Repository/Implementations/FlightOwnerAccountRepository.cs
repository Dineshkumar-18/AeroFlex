using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Helpers;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AeroFlex.Repository.Implementations
{
    public class FlightOwnerAccountRepository : UserAccountFunctionBase
    {
        public FlightOwnerAccountRepository(ApplicationDbContext context, IOptions<JwtSection> config) : base(context, config)
        {

        }

        public override async Task<GeneralResponse> CreateAsync(Register register)
        {
            
                if (register is null) return new GeneralResponse(false, "Model is invalid");

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

                var user = await AddToDatabase(new FlightOwner
                {
                    UserName = register.UserName,
                    Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                    Email = register.Email,
                    FirstName = register.FirstName,
                    LastName = register.LastName,
                    PhoneNumber = register.PhoneNumber,
                });


                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName.Equals(Roles.FlightOwner.ToString()));
                if (userRole == null) return new GeneralResponse(false, "User role not found");

                var userRoleMapping = await AddToDatabase(new UserRoleMapping
                {
                    UserId = user.UserId,
                    RoleId = userRole.RoleId
                });
                return new GeneralResponse(true, "Flight Owner Registered Successfully");
        }



        public override async Task<LoginResponse> SignInAsync(Login login)
        {
            if (login == null) return new LoginResponse(false, "Model is invalid");
            User? user = null;
            var userEmail = await FindByEmail(login.Email);
            var userName = await FindByUserName(login.Email);
            if (userEmail is not null) user = userEmail;
            else if (userName is not null) user = userName;

            if (user is null)
            {
                return new LoginResponse(false, "Username or Email doesnot exist");
            }
            if ((user.Email == login.Email || user.UserName == login.Email) && BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                var role = await FindUserRole(user.UserId);
                var roleName = await FindRoleName(role!.RoleId);

                string jwtToken = GenerateJwtToken(user, roleName!.RoleName);
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
                        UserId = user.UserId,
                    });
                }

                return new LoginResponse(true, "Login succeffully", jwtToken, refreshToken);
            }
            return new LoginResponse(false, "Email or Password is invalid");

        }
    }
}
