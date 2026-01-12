using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceBillingSystem_API_Prod.Domain.Entities
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        public int BillId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PaidAmount { get; set; }

        [Required]
        [MaxLength(20)]
        public string PaymentMode { get; set; } // Cash / UPI / CreditCard / DebitCard / NetBanking

        public DateTime PaidOn { get; set; } = DateTime.UtcNow;

        // Navigation
        public Bill Bill { get; set; }
        //public string Status { get; internal set; }
        //public DateTime PaymentDate { get; internal set; }
    }
}
