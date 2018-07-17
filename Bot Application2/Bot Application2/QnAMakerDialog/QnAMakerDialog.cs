using Newtonsoft.Json;
using System;
using System.Net;

namespace QnAMakerDialog
{
    internal class QnAMakerDialog
    {
        public QnAMakerDialog()
        {
        }

        internal QnAResult GetQnAResponse(string userAnswer, string subscriptionkey, string knowledgeBaseId)
        {
            var rString = String.Empty;

            /**
             * Build the uri
             **/
            var build = new UriBuilder($"https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/" + knowledgeBaseId + "/generateAnswer/");

            /**
             *  Add query to the header as the part of body
             **/

            var body = $"{{\"question\": \"{userAnswer}\"}}";

            /**
             * Sending the post request to API endpoint
             **/

            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF-8 encoding part 
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key sent to the method to the header details

                client.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionkey);
                client.Headers.Add("Content-Type", "application/json");

                rString = client.UploadString(build.Uri, body);
            }

            /**
             * De-serialize the response recieved from the qnamaker api
             **/

            QnAResult res;

            try
            {
                var t = JsonConvert.DeserializeObject(rString);
                res = JsonConvert.DeserializeObject<QnAResult>(rString);
                return res;
            }
            catch (Exception)
            {
                throw new Exception("Unable to convert the JSON result");
            }
        }
    }
    public class QnAResult
    {
        public Answer[] ans { get; set; }

        public class Answer
        {
            [JsonProperty(PropertyName = "answer")]
            public string ans { get; set; }

            [JsonProperty(PropertyName = "score")]
            public double score { get; set; }
        }
    }
}