// Copyright (c) Thoughtpost, Inc. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.ApplicationInsights;

using Newtonsoft.Json;

using Thoughtpost.Bots.Shared;
using Thoughtpost.ConnectWise;
using Thoughtpost.ConnectWise.Manage;

using System.Linq;

namespace ConnectWiseSlackBot
{
    public class SlackBot : ActivityHandler
    {
        public SlackBot(StateStorage storage, IConfiguration configuration)
        {
            this._storage = storage;
            this._configuration = configuration;
        }

        protected StateStorage _storage { get; set; }
        protected IConfiguration _configuration { get; set; }

        protected ManageApiClient _client { get; set; }

        protected ManageApiClient GetApiClient()
        {
            ManageApiClient client = new ManageApiClient();

            var cwSection = _configuration.GetSection("ConnectWise");

            client.AppId = cwSection["AppId"];
            client.CompanyName = cwSection["CompanyName"];
            client.SiteUrl = cwSection["SiteUrl"];
            client.PublicKey = cwSection["PublicKey"];
            client.PrivateKey = cwSection["PrivateKey"];
            client.ClientId = cwSection["ClientId"];

            return client;
        }


        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            TelemetryClient telemetry = new TelemetryClient();

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                string activityJson = JsonConvert.SerializeObject(turnContext.Activity);
                string input = turnContext.Activity.Text;

                telemetry.TrackTrace("Activity - " + activityJson);

                ManageApiClient client = GetApiClient();

                var parts = Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .ToList();

                string command = parts[0].ToLower();

                switch (command)
                {
                    case "createticket":
                        {
                            var ticket = await client.CreateTicket(parts[3],
                                parts[1],
                                (Thoughtpost.ConnectWise.Manage.Models.SeverityEnum)System.Enum.Parse(typeof(Thoughtpost.ConnectWise.Manage.Models.SeverityEnum), parts[2]));

                            string url = _configuration.GetSection("ConnectWise")["TicketUrl"];
                            url = url.Replace("###", ticket.Id.ToString());

                            var reply = MessageFactory.ContentUrl(
                                url,
                                @"text/html",
                                @"Ticket #" + ticket.Id.ToString() + " - " + ticket.Summary,
                                @"Ticket #" + ticket.Id.ToString() + " was created successfully.");

                            await turnContext.SendActivityAsync(reply, cancellationToken);
                        }
                        break;

                    case "getticket":
                        {
                            var ticket = await client.GetTicket(System.Int32.Parse(parts[1]));
                            string url = _configuration.GetSection("ConnectWise")["TicketUrl"];
                            url = url.Replace("###", ticket.Id.ToString());

                            string lastUpdate = ticket.Info["lastUpdated"];
                            System.DateTime dtUpdate = DateTime.Parse(lastUpdate);

                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine($"**Ticket #{ticket.Id} - {ticket.Summary}**");
                            sb.AppendLine("");
                            sb.AppendLine($"**Status: ** {ticket.Status.Name}");
                            sb.AppendLine("");
                            sb.AppendLine($"**Severity: ** {ticket.Severity.Value.ToString()}");
                            sb.AppendLine("");
                            sb.AppendLine($"**Last Updated: ** {dtUpdate.ToString()}");
                            sb.AppendLine("");
                            sb.AppendLine($"**Contact: ** {ticket.ContactName} ({ticket.ContactEmailAddress})");
                            sb.AppendLine("");
                            sb.AppendLine($"Click here for the complete ticket: [Link]({url})");

                            var reply = MessageFactory.Text(sb.ToString());

                            await turnContext.SendActivityAsync(reply, cancellationToken);
                        }
                        break;


                    default:
                        {
                        }
                        break;
                }
            

            }
            else
            {
                await turnContext.SendActivityAsync(
                    MessageFactory.Text($"Sorry, I didn't understand that."),
                    cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        MessageFactory.Text($"ConnectWise Slack Bot, ready to go! :)"), 
                        cancellationToken);
                }
            }
        }

    }
}
