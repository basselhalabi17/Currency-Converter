using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyConverter.Repository;
using CurrencyConverter.Domain;
//using CurrencyConverter.Models;

namespace CurrencyConverter.Models
{
    public class SQLExchangeHistoryRepository : IExchangeHistoryRepository
    {
        private readonly AppDbContext context;
        public SQLExchangeHistoryRepository(AppDbContext context)
        {
            this.context = context;
        }

        public ExchangeHistory add(ExchangeHistory exc)
        {
            context.ExchangeHistory.Add(exc);
            context.SaveChanges();
            return exc;
        }

        public ExchangeHistory Edit(ExchangeHistory exchangehist)
        {
            var x = context.ExchangeHistory.Attach(exchangehist);
            x.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return exchangehist;

        }

        public ExchangeHistory GetHist(int Id)
        {
            return context.ExchangeHistory.OrderByDescending(x => x.ExchangeDate).Where(x => x.Currency.IsActive == true).FirstOrDefault(x => x.CurrencyId == Id);
        }


        public IEnumerable<ExchangeHistory> GetAllHistOfCurrency(DateTime startDate, DateTime endDate)
        {
            return context.ExchangeHistory.Where(x => x.ExchangeDate >= startDate && x.ExchangeDate <= endDate).Where(x => x.Currency.IsActive == true).OrderBy(o => o.ExchangeDate);
        }


        public ExchangeHistory GetHistByName(string name)
        {
            return context.ExchangeHistory.OrderByDescending(x => x.ExchangeDate).Where(x => x.Currency.IsActive == true).FirstOrDefault(x => x.Currency.Name == name);
        }
    }
}
