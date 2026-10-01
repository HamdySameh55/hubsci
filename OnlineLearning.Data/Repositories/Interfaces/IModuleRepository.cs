using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface IModuleRepository
{
    Task<IEnumerable<Module>> GetAllAsync();

    Task<Module?> GetByIdAsync(int id);

    Task AddAsync(Module module);

    void Update(Module module);

    void Delete(Module module);

    Task SaveChangesAsync();
}