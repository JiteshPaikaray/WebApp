using Microsoft.AspNetCore.Mvc;
using WebApp.Services.ML;
using WebApp.Shared;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
    private readonly IChatbotMLService _chatbotMLService;
    private readonly ILogger<ChatbotController> _logger;

    public ChatbotController(IChatbotMLService chatbotMLService, ILogger<ChatbotController> logger)
    {
        _chatbotMLService = chatbotMLService;
        _logger = logger;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ChatbotResponse>> Chat([FromBody] ChatbotRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ChatbotResponse
            {
                IsSuccess = false,
                ErrorMessage = "Invalid request"
            });
        }

        try
        {
            var response = await _chatbotMLService.ProcessUserMessageAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in chat endpoint");
            return StatusCode(500, new ChatbotResponse
            {
                IsSuccess = false,
                ErrorMessage = "An unexpected error occurred"
            });
        }
    }
}
