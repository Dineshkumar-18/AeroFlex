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
    public class FlightOwnerAccountRepository : UserAccountFunctionBase, IFlightOwnerAccount
    {
        public FlightOwnerAccountRepository(ApplicationDbContext context, IOptions<JwtSection> config, IHttpContextAccessor httpContextAccessor) : base(context, config, httpContextAccessor)
        {

        }

        public override async Task<GeneralResponse> CreateAsync(Register register)
        {
            
           if (register is null) return new GeneralResponse(false, "Model is invalid");

          using var transaction=await _context.Database.BeginTransactionAsync();
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

                var user = await AddToDatabase(new FlightOwner
                {
                    UserName = register.UserName,
                    Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                    Email = register.Email,
                    FirstName = register.FirstName,
                    LastName = register.LastName,
                    PhoneNumber = register.PhoneNumber,
                });


                var roles = await _context.Roles
                .Where(r => r.RoleName.Equals(Roles.FlightOwner.ToString()) || r.RoleName.Equals(Roles.User.ToString()))
                .ToListAsync();

                if (!roles.Any()) return new GeneralResponse(false, "Roles not found");

                //ensure both roles are found
                var FlightOwnerRole = roles.FirstOrDefault(r => r.RoleName.Equals(Roles.FlightOwner.ToString()));
                var UserRole = roles.FirstOrDefault(r => r.RoleName.Equals(Roles.User.ToString()));

                if (FlightOwnerRole == null) return new GeneralResponse(false, "FlightOwner role not found");
                if (UserRole == null) return new GeneralResponse(false, "User role not found");

                var roleMappings = new List<UserRoleMapping>
                {
                     new UserRoleMapping
                    {
                        UserId = user.UserId,
                        RoleId = FlightOwnerRole.RoleId
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
                return new GeneralResponse(true, "Flight Owner Registered Successfully");
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                // Log the exception if needed
                return new GeneralResponse(false, $"Error while signing in: {ex.Message}");
            }
        }



        public override async Task<LoginResponse> SignInAsync(Login login)
        {
            if (login == null) return new LoginResponse(false, "Model is invalid");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.FlightOwners
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
