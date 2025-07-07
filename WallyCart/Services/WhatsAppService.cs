using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class WhatsAppService
{
    private readonly HttpClient _httpClient;
    private readonly string _accesToken;
    private readonly string _phoneNumberId;

    public WhatsAppService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _accesToken = config["WhatsApp:AccessToken"]!;
        _phoneNumberId = config["WhatsApp:PhoneNumberId"]!;
    }

    public async Task<bool> SendTemplateMessageAsync(string toPhoneNUmber)
    {
        var url = $"https://graph.facebook.com/v19.0/{_phoneNumberId}/messages";

        var payload = new
        {
            messaging_product = "whatsapp",
            to = toPhoneNUmber,
            type = "template",
            template = new
            {
                name = "hello_world",
                language = new { code = "en_US" }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accesToken);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SendWhatsAppMessageAsync(string toPhone, string message)
    {
        var url = $"https://graph.facebook.com/v19.0/{_phoneNumberId}/messages";

        var payload = new
        {
            messaging_product = "whatsapp",
            to = toPhone,
            type = "text",
            text = new
            {
                body = message
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accesToken);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}