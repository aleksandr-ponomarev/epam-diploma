using CurrencyApi.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyApi.Database
{
    public class DatabaseOperator
    {
        private exchangeContext _db;
        private int _counter;
        private ILogger _logger;

        public DatabaseOperator()
        {
            var connSting = Environment.GetEnvironmentVariable("SQL_CONN_STRING");
            _db = new exchangeContext(connSting);
            _counter = 0;

            _logger = LoggerFactory.Create(o => o.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "[hh:mm:ss] ";
            }).AddConsole()).CreateLogger("DatabaseOperator");
        }

        public async Task SaveQoutes(List<Quote> quotes)
        {
            foreach (var quoteToAdd in quotes)
            {
                if(_db.Quotes.FirstOrDefault(dbQoute => dbQoute.Valuteid == quoteToAdd.Valuteid && dbQoute.Date == quoteToAdd.Date) == null)
                {
                    await _db.Quotes.AddAsync(quoteToAdd);
                    
                    _counter++;
                }
            }
            await _db.SaveChangesAsync();

            _logger.LogInformation($"Quotes saved to Db: {_counter}");
        }

        public async Task<List<Quote>> GetQuotes(DateTime startDate, DateTime endDate, string valuteId, int page, int pageSize)
        {
            var quotes = _db.Quotes.Where(quote => quote.Date >= startDate && quote.Date <= endDate);
            if (!string.IsNullOrEmpty(valuteId))
            {
                quotes = quotes.Where(c => c.Valuteid == valuteId);
            }

            _logger.LogInformation($"Quotes loaded from Db: {quotes.Count()}");

            return quotes.OrderBy(x => x.Id).Skip((page) * pageSize).Take(pageSize).ToList(); ;
        }

        public async Task ClearQoutes()
        {
            var all = from c in _db.Quotes select c;
            _db.Quotes.RemoveRange(all);
            await _db.SaveChangesAsync();

            _logger.LogInformation($"Table cleared.");
        }
    }
}
