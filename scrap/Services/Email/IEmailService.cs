namespace gemini.Services.Email;

public interface IEmailService
{
    // Task ComposeMessage(string to, string subject, string body);
    Task ComposeMessage(string to, string subject, string body, CancellationToken cancellationToken);
}