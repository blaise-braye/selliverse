namespace Selliverse.Server
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment whe)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("The Selliverse isn't real, it cannot hurt you.");
            });
        }
    }
}
