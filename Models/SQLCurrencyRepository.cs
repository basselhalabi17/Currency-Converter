using System.Collections.Generic;
using System.Linq;
using CurrencyConverter.Repository;
using CurrencyConverter.Domain;

namespace CurrencyConverter.Models
{
    public class SQLCurrencyRepository : ICurrencyRepository
    {

        private readonly AppDbContext context;
        public SQLCurrencyRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Currency add(Currency currency)
        {
            currency.IsActive = true;
            context.Currencies.Add(currency);
            context.SaveChanges();
            return currency;
        }

        public Currency Delete(int id)
        {
            Currency cur = context.Currencies.Find(id);
            if (cur != null & cur.IsActive == true)
            {
                cur.IsActive = false;
                var x = context.Currencies.Attach(cur);
                x.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();

            }
            return cur;
        }

        public Currency GetCurrency(int Id)
        {
            return context.Currencies.Find(Id);
        }

        public IEnumerable<Currency> GetAllCurrencies()
        {
            return context.Currencies.Where(c => c.IsActive == true);
        }


        public Currency Update(Currency cur)
        {
            var x = context.Currencies.Attach(cur);
            x.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return cur;

        }

        public Currency GetByName(string name)
        {
            return context.Currencies.FirstOrDefault(c => c.Name == name);
        }


    }
}
