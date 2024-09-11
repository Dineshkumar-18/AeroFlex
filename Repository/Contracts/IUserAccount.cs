using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Response;

namespace AeroFlex.Repository.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(Register register);
        Task<LoginResponse> SignInAsync(Login login);

        //Task<List<UserRoleMapping>?> FindUserRole(int UserId);
        //Task<List<Role>?> FindRoleName(List<UserRoleMapping> RoleMapping);
        string GenerateJwtToken(User user, List<String> roleNames);
        string GenerateRefreshToken();
        Task<User> FindByEmail(string email);
        Task<User> FindByUserName(string username);
        Task<T> AddToDatabase<T>(T model);
        Task<List<T>> AddToDatabaseRange<T>(IEnumerable<T> entities) where T : class;
        void AppendCookie(string jwtToken);
    }
}
