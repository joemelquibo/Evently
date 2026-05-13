using System.ComponentModel.DataAnnotations;

namespace Evently.Models
{
    public class CashInViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 100000, ErrorMessage = "Amount must be between ₱1 and ₱100,000")]
        public decimal Amount { get; set; }

        // Populated in the GET action so the view can show current balance
        public decimal CurrentBalance { get; set; }

        // The selected payment method (GCash, Maya, BDO, etc.) — display only, not saved to DB
        public string? PaymentMethod { get; set; }
    }
}