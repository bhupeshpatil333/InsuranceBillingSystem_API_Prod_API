using InsuranceBillingSystem_API_Prod.Application.DTOs.Services;

namespace InsuranceBillingSystem_API_Prod.Application.Interfaces
{
    public interface IServiceService
    {
        Task<List<ServiceResponseDto>> GetAllActiveAsync();
        Task CreateAsync(CreateServiceDto dto);
        Task UpdateAsync(int id, CreateServiceDto dto);
        Task DisableAsync(int id);
    }
}
