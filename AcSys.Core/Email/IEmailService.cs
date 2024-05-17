using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AcSys.Core.Email
{
    public interface IEmailService
    {
        bool Sent { get; set; }

        void Send(MailMessage message, bool shouldThrowError = false);
        Task SendAsync(MailMessage message, bool shouldThrowError = false);

        void Send(string to, string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null);
        Task SendAsync(string to, string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null);

        void Send(string from, string fromDisplayName, string to, string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null);
        Task SendAsync(string from, string fromDisplayName, string to, string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null);

        void Send(string from, string fromDisplayName, string to, string cc, string bcc, string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null);
        Task SendAsync(string from, string fromDisplayName, string to, string cc, string bcc, string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null);
    }
}
