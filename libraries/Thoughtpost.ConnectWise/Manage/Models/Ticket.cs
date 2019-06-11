using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Thoughtpost.ConnectWise.Manage.Models
{
    public class Ticket
    {
        [JsonProperty("company")]
        public CompanyReference Company { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("severity")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SeverityEnum? Severity { get; set; }

        [JsonProperty("contact")]
        public ContactReference Contact { get; set; }

        [JsonProperty("contactEmailLookup")]
        public string ContactEmailLookup { get; set; }

        [JsonProperty("initialResolution")]
        public string InitialResolution { get; set; }

        [JsonProperty("contactEmailAddress")]
        public string ContactEmailAddress { get; set; }

        [JsonProperty("contactPhoneExtension")]
        public string ContactPhoneExtension { get; set; }

        [JsonProperty("contactPhoneNumber")]
        public string ContactPhoneNumber { get; set; }

        [JsonProperty("contactName")]
        public string ContactName { get; set; }

        [JsonProperty("status")]
        public ServiceStatusReference Status { get; set; }

        [JsonProperty("initialInternalAnalysis")]
        public string InitialInternalAnalysis { get; set; }

        [JsonProperty("_info")]
        public Metadata Info { get; set; }
    }

    public enum SeverityEnum
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

}
