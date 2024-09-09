using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Response;

namespace AeroFlex.Repository.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync<T>(T register);
        Task<LoginResponse> SignInAsync(Login login);
        Task<UserRoleMapping?> FindUserRole(int UserId);
        Task<Role?> FindRoleName(int RoleId);
        string GenerateJwtToken(User user, string roleName);
        string GenerateRefreshToken();
        Task<User> FindByEmail(string email);
        Task<User> FindByUserName(string username);
        Task<T> AddToDatabase<T>(T model);
    }
}
