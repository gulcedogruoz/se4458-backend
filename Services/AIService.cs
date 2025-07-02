using HotelBooking.DTO;
using HotelBooking.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HotelBooking.Services
{
    public class AIService : IAIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ClaudeSettings _claudeSettings;

        public AIService(IHttpClientFactory httpClientFactory, IOptions<ClaudeSettings> claudeOptions)
        {
            _httpClientFactory = httpClientFactory;
            _claudeSettings = claudeOptions.Value;
        }

        public async Task<AIResponseDto> GetAnswerAsync(AIRequestDto request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_claudeSettings.ApiUrl); // e.g. "https://api.anthropic.com/v1/"
            client.DefaultRequestHeaders.Add("x-api-key", _claudeSettings.ApiKey);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

            var systemPrompt = @"
You are an AI hotel booking assistant.

Your task is to extract the user's hotel search intent and return it as a structured JSON object in the following format:

{
  ""intent"": ""SearchHotels"",
  ""parameters"": {
    ""city"": ""string"",
    ""startDate"": ""ISO 8601 date string"",
    ""endDate"": ""ISO 8601 date string"",
    ""numberOfPeople"": integer
  }
}

Only include the parameters the user mentions. Do NOT invent or assume data. Return ONLY a valid JSON object, no explanations.

If the user’s message is not about hotel search, return:
{
  ""intent"": ""None"",
  ""parameters"": {}
}

You MUST respond with only the JSON object.
";

            var requestBody = new
            {
                model = "claude-3-opus-20240229",
                system = systemPrompt,
                max_tokens = 512,
                temperature = 0.2,
                messages = new[]
                {
                    new {
                        role = "user",
                        content = request.Question
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("messages", content); // "messages" endpoint relative path

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Claude API error: {response.StatusCode} - {errorBody}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Claude raw response:");
            Console.WriteLine(responseBody);

            using var doc = JsonDocument.Parse(responseBody);
            var contentArray = doc.RootElement.GetProperty("content");

            string rawAnswer = string.Empty;

            foreach (var item in contentArray.EnumerateArray())
            {
                var type = item.GetProperty("type").GetString();
                if (type == "text")
                {
                    rawAnswer = item.GetProperty("text").GetString();
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(rawAnswer))
            {
                throw new Exception("Claude response did not contain any text content.");
            }

            rawAnswer = rawAnswer.Trim();

            try
            {
                using var jsonDoc = JsonDocument.Parse(rawAnswer);
                var prettyJson = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });
                return new AIResponseDto { Answer = prettyJson };
            }
            catch (JsonException)
            {
                // If response is not a valid JSON, return raw text
                return new AIResponseDto { Answer = rawAnswer };
            }
        }
    }
}
