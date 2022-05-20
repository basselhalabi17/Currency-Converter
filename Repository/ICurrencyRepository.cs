using System.Collections.Generic;
using CurrencyConverter.Domain;

namespace CurrencyConverter.Repository
{
    public interface ICurrencyRepository
    {
        Currency GetCurrency(int Id);
        IEnumerable<Currency> GetAllCurrencies();
      
        Currency add(Currency currency);
        Currency Update(Currency cur);
        Currency Delete(int id);

        Currency GetByName(string name);

    }
}
