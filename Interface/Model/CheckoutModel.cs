using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model
{
    public class CheckoutModel
    {

        public string? CartId { get; set; }   // The ID of the cart being checked out.
        public string UserId { get; set; }   // The ID of the user making the purchase.
        public string? ShippingAddress { get; set; }   // The address where the order will be shipped.
        public string? BillingAddress { get; set; }    // The billing address associated with the payment.
        public string? PaymentMethod { get; set; }   // The payment method selected (e.g., Credit Card, PayPal).
        public string? CreditCardNumber { get; set; }   // The credit card number (if applicable).
        public string CreditCardExpiration { get; set; }   // Expiration date of the credit card.
        public string? CreditCardCVC { get; set; }   // The CVC code for the credit card.
        public decimal OrderTotal { get; set; }   // The total amount of the order.
        public IEnumerable<CartItemModel> CartItems { get; set; }   // The list of items in the cart.
        public DateTime OrderDate { get; set; }   // The date when the orde

        public int Customerid { get; set; }
        public string? DiscountCode { get; set; }
    }
}
