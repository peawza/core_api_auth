using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json.Serialization;

namespace Authentication.ApiClients.ApiClientsModels
{
    public class SmsSendClientsModels
    {
        public class SmsSend_Criteria
        {

            [Required]
            public string To { get; set; }
            [Required]
            public string Text { get; set; }


        }
        public class SmsSendHtml_Criteria
        {
            [JsonIgnore]
            public string Username { get; set; }
            [JsonIgnore]
            public string Passwd { get; set; }
            [JsonIgnore]
            public string From { get; set; }
            [Required]
            public string To { get; set; }
            [Required]
            public string Text { get; set; }
            [JsonIgnore]
            public string Datacoding { get; set; }
            [JsonIgnore]
            public DateTime? Datetime { get; set; }
        }

        public class SmsSendHtml_Result
        {
            public bool IsSuccessStatusCode { get; set; }
            public HttpStatusCode HttpStatusCode { get; set; }

            public bool IsOk { get; set; }
            public string RawContent { get; set; } = "";
        }
    }
}
