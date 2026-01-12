using InsuranceBillingSystem_API_Prod.Application.DTOs.Patient;

namespace InsuranceBillingSystem_API_Prod.Application.Interfaces
{
    public interface IPatientService
    {
        Task<int> CreateAsync(CreatePatientDto dto);
        Task<List<object>> GetAllAsync();
        Task UpdateAsync(int id, CreatePatientDto dto);
        Task DeleteAsync(int id);
        Task<object> GetByIdAsync(int id);

    }
}
