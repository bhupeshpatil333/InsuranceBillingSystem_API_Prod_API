using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceBillingSystem_API_Prod.Domain.Entities
{
    public class PatientInsuranceMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MappingId { get; set; }

        public int PatientId { get; set; }
        public int PolicyId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public Patient Patient { get; set; }
        public InsurancePolicy Policy { get; set; }
    }
}
