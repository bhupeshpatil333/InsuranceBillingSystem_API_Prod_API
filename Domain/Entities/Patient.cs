using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceBillingSystem_API_Prod.Domain.Entities
{
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(15)]
        public string Mobile { get; set; }

        public DateTime Dob { get; set; }

        public string Email { get; set; }   // ✅ REQUIRED

        // Navigation
        public ICollection<Bill> Bills { get; set; }
        public ICollection<PatientInsuranceMapping> InsuranceMappings { get; set; }
    }
}
