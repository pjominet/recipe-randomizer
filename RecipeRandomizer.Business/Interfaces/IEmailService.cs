using System.Threading.Tasks;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string html, string sender = null);
    }
}
