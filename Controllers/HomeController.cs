using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using CurrencyConverter.Domain;
using CurrencyConverter.Repository;


namespace CurrencyConverter.Controllers
{
    public class currencydata
    {
        public string Name { get; set; }
        public string Sign { get; set; }
        public float LatestRate { get; set; }
        public DateTime LastExchangeDate { get; set; }
    }
    public class message {
        public string msg { get; set; }
    }

    [ApiController]
   [Route("[controller]")]

    public class HomeController: ControllerBase
    {
        private readonly  ICurrencyRepository _currencyrep;
        private readonly IExchangeHistoryRepository _exchangerep;

        public HomeController(ICurrencyRepository curr, IExchangeHistoryRepository exc)
        {
            _currencyrep = curr;
            _exchangerep = exc;
        }
        
        [HttpPost]
        [Route("Create")]
        public IActionResult Create(string name, string sign, float rate, DateTime ExchangeDat)
        {
            
            if (name != null && sign != null && rate != 0)
            {
                Currency newCurrency = new Currency
                {
                    Name = name,
                    Sign = sign,
                    IsActive = true,

                };
                _currencyrep.add(newCurrency);
                DateTime date;
                if (ExchangeDat == default)
                {
                    date = DateTime.Now;
                }
                else date = ExchangeDat;
                ExchangeHistory exchist = new ExchangeHistory
                {
                    CurrencyId = newCurrency.Id,
                    Rate = rate,
                    ExchangeDate = date,
                    //Id = model.Id + 1
                };
                currencydata entry = new currencydata
                {
                    Name = name,
                    Sign = sign,
                    LatestRate = rate,
                    LastExchangeDate = date
                };
                _exchangerep.add(exchist);
                return new OkObjectResult(entry);
            }
            
            else
            {
                return StatusCode(404, new message { msg = "Some fields are required" });

            }
        }
        [HttpGet]
        [Route("GetByName")]
        public IActionResult GetByName(string name)
        {
            Currency c = _currencyrep.GetByName(name);
            if (c==null )
            {
                return StatusCode(404, new message { msg = "Not Found" });
                
            }
            else if (!c.IsActive)
            {
                return StatusCode(404, new message { msg = "currency not active" });
               
            }
            else
            {
                ExchangeHistory e = _exchangerep.GetHist(c.Id);
                currencydata entry = new currencydata
                {
                    Name = name,
                    Sign = c.Sign,
                    LatestRate = e.Rate,
                    LastExchangeDate = e.ExchangeDate
                };
                return new OkObjectResult(entry);
            }
        }

        [HttpGet]
        [Route("GetAllCurrencies")]

        public IActionResult GetAllCurrencies()
        {
            var AllCurrencies = _currencyrep.GetAllCurrencies();
            if (AllCurrencies == null)
            {
                return StatusCode(404, new message { msg = "Not Found" });
            }
            else
            {
                var currinfo = new List<currencydata>();
                foreach (Currency c in AllCurrencies)
                {
                    Console.WriteLine(c);
                    ExchangeHistory e = _exchangerep.GetHist(c.Id);
                    currencydata entry = new currencydata
                    {
                        Name = c.Name,
                        Sign = c.Sign,
                        LatestRate = e.Rate,
                        LastExchangeDate = e.ExchangeDate
                    };
                    currinfo.Add(entry);
                }
                return new OkObjectResult(currinfo);
            }
        }

        [HttpPut]
        [Route("Update")]

