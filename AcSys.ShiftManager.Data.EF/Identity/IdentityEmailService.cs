using System.Threading.Tasks;
using AcSys.Core.Email;
using Microsoft.AspNet.Identity;

namespace AcSys.ShiftManager.Data.EF.Identity
{
    public class IdentityEmailService : IIdentityMessageService
    {
        protected IEmailService _emailService = null;

        public IdentityEmailService(IEmailService emailService)
        {
            this._emailService = emailService;
        }

        public async Task SendAsync(IdentityMessage message)
        {
            await this._emailService.SendAsync(message.Destination, message.Subject, message.Body);
        }
    }
}
