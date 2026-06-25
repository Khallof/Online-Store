using Store.Core.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Payment
{
    public class PaymentCreateDto
    {

        [Required(ErrorMessage = "Order is required")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [RegularExpression("^(CreditCard|DebitCard|PayPal|BankTransfer|Cash|Other)$",
            ErrorMessage = "Invalid payment method")]
        public string PaymentMethod { get; set; } = string.Empty;

        public CustomerDto? Customer { get; set; }
    }
}
