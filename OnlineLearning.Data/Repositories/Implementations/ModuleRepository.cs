using Microsoft.EntityFrameworkCore;
using OnlineLearning.Data.Context;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Implementations;

public class ModuleRepository : IModuleRepository
{
    private readonly ApplicationDbContext _context;

    public ModuleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Module>> GetAllAsync()
    {
        return await _context.Modules.ToListAsync();
    }

    public async Task<Module?> GetByIdAsync(int id)
    {
        return await _context.Modules.FindAsync(id);
    }

    public async Task AddAsync(Module module)
    {
        await _context.Modules.AddAsync(module);
    }

    public void Update(Module module)
    {
        _context.Modules.Update(module);
    }

    public void Delete(Module module)
    {
        _context.Modules.Remove(module);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}