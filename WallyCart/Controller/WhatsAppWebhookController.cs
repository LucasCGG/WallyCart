using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WallyCart.Session;

[ApiController]
[Route("api/webhook/whatsapp")]
public class WhatsAppWebhookController : ControllerBase
{
    private readonly ILogger<WhatsAppWebhookController> _logger;
    private readonly WhatsAppService _whatsAppService;
    private readonly SessionManager _sessionManager;
    private readonly CommandRouter _commandRouter;
    private readonly UserService _userService;
    private readonly string _verifyToken;

    public WhatsAppWebhookController(
    ILogger<WhatsAppWebhookController> logger,
    WhatsAppService whatsAppService,
    SessionManager sessionManager,
    CommandRouter commandRouter,
    UserService userService,
    IConfiguration config)
    {
        _logger = logger;
        _whatsAppService = whatsAppService;
        _sessionManager = sessionManager;
        _commandRouter = commandRouter;
        _userService = userService;

        _verifyToken = config["Whatsapp:VerifyToken"]!;
    }


    [HttpGet]
    public IActionResult VerifyWebhook(
        [FromQuery(Name = "hub.mode")] string mode,
        [FromQuery(Name = "hub.verify_token")] string token,
        [FromQuery(Name = "hub.challenge")] string challenge)
    {
        if (mode == "subscribe" && token == _verifyToken)
        {
            _logger.LogInformation("Webhook verified successfully.");
            return Ok(challenge);
        }

        _logger.LogWarning("Webhook verification failed.");
        return Forbid();
    }


    [HttpPost]
    public async Task<IActionResult> ReceiveMessage([FromBody] JsonElement body)
    {
        try
        {
            var entry = body.GetProperty("entry")[0];
            var changes = entry.GetProperty("changes")[0];
            var value = changes.GetProperty("value");

            var messages = value.GetProperty("messages");
            if (messages.ValueKind == JsonValueKind.Array && messages.GetArrayLength() > 0)
            {
                var msg = messages[0];
                var text = msg.GetProperty("text").GetProperty("body").GetString();
                var from = msg.GetProperty("from").GetString();

                Console.WriteLine($"💬 Received from {from}: {text}");

                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(text))
                {
                    var user = await _userService.GetUserByPhoneAsync(from);
                    if (user == null)
                    {
                        return StatusCode(400, "No user found with this number");
                    }

                    var session = _sessionManager.GetOrCreate(user.Id);
                    var responseMessage = await _commandRouter.HandleCommandAsync(session, text);

                    await _whatsAppService.SendWhatsAppMessageAsync(from, responseMessage ?? "⚠️ No response from handler.");

                }
                else
                {
                    _logger.LogWarning("Received message with null 'from' or 'text'");
                }

                return Ok();
            }

            return BadRequest("No message found in payload.");
        }
        catch (Exception ex)
        {
            _logger.LogError("❌ Error parsing WhatsApp message: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
