using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/whatsapp")]
public class WhatsAppController : ControllerBase
{
    private readonly WhatsAppService _whatsAppService;

    public WhatsAppController(WhatsAppService whatsAppService)
    {
        _whatsAppService = whatsAppService;
    }

    [HttpPost]
    public async Task<IActionResult> SendTestMessage([FromBody] string to)
    {
        var success = await _whatsAppService.SendTemplateMessageAsync(to);
        return success ? Ok("Message sent.") : StatusCode(500, "Failed to send message.");
    }
}

