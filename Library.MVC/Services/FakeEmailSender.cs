using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Library.MVC.Services
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Stub
            return Task.CompletedTask;
        }
    }
}
