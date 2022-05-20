using System.ComponentModel.DataAnnotations;

namespace CurrencyConverter.Domain
{
    public class Currency
    {
        public int Id { get; set; }
        [Required, MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }
        [Required]
        public string Sign { get; set; }
        public bool IsActive { get; set; }
    }
}