        public IActionResult Update(int id, string name, string sign, float rate, DateTime ExchangeDat)
        {
            if (id != 0)
            {
                Currency c = _currencyrep.GetCurrency(id);
                if (c == null) return StatusCode(404, new message { msg = "Currency Not Found" });
                else if (c.IsActive)
                {
                    string oldname = c.Name;
                    string oldsign = c.Sign;
                    if (name != null && sign != null)
                    {
                        c.Name = name;
                        c.Sign = sign;
                        _currencyrep.Update(c);
                    }
                    else if (name != null && sign == null)
                    {
                        c.Name = name;
                        c.Sign = oldsign;
                        _currencyrep.Update(c);
                    }
                    else if (name == null && sign != null)
                    {
                        c.Name = oldname;
                        c.Sign = sign;
                        _currencyrep.Update(c);
                    }
                    ExchangeHistory e = new ExchangeHistory
                    {
                        CurrencyId = id,
                        Rate = rate
                    };
                    ExchangeHistory temp = _exchangerep.GetHist(id);
                    if (rate != 0) //rate changed
                    {
                        if (ExchangeDat != default(DateTime)) //if date updated, then new entry in database
                        {
                            e.ExchangeDate = ExchangeDat;
                            _exchangerep.add(e);
                        }
                        else //if date not set, then the date is set to the current date
                        {
                            e.ExchangeDate = DateTime.Now;
                            _exchangerep.add(e);
                        }
                    }
                    else
                    {
                        e.Rate = temp.Rate;
                        e.ExchangeDate = temp.ExchangeDate;
                    }

                    currencydata entry = new currencydata
                    {
                        Name = c.Name,
                        Sign = c.Sign,
                        LatestRate = e.Rate,
                        LastExchangeDate = e.ExchangeDate
                    };
                    return new OkObjectResult(entry);
                }
                else
                {
                    return StatusCode(404, new message { msg = "currency already deleted" });
                }
            }
            else
            {
                return StatusCode(404, new message { msg = "Id is null" });
            }
        }
        [HttpDelete]
        [Route("Delete")]

        public IActionResult Delete(string name)
        {
            Currency c = _currencyrep.GetByName(name);
            if (c == null)
            {
                return StatusCode(404, new message { msg = "Not Found" });

            }
            else if (!c.IsActive)
            {
                return StatusCode(404, new message { msg = "currency already deleted" });

            }
            else
            {
                c.IsActive = false;
                _currencyrep.Update(c);
                return StatusCode(200, new message { msg = "currency deleted successfully" });
            }
        }


        [HttpGet]
        [Route("GetHigheshtNCurrencies")]

        public IActionResult GetHigheshtNCurrencies(int N)
        {
           
            var AllCurrencies = _currencyrep.GetAllCurrencies();
            if (AllCurrencies == null)
            {
                return StatusCode(404, new message { msg = "Not Found" });
            }
            else
            {
                var lasthistory = new List<ExchangeHistory>();
                foreach (Currency c in AllCurrencies)
                {

                    ExchangeHistory e = _exchangerep.GetHist(c.Id);
                    lasthistory.Add(e);
                }
                    lasthistory = lasthistory.OrderByDescending(x => x.Rate).Take(N).ToList();
                    return new OkObjectResult(lasthistory);
            }
        }


        [HttpGet]
        [Route("GetLowestNCurrencies")]

        public IActionResult GetLowestNCurrencies(int N)
        {

            var AllCurrencies = _currencyrep.GetAllCurrencies();
            if (AllCurrencies == null)
            {
                return StatusCode(404, new message { msg = "Not Found" });
            }
            else
            {
                //var currinfo = new List<currencydata>();
                var lasthistory = new List<ExchangeHistory>();
                foreach (Currency c in AllCurrencies)
                {

                    ExchangeHistory e = _exchangerep.GetHist(c.Id);
                    lasthistory.Add(e);
                }
                int count = lasthistory.Count();
                if (N >= count)
                {
                    lasthistory = lasthistory.OrderBy(x => x.Rate).ToList();
                    return new OkObjectResult(lasthistory);
                }
                else
                {
                    lasthistory = lasthistory.OrderBy(x => x.Rate).Take(N).ToList();
                    return new OkObjectResult(lasthistory);
                }
                //return new OkObjectResult(lasthistory);
            }
        }

        [HttpGet]
        [Route("GetMostImprovedNCurrenciesByDate")]

