﻿using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Helpers;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AeroFlex.Repository.Implementations
{
    public class UserAccountRepository : UserAccountFunctionBase
    {
        public UserAccountRepository(ApplicationDbContext context,IOptions<JwtSection> config) : base(context,config)
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

                var user = await AddToDatabase(new User
                {
                    UserName = register.UserName,
                    Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                    Email = register.Email,
                    FirstName = register.FirstName,
                    LastName = register.LastName,
                    PhoneNumber = register.PhoneNumber
                });

                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName.Equals(Roles.User.ToString()));
                if (userRole == null) return new GeneralResponse(false, "User role not found");

                var userRoleMapping = await AddToDatabase(new UserRoleMapping
                {
                    UserId = user.UserId,
                    RoleId = userRole.RoleId
                });

                return new GeneralResponse(true, "User Registered Successfully");

        }



        public override async Task<LoginResponse> SignInAsync(Login login)
        {
            if (login == null) return new LoginResponse(false, "Model is invalid");


            var user = await _context.Users
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

                return new LoginResponse(true, "Login succeffully", jwtToken, refreshToken);
            }

        }
    }

}
