using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using WebApp.Shared;

namespace WebAPP.Services.Chatbot;

public interface IChatbotClient
{
    Task<ChatbotResponse> SendMessageAsync(ChatbotRequest request);
}

public class ChatbotClient : IChatbotClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatbotClient> _logger;

    public ChatbotClient(HttpClient httpClient, ILogger<ChatbotClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ChatbotResponse> SendMessageAsync(ChatbotRequest request)
    {
        try
        {
            _logger.LogInformation("Sending chat request to backend: {Message}", request.UserMessage);

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/chatbot/chat", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Backend returned error: {StatusCode}", response.StatusCode);
                return new ChatbotResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Failed to get response from server"
                };
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ChatbotResponse>(responseContent);
            return result ?? new ChatbotResponse
            {
                IsSuccess = false,
                ErrorMessage = "Invalid response format"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error communicating with chatbot service");
            return new ChatbotResponse
            {
                IsSuccess = false,
                ErrorMessage = "Communication error with server"
            };
        }
    }
}



