namespace WebApp.Shared;

public class ChatbotRequest
{
    public string UserMessage { get; set; } = string.Empty;
    public string? ConversationId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
