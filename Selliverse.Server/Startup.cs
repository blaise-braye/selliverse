namespace Selliverse.Server
{
    using Akka.Actor;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<SocketTranslator>();

            var system = ActorSystem.Create("selliverse");


            serviceCollection.AddSingleton<ActorSystem>(system);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment whe)
        {
            var websocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(60),
                // Origins go here
            };
            app.UseWebSockets(websocketOptions);

            app.UseMiddleware<SocketMiddleware>();

            app.Run(async context =>
            {
                await context.Response.WriteAsync("The Selliverse isn't real, it cannot hurt you.");
            });
        }
    }
}
