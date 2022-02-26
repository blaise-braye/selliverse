using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.Threading.Tasks;

namespace Selliverse.Server
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(LogEventLevel.Information)
                .CreateLogger();

            await Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(whb =>
                {
                    whb.UseStartup<Startup>();
                })
                .UseSerilog()
                .Build()
                .RunAsync();
        }
    }
}