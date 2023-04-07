namespace Basket.Api.Models
{
    public class BasketCheckout
    {
        [Required]
        public string? City { get; set; }

        [Required]
        public string? Street { get; set; }

        [Required]
        public string? State { get; set; }

        [Required]
        public string? ZipCode { get; set; }

        [Required]
        public string? CardNumber { get; set; }

        [Required]
        public string? CardHolder { get; set; }

        public DateTime CardExpiration { get; set; }

        [Required]
        public string? CardSecurityNumber { get; set; }

        public int CardTypeId { get; set; }
    }
}