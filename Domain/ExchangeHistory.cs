using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CurrencyConverter.Domain
{
    public class ExchangeHistory
    {
        public int Id { get; set; }
        [Required]
        public DateTime ExchangeDate { get; set; }
        [Required]
        public float Rate { get; set; }

        public int CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public Currency Currency { get; set; }

    }
}
