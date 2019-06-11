// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.ApplicationInsights;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Thoughtpost.Bots.Shared;
using Thoughtpost.ConnectWise.Manage;
using Thoughtpost.ConnectWise.Manage.Models;

namespace ConnectWiseSmsBot
{
    public class SmsBot : ActivityHandler
    {
        public SmsBot(StateStorage storage, IConfiguration configuration)
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
            telemetry.InstrumentationKey = _configuration.GetSection("ApplicationInsights")["InstrumentationKey"];
            telemetry.TrackTrace("OnMessageActivityAsync");

            string activityJson = JsonConvert.SerializeObject(turnContext.Activity);
            string from = "unknown";

            try
            {
                dynamic activity = JObject.Parse(activityJson);

                from = activity.from.id;
            }
            catch (Exception ex)
            {
                telemetry.TrackException(ex);
            }

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                string message = turnContext.Activity.Text;
                message = message.ToLower();

                ManageApiClient client = GetApiClient();

                UserConfig cfg =
                            await _storage.UserConfigAccessor.GetAsync(turnContext, () => new UserConfig());

                if ( string.IsNullOrEmpty( cfg.Id ) == false )
                {
                    // Already verified, turn the message into a ticket
                    Ticket newTicket = new Ticket()
                    {
                        Company = new CompanyReference() { Id = Int32.Parse(cfg.CompanyId) },
                        Contact = new ContactReference() { Id = Int32.Parse(cfg.Id) },
                        Summary = turnContext.Activity.Text,
                        Severity = SeverityEnum.High // what user created ticket ever had less? :)
                    };

                    newTicket = await client.CreateTicket(newTicket);

                    if ( newTicket != null )
                    {
                        await turnContext.SendActivityAsync(
                            MessageFactory.Text($"Thanks {cfg.Name}, a new ticket was created with ID #{newTicket.Id.ToString()}."),
                            cancellationToken);
                    }
                }
                else
                {
                    if (message.StartsWith("cwverify"))
                    {
                        string phone = StrippedPhone(from);

                        List<Contact> contacts = await client.GetContactByCommunication(phone);

                        if (contacts.Count == 1)
                        {
                            cfg.Name = contacts[0].FirstName;
                            cfg.Id = contacts[0].Id.ToString();
                            cfg.CompanyId = contacts[0].Company.Id.ToString();

                            await _storage.UserConfigAccessor.SetAsync(turnContext, cfg);

                            await _storage.State.SaveChangesAsync(turnContext);

                            await turnContext.SendActivityAsync(
                                MessageFactory.Text($"Thanks {cfg.Name}, we have verified your account using {from}. You can now send messages to this number to create tickets."),
                                cancellationToken);
                        }
                        else
                        {
                            await turnContext.SendActivityAsync(
                                MessageFactory.Text($"Sorry, we cannot identify the account associated with {from}."),
                                cancellationToken);
                        }
                    }
                    else
                    {
                        // No response
                    }
                }
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
        }

        public static string StrippedPhone(string number)
        {
            string stripped = Regex.Replace(number, "[^0-9]", "");
            if (stripped.Length == 11) stripped = stripped.Substring(1);
            return stripped;
        }
    }
}
