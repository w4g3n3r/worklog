using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace wl.Tempo
{
    public class Client
    {
        private string username;
        private string clientSecret;
        private const string uri = "https://api.tempo.io/core/3/worklogs";

        public Client(string username, string clientSecret)
        {
            this.username = username;
            this.clientSecret = clientSecret;
        }

        public bool CreateWorkLog(wl.WorkLog wl)
        {
            if (wl.TaskId == 0) return true;

            var workLogBean = new WorkLog
            {
                IssueKey = string.Join("-", wl.Project, wl.TaskId.ToString()),
                TimeSpent = new TimeSpan(0, (int)wl.Minutes, 0),
                Start = wl.Begin,
                Description = wl.Message,
                AuthorAccountId = username
            };

            return PostWorklog(workLogBean);
        }

        private bool PostWorklog(WorkLog workLogBean)
        {
            var javascriptSerializer = new DataContractJsonSerializer(typeof(WorkLog), new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true,
                EmitTypeInformation = System.Runtime.Serialization.EmitTypeInformation.Never,
                KnownTypes = new[] { typeof(WorkLog) },
                DateTimeFormat = new System.Runtime.Serialization.DateTimeFormat("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff")
            });

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            //Start the request using the necessary url, credentials, and content
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", string.Format("Bearer {0}", this.clientSecret));

            var memoryStream = new MemoryStream();
            javascriptSerializer.WriteObject(memoryStream, workLogBean);
            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);
            string serializedRequestContent = sr.ReadToEnd();
            byte[] contentArray = System.Text.Encoding.UTF8.GetBytes(serializedRequestContent);
            request.ContentLength = contentArray.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(contentArray, 0, contentArray.Length);
            requestStream.Close();

            var response = (HttpWebResponse)request.GetResponse();

            if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created) && response.ContentType.Contains("application/json"))
            {
                var memoryStreamResponse = new MemoryStream();
                response.GetResponseStream().CopyTo(memoryStreamResponse);
                memoryStreamResponse.Position = 0;

                var result = new StreamReader(memoryStreamResponse).ReadToEnd();

                memoryStreamResponse.Position = 0;

                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
