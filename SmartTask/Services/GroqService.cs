using Newtonsoft.Json;
using System.Text;

namespace SmartTask.Services
{
    public class GroqService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GroqService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["Groq:ApiKey"]!;
        }

        public async Task<string> GetAiResponseAsync(string prompt)
        {
            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = @"Du er en opgaveassistent i SmartTask-applikationen. 
                                    Din eneste opgave er at hjælpe brugeren med deres opgaveliste.

                                    Du må KUN:
                                    - Hjælpe med at prioritere opgaver
                                    - Nedbryde opgaver i delopgaver
                                    - Foreslå rækkefølge for opgaver baseret på deadline og prioritet
                                    - Kommentere på opgavernes status og fremgang

                                    Du må IKKE:
                                    - Besvare spørgsmål der ikke handler om brugerens opgaver
                                    - Skrive kode, essays, digte eller andet indhold
                                    - Diskutere nyheder, vejr, politik eller andre emner
                                    - Agere som en generel AI-assistent

                                    Hvis brugeren stiller et irrelevant spørgsmål, svar kort:
                                    'Jeg kan kun hjælpe med dine opgaver i SmartTask.'

                                    Svar altid på dansk. Vær kortfattet og præcis."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                max_tokens = 500
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync(
                "https://api.groq.com/openai/v1/chat/completions",
                content
            );

            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"Fejl fra Groq API: {response.StatusCode} – {responseJson}";
            }

            dynamic result = JsonConvert.DeserializeObject(responseJson)!;
            return result.choices[0].message.content.ToString();
        }
    }
}