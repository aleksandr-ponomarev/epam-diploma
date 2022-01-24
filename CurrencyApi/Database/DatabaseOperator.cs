using CurrencyApi.DBModels;
using Npgsql;
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
        private readonly string _sqlConnString;
        private readonly ILogger _logger;
        private int _counter;

        public DatabaseOperator()
        {
            _sqlConnString = Environment.GetEnvironmentVariable("SQL_CONN_STRING");
            _db = new exchangeContext(_sqlConnString);
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

        public async Task InitDB()
        {
            using var psgConn = new NpgsqlConnection(_sqlConnString);
            await psgConn.OpenAsync();

            var createTableCmd = new NpgsqlCommand(
               $"CREATE TABLE IF NOT EXISTS quotes(" +
                "id        BIGSERIAL   PRIMARY KEY, " +
                "Date      TIMESTAMP   NOT NULL, " +
                "Name      TEXT        NOT NULL, " +
                "ValuteId  TEXT        NOT NULL, " +
                "NumCode   INTEGER     NOT NULL, " +
                "CharCode  TEXT        NOT NULL, " +
                "Nominal   INTEGER     NOT NULL, " +
                "Value     NUMERIC     NOT NULL)"
               , psgConn);
            var createNewIndexCmd = new NpgsqlCommand($"CREATE INDEX IF NOT EXISTS quotes_index ON quotes using btree (Date, Name, ValuteId)", psgConn);
            var createNewConstraintCmd = new NpgsqlCommand($"ALTER TABLE quotes ADD CONSTRAINT quotes_const UNIQUE (Date, ValuteId)", psgConn);

            try
            {
                await createTableCmd.ExecuteNonQueryAsync();
                await createNewIndexCmd.ExecuteNonQueryAsync();
                await createNewConstraintCmd.ExecuteNonQueryAsync();

                _logger.LogInformation("New table created.");
            }
            catch
            {
                _logger.LogInformation("Table existed. Skipping.");
            }

            await psgConn.CloseAsync();
        }
    }
}
