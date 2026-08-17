using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace gemini.Services.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    private readonly string? _fromUsername;
    private readonly string? _fromPassword;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _logger = logger;
        _configuration = configuration;
        _fromUsername = _configuration["Email:Username"];
        _fromPassword = _configuration["Email:Password"];
    }

    public async Task ComposeMessage(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        if (_fromUsername is null || _fromPassword is null)
        {
            _logger.LogCritical("Missing configurational username or password");
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("MG", _fromUsername));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = @body
        };

        await ConnectAndSend(message, cancellationToken);
    }

    private async Task ConnectAndSend(MimeMessage message, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();

        // Only for debugging purposes
        // client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

        try
        {
            await client.ConnectAsync(
                "smtp.gmail.com",
                587,
                MailKit.Security.SecureSocketOptions.StartTls,
                cancellationToken
            );
            await client.AuthenticateAsync(_fromUsername!, _fromPassword!, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (System.Exception ex)
        {
            _logger.LogCritical(ex, "Wrong username or password!");
            throw;
        }

    }
}