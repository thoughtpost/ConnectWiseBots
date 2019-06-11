using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging;

namespace Thoughtpost.Bots.Shared
{
    // This extends the normal Bot HTTP adapter to add transcript logging middleware
    // and catch-all exception handling
    public class ExtendedAdapter : BotFrameworkHttpAdapter
    {
        public ExtendedAdapter(ICredentialProvider credentialProvider, 
            ILogger<BotFrameworkHttpAdapter> logger, 
            TranscriptLoggerMiddleware transcriptMiddleware)
            : base(credentialProvider)
        {
            if (credentialProvider == null)
            {
                throw new NullReferenceException(nameof(credentialProvider));
            }

            if (logger == null)
            {
                throw new NullReferenceException(nameof(logger));
            }

            if (transcriptMiddleware != null)
            {
                Use(transcriptMiddleware);
            }

            // Add translation middleware to the adapter's middleware pipeline

            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError($"Exception caught : {exception.Message}");

                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync("Sorry, it looks like something went wrong.");
            };
        }

        //public async System.Threading.Tasks.Task ProcessAsync(
        //    Microsoft.AspNetCore.Http.HttpRequest httpRequest,
        //    Microsoft.AspNetCore.Http.HttpResponse httpResponse, 
        //    Microsoft.Bot.Builder.IBot bot, 
        //    System.Threading.CancellationToken cancellationToken
        //)
        //{
        //    await base.ProcessAsync(httpRequest, 
        //        httpResponse, 
        //        bot, 
        //        cancellationToken);
        //}
    }
}