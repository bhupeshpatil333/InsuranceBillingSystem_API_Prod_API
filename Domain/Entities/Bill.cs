using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceBillingSystem_API_Prod.Domain.Entities
{
    public class Bill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillId { get; set; }
        public string InvoiceNumber { get; set; }


        public int PatientId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal GrossAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal InsuranceAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal NetPayable { get; set; }

        public string Status { get; set; } // Pending / Paid

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public decimal PaidAmount { get; set; }   // 🔥 ADD THIS
        public decimal RemainingAmount { get; set; } // 🔥 ADD THIS

        // Navigation
        public Patient Patient { get; set; }
        public ICollection<BillItem> BillItems { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
