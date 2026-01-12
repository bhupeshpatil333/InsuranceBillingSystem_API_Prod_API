using InsuranceBillingSystem_API_Prod.Application.DTOs.Patient;
using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using InsuranceBillingSystem_API_Prod.Domain.Entities;
using InsuranceBillingSystem_API_Prod.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InsuranceBillingSystem_API_Prod.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;

        public PatientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(CreatePatientDto dto)
        {
            var patient = new Patient
            {
                FullName = dto.FullName,
                Mobile = dto.Mobile,
                Dob = dto.Dob
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return patient.PatientId;
        }

        public async Task<List<object>> GetAllAsync()
        {
            return await _context.Patients
                .Select(p => new
                {
                    p.PatientId,
                    p.FullName,
                    p.Mobile,
                    p.Dob
                }).ToListAsync<object>();
        }

        public async Task UpdateAsync(int id, CreatePatientDto dto)
        {
            var patient = await _context.Patients.FindAsync(id)
                ?? throw new Exception("Patient not found");

            patient.FullName = dto.FullName;
            patient.Mobile = dto.Mobile;
            patient.Dob = dto.Dob;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id)
                ?? throw new Exception("Patient not found");

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }

        public async Task<object> GetByIdAsync(int id)
        {
            var patient = await _context.Patients
                .Where(p => p.PatientId == id)
                .Select(p => new
                {
                    p.PatientId,
                    p.FullName,
                    p.Mobile,
                    p.Dob
                })
                .FirstOrDefaultAsync();

            if (patient == null)
                throw new Exception("Patient not found");

            return patient;
        }

    }
}
