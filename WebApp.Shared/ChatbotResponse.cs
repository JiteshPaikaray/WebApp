namespace WebApp.Shared;

public class ChatbotResponse
{
    public string? BotMessage { get; set; }
    public string? ConversationId { get; set; }
    public float Confidence { get; set; }
    public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
    public bool IsSuccess { get; set; } = true;
    public string? ErrorMessage { get; set; }
}
