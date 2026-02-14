using Microsoft.EntityFrameworkCore;
using PVHealth.Data.Context;
using PVHealth.Domain.Entities;
using PVHealth.Domain.Interfaces;

namespace PVHealth.Data.Repositories;

public class SurveyRepository : IRepository<Survey>
{
    private readonly ApplicationDbContext _context;
    public SurveyRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Survey?>GetByIdAsync(Guid id)
    {
        return await _context.Surveys.Include(s=>s.Patient).FirstOrDefaultAsync(s=>s.Id == id);
    }
    public async Task<IEnumerable<Survey>> GetAllAsync()
    {
        return await _context.Surveys.Include(s=>s.Patient).ToListAsync();
    }
    public async Task<Survey> AddAsync(Survey data)
    {
        data.CreatedAt = DateTime.UtcNow;
        data.UpdatedAt = DateTime.UtcNow;
        await _context.Surveys.AddAsync(data);
        await _context.SaveChangesAsync();
        return data;
    }
    public async Task<Survey>UpdateAsync(Survey data)
    {
        data.UpdatedAt = DateTime.UtcNow;
        _context.Surveys.Update(data);
        await _context.SaveChangesAsync();
        return data;
    }
    public async Task<bool>DeleteAsync(Guid id)
    {
        var survey = await _context.Surveys.FindAsync(id);
        if (survey == null) return false;
        _context.Surveys.Remove(survey);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool>ExistsAsync(Guid id)
    {
        return await _context.Surveys.AnyAsync(s=>s.Id == id);
    }
    public async Task<Survey?> GetByPatientIdAsync(Guid id)
    {
        return await _context.Surveys.Include(s=>s.Patient).FirstOrDefaultAsync(s=>s.PatientId == id);
    }
}
