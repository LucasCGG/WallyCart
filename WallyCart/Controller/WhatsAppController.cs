using Microsoft.AspNetCore.Mvc;
using WallyCart.Dtos.WhatsApp;

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
    public async Task<IActionResult> SendTestMessage([FromBody] SendTemplateMessageDto dto)
    {
        var success = await _whatsAppService.SendTemplateMessageAsync(dto.To);
        return success ? Ok("Message sent.") : StatusCode(500, "Failed to send message.");
    }

    [HttpPost("send-text")]
    public async Task<IActionResult> SendTextMessage([FromBody] SendTextMessageDto dto)
    {
        var success = await _whatsAppService.SendWhatsAppMessageAsync(dto.To, dto.Message);
        return success ? Ok("Text message sent.") : StatusCode(500, "Failed to send text message.");
    }
}

