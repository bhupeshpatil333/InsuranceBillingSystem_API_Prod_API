using Humanizer;
using InsuranceBillingSystem_API_Prod.Application.DTOs.Services;
using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using InsuranceBillingSystem_API_Prod.Domain.Entities;
using InsuranceBillingSystem_API_Prod.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InsuranceBillingSystem_API_Prod.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly AppDbContext _context;

        public ServiceService(AppDbContext context)
        {
            _context = context;
        }

        // Billing screen ke liye
        public async Task<List<ServiceResponseDto>> GetAllActiveAsync()
        {
            return await _context.Services
                .Select(s => new ServiceResponseDto
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    Cost = s.Cost,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        // Admin only
        public async Task CreateAsync(CreateServiceDto dto)
        {
            var exists = await _context.Services
                .AnyAsync(s => s.ServiceName == dto.ServiceName);

            if (exists)
                throw new Exception("Service already exists");

            _context.Services.Add(new ServiceItem
            {
                ServiceName = dto.ServiceName,
                Cost = dto.Cost,
                IsActive = true
            });

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, CreateServiceDto dto)
        {
            var service = await _context.Services.FindAsync(id)
                ?? throw new Exception("Service not found");

            service.ServiceName = dto.ServiceName;
            service.Cost = dto.Cost;
            service.IsActive = dto.IsActive; // Add this line

            await _context.SaveChangesAsync();
        }

        public async Task DisableAsync(int id)
        {
            var service = await _context.Services.FindAsync(id)
                ?? throw new Exception("Service not found");

            service.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
