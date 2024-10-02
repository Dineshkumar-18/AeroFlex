using AeroFlex.Models;


namespace AeroFlex.Repository.Contracts
{
    public interface IClassRepository
    {
        Task<List<Class>> GetAllClassesAsync();
        Task<Class> GetClassByNameAsync(string className);
    }

}
