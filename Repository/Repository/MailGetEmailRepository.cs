using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string toName, string subject, string body)
    {
        var apiKey = _configuration["Mailjet:ApiKey"];
        var apiSecret = _configuration["Mailjet:ApiSecret"];
        var email = _configuration["Mailjet:SenderEmail"];
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret) || string.IsNullOrEmpty(email))
        {
            Console.WriteLine("❌ Missing API credentials in configuration.");
            return;
        }
        MailjetClient client = new MailjetClient(apiKey, apiSecret);
        var request = new MailjetRequest { Resource = SendV31.Resource }
            .Property(Send.Messages, new JArray {
                new JObject {
                    {"From", new JObject {
                        {"Email", email}, 
                        {"Name", "Nutri track"}
                    }},
                    {"To", new JArray {
                        new JObject {
                            {"Email", toEmail},
                            {"Name", toName}
                        }
                    }},
                    {"Subject", subject},
                    {"TextPart", body},
                    {"HTMLPart", $"<h3>{body}</h3>"}
                }
            });

        MailjetResponse response = await client.PostAsync(request);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("✅ Email sent successfully!");
        }
        else
        {
            Console.WriteLine($"❌ Failed to send email. Status: {response.StatusCode}");
            Console.WriteLine(response.GetErrorMessage());
        }
    }
}
