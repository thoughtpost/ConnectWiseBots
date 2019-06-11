using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Thoughtpost.ConnectWise.Manage.Models;

namespace Thoughtpost.ConnectWise.Manage
{
    public class ManageApiClient
    {
        #region Constructors
        public ManageApiClient() { }
        #endregion

        public async Task<List<Contact>> GetContactByCommunication(string comm)
        {
            var path = this.Version + "/company/contacts?page=&conditions=inactiveFlag = false&childconditions=communicationItems/value = '" +
                comm + "'";

            List<Contact> response = null;

            var result = await GetClient().GetAsync(path);
            string responseBody = result.Content.ReadAsStringAsync().Result;

            if (result.StatusCode == HttpStatusCode.OK)
            {
                response = JsonConvert.DeserializeObject<List<Contact>>(responseBody);
            }

            return response;
        }

        public async Task<Ticket> CreateTicket(string summary, string company, SeverityEnum severity)
        {
            Ticket ticket = new Ticket()
            {
                Summary = summary,
                Severity = severity,
                Company = new CompanyReference()
                {
                    Identifier = company
                }
            };

            return await CreateTicket(ticket);
        }

        public async Task<Ticket> CreateTicket(Ticket ticket)
        {
            var path = this.Version + "/service/tickets";
            Ticket response = null;

            string json = JsonConvert.SerializeObject(ticket, 
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            StringContent content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var result = await GetClient().PostAsync(path, content);
            string responseBody = result.Content.ReadAsStringAsync().Result;

            if (result.StatusCode == HttpStatusCode.Created)
            {
                response = JsonConvert.DeserializeObject<Ticket>(responseBody);
            }

            return response;
        }


        public async Task<Ticket> GetTicket(int id)
        {
            var path = this.Version + "/service/tickets/" + id.ToString();
            Ticket response = null;

            var result = await GetClient().GetAsync(path);
            string responseBody = result.Content.ReadAsStringAsync().Result;

            if (result.StatusCode == HttpStatusCode.OK)
            {
                response = JsonConvert.DeserializeObject<Ticket>(responseBody);
            }

            return response;
        }

        #region Properties
        public virtual string Version
        {
            get
            {
                return "/v4_6_release/apis/3.0";
            }
        }

        public string AppId { get; set; } 
        public string SiteUrl { get; set; } 
        public string CompanyName { get; set; } 
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string ClientId { get; set; }
        #endregion

        #region Helpers
        public string GetToken()
        {
            string text = CompanyName + "+" + PublicKey + ":" + PrivateKey;

            return Base64Encode(text);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        protected HttpClient GetClient()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(SiteUrl);
            string token = GetToken();

            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic " + token);
            //client.DefaultRequestHeaders.TryAddWithoutValidation("cookie", "cw-app-id=" + this.AppId);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("clientId", this.ClientId);

            return client;
        }

        private static readonly Encoding encoding = Encoding.UTF8;

        public HttpWebResponse PostMultipartFormData(
            string url, string userAgent,
            Dictionary<string, object> parameters)
        {
            string formDataBoundary = String.Format("----------{0:N}", 
                Guid.NewGuid());

            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(parameters, formDataBoundary);

            return PostForm(url, userAgent, contentType, formData);
        }

        private HttpWebResponse PostForm(string url, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            string token = GetToken();

            request.Headers.Add("Authorization", "Basic " + token);
            request.Headers.Add("cookie", "cw-app-id=" + this.AppId);

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> parameters, 
            string boundary)
        {
            Stream stream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in parameters)
            {
                if (needsCLRF)
                    stream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));
                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                    boundary,
                    param.Key,
                    fileToUpload.FileName ?? param.Key,
                    fileToUpload.ContentType ?? "application/octet-stream");
                    stream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));
                    stream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                    boundary,
                    param.Key,
                    param.Value);
                    stream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            string footer = "\r\n--" + boundary + "--\r\n";
            stream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            stream.Position = 0;

            byte[] formData = new byte[stream.Length];
            stream.Read(formData, 0, formData.Length);
            stream.Close();

            return formData;
        }
        #endregion


    }

    public class FileParameter
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public FileParameter(byte[] file) : this(file, null) { }
        public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
        public FileParameter(byte[] file, string filename, string contenttype)
        {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }
    }

}
