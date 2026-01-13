using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceBillingSystem_API_Prod.Domain.Entities
{
    public class ServiceItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServiceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ServiceName { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Cost { get; set; }

        public bool IsActive { get; set; }
    }
}
