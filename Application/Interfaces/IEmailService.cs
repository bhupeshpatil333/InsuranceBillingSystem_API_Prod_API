namespace InsuranceBillingSystem_API_Prod.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            byte[] attachment,
            string fileName
        );
    }
}
