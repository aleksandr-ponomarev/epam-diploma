using CurrencyApi.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CurrencyApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var db = new DatabaseOperator();
            await db.InitDB();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging(logging =>
                    {
                        logging.AddSimpleConsole(options =>
                        {
                            //options.IncludeScopes = true;
                            options.SingleLine = true;
                            options.TimestampFormat = "[hh:mm:ss] ";
                        });
                        logging.ClearProviders();
                        logging.AddConsole();
                    });
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:5000");
                });
    }
}
