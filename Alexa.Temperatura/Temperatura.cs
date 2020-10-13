using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Alexa.NET;
using Alexa.NET.Request.Type;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Alexa.Temperatura
{
    public class Temperatura
    {
        private readonly HttpClient _client;

        public Temperatura(HttpClient client)
        {
            _client = client;
        }

        [FunctionName("Temperatura")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string json = await req.ReadAsStringAsync();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);

            return await ProcessRequest(skillRequest);
        }

        private async Task<IActionResult> ProcessRequest(SkillRequest skillRequest)
        {
            var requestType = skillRequest.GetRequestType();
            SkillResponse response = null;
            if (requestType == typeof(LaunchRequest))
            {
                response = ResponseBuilder.Tell("Welcome a Dotnet Marraia temperature. Create By Fernando Mendes");
                response.Response.ShouldEndSession = false;
            }
            else if (requestType == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;
                if (intentRequest.Intent.Name == "AddIntent")
                {
                    var speech = new SsmlOutputSpeech();
                    var result = await GetTemperature();
                    speech.Ssml = $"<speak>The temperature in jundiaí at this moment is { result} degrees</speak>";

                    response = ResponseBuilder.TellWithCard(speech, "The answer is", $"The answer is: {result}");
                    response.Response.ShouldEndSession = false;
                }
            }
            else if (requestType == typeof(SessionEndedRequest))
            {
                var speech = new SsmlOutputSpeech();
                speech.Ssml = $"<speak>Bye Bye Marraia!!</speak>";

                response = ResponseBuilder.TellWithCard(speech, "Bye Bye Marraia","Marraia");
                response.Response.ShouldEndSession = true;
            }
            return new OkObjectResult(response);
        }

        private async Task<int> GetTemperature()
        {
            var response = await _client.GetAsync("http://apiadvisor.climatempo.com.br/api/v1/weather/locale/3877/current?token={suachave}");
            var temperature = await response.Content.ReadAsStringAsync();
            var content = System.Text.Json.JsonSerializer.Deserialize<Clima>(temperature);

            return content.data.temperature;
        }
    }
}
