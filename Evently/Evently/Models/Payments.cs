using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Evently.Models
{
    public class Payments
    {
        [Key]
        public int PaymentId {  get; set; }
        public required decimal PayAmt { get; set; }
        public required string PayMethod { get; set; }
        public required string PayStatus { get; set; }

        [ForeignKey("UserId")]
        public required Users Users { get; set; }
    }
}
