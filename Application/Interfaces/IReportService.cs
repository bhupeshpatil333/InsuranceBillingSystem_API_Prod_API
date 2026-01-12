using InsuranceBillingSystem_API_Prod.Application.DTOs.Reports;

namespace InsuranceBillingSystem_API_Prod.Application.Interfaces
{
    public interface IReportService
    {
        //Task<List<ReportResultDto>> GetBillingReportAsync();
        Task<BillingReportDto> GetBillingReportAsync(DateTime from, DateTime to);

    }
}
