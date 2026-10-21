namespace TrainingApi.Notifications;

public class SmsNotificationSender
{
    private const string ApiUrl = "https://api.smsgateway.example.com/v2/messages";
    private const string ApiKey = "a8f5f167f44f4964e6c998dee827110c";
    private const string SenderId = "TRAINING";

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException("Phone number is required", nameof(phoneNumber));
        }

        // Simulated send while the SMS gateway is unavailable in local environments.
        await Task.Delay(500);

        Console.WriteLine($"[SMS] {ApiUrl} ({SenderId}, key {ApiKey[..4]}...) -> {phoneNumber}: {message}");
        return true;
    }
}
