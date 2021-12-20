using CurrencyApi.Database;
using CurrencyApi.DBModels;
using CurrencyApi.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CurrencyApi.Loader
{
    public class QuotesLoader
    {
        private List<Quote> _quotesList;
        private string _exchangeApiAddress;
        private DatabaseOperator _dbOperator;

        public QuotesLoader()
        {
            _quotesList = new List<Quote>();
            _exchangeApiAddress = Environment.GetEnvironmentVariable("EXCHANGE_API_ADDRESS");
            _dbOperator = new DatabaseOperator();

        }

        public async Task LoadAndSevetoDb()
        {
            await FormMonthQoutes();
            await _dbOperator.SaveQoutes(_quotesList);

        }

        private async Task FormMonthQoutes()
        {
            for (var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); date <= DateTime.Now; date = date.AddDays(1))
            {
                await GetDailyQuotes(date);
            }
        }

        private async Task GetDailyQuotes(DateTime date)
        {
            var m_strFilePath = $"{_exchangeApiAddress}?date_req={date:dd'/'M'/'yyyy}";

            string xmlStr;
            using (var wc = new WebClient())
            {
                xmlStr = await wc.DownloadStringTaskAsync(m_strFilePath);
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStr);
            XmlSerializer formatter = new(typeof(ValCurs));
            using XmlReader reader = new XmlNodeReader(xmlDoc);
            var response = (ValCurs)formatter.Deserialize(reader);

            foreach (var xmlQuote in response.Valute)
            {
                var quote = new Quote()
                {
                    Valuteid = xmlQuote.ID,
                    Numcode = xmlQuote.NumCode,
                    Charcode = xmlQuote.CharCode,
                    Nominal = xmlQuote.Nominal,
                    Name = xmlQuote.Name,
                    Value = decimal.Parse(xmlQuote.Value),
                    Date = date
                };

                _quotesList.Add(quote);
            }
        }
    }
}
