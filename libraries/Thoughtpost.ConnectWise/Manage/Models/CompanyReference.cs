using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Thoughtpost.ConnectWise.Manage.Models
{
    public class CompanyReference
    {
        [JsonProperty("id")]
        public int? Id { get; set; }
        [JsonProperty("identifier")]
        public string Identifier { get; set; }
        [JsonProperty("_info")]
        public Metadata Info { get; set; }

    }
}
