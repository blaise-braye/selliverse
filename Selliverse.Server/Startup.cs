using Microsoft.AspNetCore.Mvc;
using Selliverse.Server.Actors;

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
            var system = ActorSystem.Create("selliverse");
            var throttleProps = Props.Create<SvThrottledBroadcastActor>();
            var throttleActor = system.ActorOf(throttleProps, "svThrottle");

            var props = Props.Create<SvCoreActor>(() => new SvCoreActor(throttleActor));
            var actor = system.ActorOf(props, "svCore");
            serviceCollection.AddSingleton(actor);
            
            serviceCollection.AddSingleton<SocketTranslator>();
            serviceCollection.AddMvc(options => options.EnableEndpointRouting = false);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment whe)
        {
            var websocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(60),
                // Origins go here
            };
            app.UseWebSockets(websocketOptions);
            app.UseHttpsRedirection();
            app.UseMiddleware<SocketMiddleware>();
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions());
            app.UseMvcWithDefaultRoute();
            
            app.Run(async context =>
            {
                await context.Response.WriteAsync("The Selliverse isn't real, it cannot hurt you. v+2");
            });
        }
    }
}
