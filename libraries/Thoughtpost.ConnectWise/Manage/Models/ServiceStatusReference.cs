using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Thoughtpost.ConnectWise.Manage.Models
{
    public class ServiceStatusReference
    {
        [JsonProperty("id")]
        public int? Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("_info")]
        public Metadata Info { get; set; }

    }
}
