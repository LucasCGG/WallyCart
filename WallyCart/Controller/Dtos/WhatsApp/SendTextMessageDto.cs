namespace WallyCart.Dtos.WhatsApp;

public class SendTextMessageDto
{
    public string To { get; set; } = default!;
    public string Message { get; set; } = default!; 
}