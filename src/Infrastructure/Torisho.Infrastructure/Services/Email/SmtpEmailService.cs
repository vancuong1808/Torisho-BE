using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Torisho.Application.Interfaces.Email;

namespace Torisho.Infrastructure.Services.Email;

public class SmtpEmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _senderName;
    private readonly string _senderEmail;

    public SmtpEmailService(IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        _smtpHost = configuration["EmailSettings:SmtpHost"]
            ?? throw new InvalidOperationException("EmailSettings:SmtpHost not configured");
        _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
        _smtpUsername = configuration["EmailSettings:SmtpUsername"]
            ?? throw new InvalidOperationException("EmailSettings:SmtpUsername not configured");
        _smtpPassword = configuration["EmailSettings:SmtpPassword"]
            ?? throw new InvalidOperationException("EmailSettings:SmtpPassword not configured");
        _senderName = configuration["EmailSettings:SenderName"] ?? "Torisho Support";
        _senderEmail = configuration["EmailSettings:SenderEmail"] ?? _smtpUsername;
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string resetLink, CancellationToken ct = default)
    {
        var subject = "Reset your Torisho password";
        var body = BuildPasswordResetBody(resetLink);

        return SendEmailAsync(toEmail, subject, body, ct);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentException("Recipient email is required", nameof(toEmail));

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_senderName, _senderEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html) { Text = body };

        using var client = new SmtpClient();

        try
        {
            ct.ThrowIfCancellationRequested();
            await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(_smtpUsername, _smtpPassword, ct);
            await client.SendAsync(message, ct);
        }
        finally
        {
            await client.DisconnectAsync(true, ct);
        }
    }

    private static string BuildPasswordResetBody(string resetLink)
    {
        return $@"<!doctype html>
<html>
    <head>
        <meta charset=""utf-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
        <title>Reset your password</title>
    </head>
    <body style=""margin:0;padding:0;background-color:#f4f6f8;"">
        <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""background-color:#f4f6f8;"">
            <tr>
                <td align=""center"" style=""padding:24px 12px;"">
                    <table role=""presentation"" width=""600"" cellspacing=""0"" cellpadding=""0"" style=""width:100%;max-width:600px;background:#ffffff;border-radius:12px;overflow:hidden;border:1px solid #e5e7eb;"">
                        <tr>
                            <td style=""padding:20px 24px;background:#111827;color:#ffffff;font-family:Arial,Helvetica,sans-serif;font-size:18px;font-weight:700;letter-spacing:0.2px;"">
                                Torisho
                            </td>
                        </tr>
                        <tr>
                            <td style=""padding:24px;font-family:Arial,Helvetica,sans-serif;color:#111827;font-size:14px;line-height:1.6;"">
                                <p style=""margin:0 0 12px 0;"">We received a request to reset your password.</p>
                                <p style=""margin:0 0 20px 0;"">Use the button below to set a new password. This link expires in 15 minutes.</p>
                                <p style=""margin:0 0 20px 0;"">
                                    <a href=""{resetLink}"" style=""display:inline-block;padding:12px 18px;background:#0b5fff;color:#ffffff;text-decoration:none;border-radius:6px;font-weight:600;"">Reset password</a>
                                </p>
                                <p style=""margin:0 0 8px 0;font-size:12px;color:#6b7280;"">If the button does not work, copy and paste this link into your browser:</p>
                                <p style=""margin:0 0 16px 0;font-size:12px;color:#2563eb;word-break:break-all;"">{resetLink}</p>
                                <p style=""margin:0;font-size:12px;color:#6b7280;"">If you did not request this, you can ignore this email.</p>
                            </td>
                        </tr>
                        <tr>
                            <td style=""padding:16px 24px;background:#f9fafb;font-family:Arial,Helvetica,sans-serif;font-size:11px;color:#6b7280;"">
                                Torisho Support · This is an automated email. Please do not reply.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </body>
</html>";
    }
}
