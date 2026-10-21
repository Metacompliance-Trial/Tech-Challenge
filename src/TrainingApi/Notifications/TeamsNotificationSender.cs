namespace TrainingApi.Notifications;

public class TeamsNotificationSender
{
    private const string WebhookUrl = "https://teams-webhooks.internal/training/3f9c2b1e-7d4a-4c1b-9e2f-6a8b0c1d2e3f";

    public async Task PostMessageAsync(string teamsUserId, string title, string message)
    {
        // Simulated post while the Teams webhook is unavailable in local environments.
        await Task.Delay(400);

        Console.WriteLine($"[Teams] {WebhookUrl} -> {teamsUserId}: {title} - {message}");
    }
}
