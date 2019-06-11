// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.3.0

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Bot.Builder.Azure;

using Thoughtpost.Bots.Shared;

namespace ConnectWiseSmsBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IStorage storage = new MemoryStorage();
            AzureBlobStorage azureStorage = new AzureBlobStorage(
                Configuration.GetSection("azureStorageConnection")?.Value,
                Configuration.GetSection("azureStorageContainer")?.Value);

            // Enable the conversation transcript middleware.
            AzureBlobTranscriptStore blobStore = new AzureBlobTranscriptStore(
                Configuration.GetSection("azureStorageConnection")?.Value,
                Configuration.GetSection("azureStorageContainer")?.Value);

            var transcriptMiddleware = new TranscriptLoggerMiddleware(blobStore);

            services.AddSingleton<StateStorage>(sp =>
            {
                // Create the custom state accessor.
                return new StateStorage(azureStorage);
            });

            services.AddSingleton<TranscriptLoggerMiddleware>(tl =>
            {
                return transcriptMiddleware;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Create the Bot Framework Adapter.
            services.AddSingleton<IBotFrameworkHttpAdapter, ExtendedAdapter>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, SmsBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
