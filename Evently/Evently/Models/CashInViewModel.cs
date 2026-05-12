using System.ComponentModel.DataAnnotations;

namespace Evently.Models
{
    public class CashInViewModel
    {
        public int UserId { get; set; }

        [Range(1, 10000, ErrorMessage = "Please enter an amount between 1 and 10000")]
        public decimal Amount { get; set; }
    }
}
