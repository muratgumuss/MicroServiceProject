using System.ComponentModel.DataAnnotations;

namespace MicroServiceApp.Web.Pages.Order.ViewModel
{

    public record PaymentViewModel
    {
        [Display(Name = "Card Number")] public string CardNumber { get; set; } = null!;

        [Display(Name = "Cardholder Name")] public string CardHolderName { get; set; } = null!;

        [Display(Name = "Expiry Date")] public string ExpiryDate { get; set; } = null!;

        [Display(Name = "CVV")] public string Cvv { get; set; } = null!;

        [Display(Name = "Payment Amount")] public decimal Amount { get; set; }

        public static PaymentViewModel Empty => new();
    }
}