        public IActionResult GetMostImprovedNCurrenciesByDate(int N,DateTime startDate, DateTime endDate)
        {
            var AllCurrencies = _currencyrep.GetAllCurrencies();
            if (AllCurrencies == null)
            {
                return StatusCode(404, new message { msg = "Not Found" });
            }
            else
            {
                Dictionary<int, float> firstinstance = new Dictionary<int, float>();
                Dictionary<int, float> lastinstance = new Dictionary<int, float>();
                Dictionary<string, float> matrix = new Dictionary<string, float>();
                //var currinfo = new List<>();

                IEnumerable<ExchangeHistory> e = _exchangerep.GetAllHistOfCurrency(startDate, endDate);
                foreach(ExchangeHistory exc in e)
                {
                    if (!firstinstance.ContainsKey(exc.CurrencyId))
                        firstinstance.Add(exc.CurrencyId, exc.Rate);
                    else
                    {
                        lastinstance[exc.CurrencyId] = exc.Rate;
                    }
                }
                foreach (var entry in firstinstance)
                {
                    if (lastinstance.ContainsKey(entry.Key))
                    {
                        Currency curr = _currencyrep.GetCurrency(entry.Key);
                        matrix[curr.Name] = lastinstance[entry.Key] - firstinstance[entry.Key];
                    }
                }
                var fes = matrix.OrderByDescending(x => x.Value).Where(x=>x.Value>0).Take(N);
                string json = JsonConvert.SerializeObject(fes.ToList());
                return new OkObjectResult(json);
            }
        }

        [HttpGet]
        [Route("GetLeastImprovedNCurrenciesByDate")]

        public IActionResult GetLeastImprovedNCurrenciesByDate(int N, DateTime startDate, DateTime endDate)
        {
            var AllCurrencies = _currencyrep.GetAllCurrencies();
            if (AllCurrencies == null)
            {
                return StatusCode(404, new message { msg = "Not Found" });
            }
            else
            {
                Dictionary<int, float> firstinstance = new Dictionary<int, float>();
                Dictionary<int, float> lastinstance = new Dictionary<int, float>();
                Dictionary<string, float> matrix = new Dictionary<string, float>();
                //var currinfo = new List<>();

                IEnumerable<ExchangeHistory> e = _exchangerep.GetAllHistOfCurrency(startDate, endDate);
                foreach (ExchangeHistory exc in e)
                {
                    if (!firstinstance.ContainsKey(exc.CurrencyId))
                        firstinstance.Add(exc.CurrencyId, exc.Rate);
                    else
                    {
                        lastinstance[exc.CurrencyId] = exc.Rate;
                    }
                }
                foreach (var entry in firstinstance)
                {
                    if (lastinstance.ContainsKey(entry.Key))
                    {
                        Currency curr = _currencyrep.GetCurrency(entry.Key);
                        matrix[curr.Name] = lastinstance[entry.Key] - firstinstance[entry.Key];
                    }
                }
                var fes = matrix.OrderBy(x => x.Value).Take(N);
                string json = JsonConvert.SerializeObject(fes.ToList());
                return new OkObjectResult(json);
            }
        }

        [HttpGet]
        [Route("ConvertAmount")]

        public IActionResult ConvertAmount(float amount, string fromCurrency, string ToCurrency)
        {
            if (fromCurrency.ToLower() == "dollar")
            {
                var x = _exchangerep.GetHistByName(ToCurrency);
                float convert = amount / x.Rate;
                return new OkObjectResult(convert);
            }
            else if (ToCurrency.ToLower() == "dollar")
            {
                var x = _exchangerep.GetHistByName(fromCurrency);
                float convert = amount * x.Rate;
                return new OkObjectResult(convert);
            }
            else
            {
                var fromhist = _exchangerep.GetHistByName(fromCurrency);
                var tohist = _exchangerep.GetHistByName(ToCurrency);
                if (fromhist == null || tohist == null)
                {
                    return StatusCode(404, new message { msg = "A currency is deleted" });
                }
                else
                {
                    float convert = (fromhist.Rate / tohist.Rate) * amount;
                    return new OkObjectResult(convert);
                }
            }
        }
    }
}
