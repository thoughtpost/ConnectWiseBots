using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Thoughtpost.ConnectWise.Manage.Models
{
    public class Contact
    {
        [JsonProperty("managerContactId")]
        public int? ManagerContactId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("school")]
        public string School { get; set; }

        [JsonProperty("nickName")]
        public string NickName { get; set; }

        [JsonProperty("marriedFlag")]
        public bool? MarriedFlag { get; set; }

        [JsonProperty("childrenFlag")]
        public bool? ChildrenFlag { get; set; }

        [JsonProperty("significantOther")]
        public string SignificantOther { get; set; }

        [JsonProperty("portalPassword")]
        public string PortalPassword { get; set; }

        [JsonProperty("portalSecurityLevel")]
        public int? PortalSecurityLevel { get; set; }

        [JsonProperty("disablePortalLoginFlag")]
        public bool? DisablePortalLoginFlag { get; set; }

        [JsonProperty("unsubscribeFlag")]
        public bool? UnsubscribeFlag { get; set; }

        [JsonProperty("gender")]
        public GenderEnum? Gender { get; set; }

        [JsonProperty("birthDay")]
        public DateTime? BirthDay { get; set; }

        [JsonProperty("anniversary")]
        public DateTime? Anniversary { get; set; }

        [JsonProperty("assistantContactId")]
        public int? AssistantContactId { get; set; }

        /*
        [JsonProperty("customFields")]
        public IList<CustomFieldValue> CustomFields { get; set; }
        */

        [JsonProperty("securityIdentifier")]
        public string SecurityIdentifier { get; set; }

        [JsonProperty("inactiveFlag")]
        public bool? InactiveFlag { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        /*
        [JsonProperty("type")]
        public ContactTypeReference Type { get; set; }
        */

        [JsonProperty("company")]
        public CompanyReference Company { get; set; }
        /*

        [JsonProperty("site")]
        public SiteReference Site { get; set; }
        */

        [JsonProperty("presence")]
        public PresenceEnum? Presence { get; set; }

        [JsonProperty("addressLine1")]
        public string AddressLine1 { get; set; }


        [JsonProperty("addressLine2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        /*
        [JsonProperty("relationship")]
        public RelationshipReference Relationship { get; set; }

        [JsonProperty("department")]
        public ContactDepartmentReference Department { get; set; }
        */

        [JsonProperty("_info")]
        public Metadata Info { get; set; }


        public enum GenderEnum
        {
            Male = 0,
            Female = 1
        }

        public enum PresenceEnum
        {
            Online = 0,
            DoNotDisturb = 1,
            Away = 2,
            Offline = 3,
            NoAgent = 4
        }
    }
}
