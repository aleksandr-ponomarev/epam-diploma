using CurrencyApi.Database;
using CurrencyApi.Loader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace CurrencyApi.Controllers
{
    [Route("/api/v1/quotes")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        [HttpPost]
        public async Task LoadQuotes()
        {
            var quotesLoader = new QuotesLoader();
            await quotesLoader.LoadAndSevetoDb();

        }

        [HttpGet]
        public async Task<IActionResult> GetQuotes([FromQuery] string startDate, string endDate, string valuteId, int page, int pageSize)
        {
            var stDate = string.IsNullOrEmpty(startDate) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) : Convert.ToDateTime(startDate);
            var enDate = string.IsNullOrEmpty(endDate) ? DateTime.Now : Convert.ToDateTime(endDate);
            var db = new DatabaseOperator();
            var qoutes = await db.GetQuotes(stDate, enDate, valuteId, page, pageSize);

            return Ok(JsonSerializer.Serialize(qoutes, new JsonSerializerOptions { WriteIndented = true }));
        }

        [HttpDelete]
        public async Task ClearQuotes([FromQuery] string date)
        {
            var db = new DatabaseOperator();
            await db.ClearQoutes();
        }
    }
}
