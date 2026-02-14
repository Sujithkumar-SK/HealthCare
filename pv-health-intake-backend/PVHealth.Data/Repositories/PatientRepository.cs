using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using PVHealth.Data.Context;
using PVHealth.Domain.Entities;
using PVHealth.Domain.Interfaces;

namespace PVHealth.Data.Repositories;
public class PatientRepository: IRepository<Patient>
{
    private readonly ApplicationDbContext _context;
    public PatientRepository(ApplicationDbContext context)
    {
        _context=context;
    }
    public async Task<Patient?>GetByIdAsync(Guid id)
    {
        return await _context.Patients.Include(p=>p.Survey).FirstOrDefaultAsync(p=>p.Id == id);
    }
    public async Task<IEnumerable<Patient>>GetAllAsync()
    {
        return await _context.Patients.Include(p=>p.Survey).ToListAsync();
    }
    public async Task<Patient>AddAsync (Patient data)
    {
        data.CreatedAt = DateTime.UtcNow;
        data.UpdatedAt = DateTime.UtcNow;
        await _context.Patients.AddAsync(data);
        await _context.SaveChangesAsync();
        return data;
    }
    public async Task<Patient> UpdateAsync(Patient data)
    {
        data.UpdatedAt = DateTime.UtcNow;
        _context.Patients.Update(data);
        await _context.SaveChangesAsync();
        return data;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if(patient == null) return false;
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Patients.AnyAsync(p=>p.Id == id);
    }
}