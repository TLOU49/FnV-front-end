using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using api.Interfaces;

namespace api.Service
{
  public class EmailService : IEmailService
  {
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
      _configuration = configuration;
      _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
      var smtpHost = _configuration["Smtp:Host"];
      var smtpPortString = _configuration["Smtp:Port"];
      var smtpUsername = _configuration["Smtp:Username"];
      var smtpPassword = _configuration["Smtp:Password"];
      var smtpFrom = _configuration["Smtp:From"];

      if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPortString) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpFrom))
      {
        throw new ArgumentNullException("SMTp configuration is missing or incomplete.");
      }

      if (!int.TryParse(smtpPortString, out int smtpPort))
      {
        throw new FormatException("SMTP port configuration is not a valid integer.");
      }

      var smtpClient = new SmtpClient(smtpHost)
      {
        // Port = int.Parse(smtpHost),
        Port = smtpPort,
        Credentials = new NetworkCredential(smtpUsername, smtpPassword),
        EnableSsl = true
      };

      // Custom server certificate validation
      ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
      {
        // Allow all certificates for development, not recommended for production
        if (sslPolicyErrors == SslPolicyErrors.None)
          return true;

        if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
        {
          foreach (var status in chain.ChainStatus)
          {
            if (status.Status != X509ChainStatusFlags.NoError && status.Status != X509ChainStatusFlags.UntrustedRoot)
            {
              return false;
            }
          }
        }
        return true;
      };

      var mailMessage = new MailMessage
      {
        From = new MailAddress(smtpFrom),
        Subject = subject,
        Body = message,
        IsBodyHtml = true
      };
      mailMessage.To.Add(email);

      try
      {
        await smtpClient.SendMailAsync(mailMessage);
        _logger.LogInformation("Email sent successfully to {Email}", email);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Could not send email to {Email}", email);
        // Log the exception
        throw new InvalidOperationException("Could not send email", ex);
      }
      // await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
    {
      var smtpHost = _configuration["Smtp:Host"];
      var smtpPortString = _configuration["Smtp:Port"];
      var smtpUsername = _configuration["Smtp:Username"];
      var smtpPassword = _configuration["Smtp:Password"];
      var smtpFrom = _configuration["Smtp:From"];

      var message = new MailMessage();
      message.To.Add(email);
      message.From = new MailAddress(smtpFrom);
      message.Subject = "Email Confirmation";
      message.Body = $"<br/>Hi, <br/> Thanks for signing up! Your Fruit & Veg account is almost complete. Click the button below to confirm your email address. <br/><br/><a style='color: #ffffff;background-color:#2187AB;padding: 10px 20px;text-decoration: none;border-radius: 5px;font-family:Arial,sans-serif;' href='{confirmationLink}'>Confirm email address</a><br></br> If you didn't create this account, please report this on imageapp@singular.co.za. <br/><br/>";
      message.IsBodyHtml = true;

      using (var smtpClient = new SmtpClient(smtpHost))
      {
        smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
        smtpClient.Port = 587;
        await smtpClient.SendMailAsync(message);
      }
    }

    public Task SendEmailConfirmationAsync(string email, string subject, string message)
    {
      throw new NotImplementedException();
    }
  }
}