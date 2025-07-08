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
        await ProcessWebhookAsync(body);
        return Ok("EVENT_RECEIVED");
    }

    private async Task ProcessWebhookAsync(JsonElement body)
    {
        try
        {
            _logger.LogDebug("Webhook body: {Body}", body);

            if (!body.TryGetProperty("entry", out var entries)) return;

            foreach (var entry in entries.EnumerateArray())
            {
                if (!entry.TryGetProperty("changes", out var changes)) continue;

                foreach (var change in changes.EnumerateArray())
                {
                    if (!change.TryGetProperty("value", out var value)) continue;
                    if (!value.TryGetProperty("messages", out var messages)) continue;

                    foreach (var msg in messages.EnumerateArray())
                    {
                        if (!msg.TryGetProperty("from", out var fromProp) ||
                            !msg.TryGetProperty("text", out var textObj) ||
                            !textObj.TryGetProperty("body", out var bodyProp))
                        {
                            _logger.LogWarning("Unsupported or incomplete message format.");
                            continue;
                        }

                        var from = fromProp.GetString();
                        var text = bodyProp.GetString();

                        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(text))
                            continue;

                        var user = await _userService.GetUserByPhoneAsync(from);
                        if (user is null)
                        {
                            await _whatsAppService.SendWhatsAppMessageAsync(
                                from, "⚠️ Sorry, this number is not registered with us.");
                            continue;
                        }

                        var session = _sessionManager.GetOrCreate(user.Id);
                        var response = await _commandRouter.HandleCommandAsync(session, text)
                                    ?? "⚠️ No response from handler.";

                        await _whatsAppService.SendWhatsAppMessageAsync(from, response);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing webhook");
        }
    }

}
