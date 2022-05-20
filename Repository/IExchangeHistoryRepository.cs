using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyConverter.Domain;

namespace CurrencyConverter.Repository
{
    public interface IExchangeHistoryRepository
    {
        ExchangeHistory Edit(ExchangeHistory cur);
        ExchangeHistory add(ExchangeHistory exc);
        ExchangeHistory GetHist(int Id);


        IEnumerable<ExchangeHistory> GetAllHistOfCurrency(DateTime d, DateTime e);
        ExchangeHistory GetHistByName(string name);



    }
}
