using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
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
        public void ConfigureServices(IServiceCollection services)
        {
            var system = ActorSystem.Create("selliverse");
            var throttleProps = Props.Create<SvThrottledBroadcastActor>();
            var throttleActor = system.ActorOf(throttleProps, "svThrottle");

            var props = Props.Create<SvCoreActor>(() => new SvCoreActor(throttleActor));
            var actor = system.ActorOf(props, "svCore");
            services.AddSingleton(actor);
            
            services.AddSingleton<SocketTranslator>();

            services.Configure<GzipCompressionProviderOptions>
                (options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.AddMvc(options => options.EnableEndpointRouting = false);
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
            app.UseResponseCompression();
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = OnPrepareResponse });
            app.UseMvcWithDefaultRoute();
            
            app.Run(async context =>
            {
                await context.Response.WriteAsync("The Selliverse isn't real, it cannot hurt you. v+2");
            });
        }

        private void OnPrepareResponse(StaticFileResponseContext context)
        {
            var file = context.File;
            var response = context.Context.Response;

            if (file.Name.EndsWith(".gz"))
            {
                response.Headers[HeaderNames.ContentEncoding] = "gzip";
            }
        }
    }
}
