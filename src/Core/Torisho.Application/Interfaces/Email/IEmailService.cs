namespace Torisho.Application.Interfaces.Email;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken ct = default);
    Task SendPasswordResetEmailAsync(string toEmail, string resetLink, CancellationToken ct = default);
}
