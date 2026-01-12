using Microsoft.AspNetCore.Identity;

namespace InsuranceBillingSystem_API_Prod.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(IdentityUser user, IList<string> roles);
    }
}
