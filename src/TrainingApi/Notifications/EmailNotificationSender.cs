namespace TrainingApi.Notifications;

public class EmailNotificationSender
{
    private const string SmtpHost = "smtp.mailrelay.internal";
    private const int SmtpPort = 587;
    private const string SmtpUsername = "training-notifications";
    private const string SmtpPassword = "Tr41n1ng!Smtp2024";
    private const string FromAddress = "no-reply@training.internal";

    public async Task SendAsync(string toAddress, string subject, string body)
    {
        // Simulated send while the mail relay is unavailable in local environments.
        await Task.Delay(300);

        Console.WriteLine($"[Email] {SmtpUsername}@{SmtpHost}:{SmtpPort} {FromAddress} -> {toAddress}: {subject} - {body}");
    }
}
