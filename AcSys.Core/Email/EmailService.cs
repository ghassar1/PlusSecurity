using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using AcSys.Core.Extensions;

namespace AcSys.Core.Email
{
    public class EmailService : IEmailService, IDisposable
    {
        private const string smtp_config_live = "smtp_live";
        private const string smtp_config_dev = "smtp_dev";
        private const string smtp_config_test = "smtp_test";

        private static string smtp_config_name = smtp_config_live;
        public static string SmtpConfigName
        {
            get { return smtp_config_name; }
            set { smtp_config_name = value; }
        }

        private static SmtpSection _smtpConfig = null;
        private SmtpClient _smtpClient = null;

        private static string fromEmailConfigName = "EMAIL_FROM";
        public static string FromEmailConfigName
        {
            get { return fromEmailConfigName; }
            set { fromEmailConfigName = value; }
        }

        private static string fromDisplayNameConfigName = "EMAIL_FROMDISPLAYNAME";
        public static string FromDisplayNameConfigName
        {
            get { return fromDisplayNameConfigName; }
            set { fromDisplayNameConfigName = value; }
        }

        private string fromEmail;
        public string FromEmail
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.fromEmail))
                {
                    if (string.IsNullOrWhiteSpace(FromEmailConfigName))
                        throw new ArgumentNullException("FromDisplayNameConfigName", "Name of the AppSetting configuration property should be defined which contains the 'From Display Name'.");

                    this.fromEmail = ConfigurationManager.AppSettings[FromEmailConfigName];
                }
                return fromEmail;
            }
            set
            {
                fromEmail = value;
            }
        }

        private string fromDisplayName;
        public string FromDisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.fromDisplayName))
                {
                    if (string.IsNullOrWhiteSpace(FromDisplayNameConfigName))
                        throw new ArgumentNullException("FromDisplayNameConfigName", "Name of the AppSetting configuration property should be defined which contains the 'From Display Name'.");

                    this.fromDisplayName = ConfigurationManager.AppSettings[FromDisplayNameConfigName];
                }
                return fromDisplayName;
            }
            set
            {
                fromDisplayName = value;
            }
        }

        public bool Sent { get; set; }

        public EmailService()
        {
            this.Sent = false;

            string appMode = ConfigurationManager.AppSettings["MODE"];
            if (appMode.IsNotNullOrWhiteSpace())
            {
                SmtpConfigName = appMode.ToUpper().Trim() == "LIVE" ? smtp_config_live : smtp_config_dev;
            }
            //LoadEmailConfigurations();
        }

        public void Send(string to, string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null)
        {
            this.Send(this.FromEmail, this.FromDisplayName, to, string.Empty, string.Empty, subject, body, shouldThrowError, attachments);
        }

        public async Task SendAsync(string to, string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null)
        {
            await this.SendAsync(this.FromEmail, this.FromDisplayName, to, string.Empty, string.Empty, subject, body, shouldThrowError, attachments);
        }

        public void Send(string from, string fromDisplayName, string to, string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null)
        {
            this.Send(from, fromDisplayName, to, string.Empty, string.Empty, subject, body, shouldThrowError, attachments);
        }

        public async Task SendAsync(string from, string fromDisplayName, string to, string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null)
        {
            await this.SendAsync(from, fromDisplayName, to, string.Empty, string.Empty, subject, body, shouldThrowError, attachments);
        }

        //public void Send(string from, string fromDisplayName, string to, string cc, string bcc, string subject, string body, bool shouldThrowError = false, string[] attachments = null)
        public void Send(string from, string fromDisplayName, string to, string cc, string bcc, string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null)
        {
            MailMessage message = PrepareMessage(from, fromDisplayName, to, cc, bcc, subject, body, shouldThrowError, attachments);
            this.Send(message);
        }

        public async Task SendAsync(string from, string fromDisplayName, string to, string cc, string bcc, string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null)
        {
            MailMessage message = PrepareMessage(from, fromDisplayName, to, cc, bcc, subject, body, shouldThrowError, attachments);
            await this.SendAsync(message);
        }

        public void Send(MailMessage message, bool shouldThrowError = false)
        {
            if (this._smtpClient == null)
            {
                this.PrepareSmtpClient();
            }

            try
            {
                this._smtpClient.Send(message);
                this.Sent = true;
            }
            catch (Exception ex)
            {
                this.Sent = false;

                //TODO: Log the error while sending email

                if (shouldThrowError)
                    throw ex;
            }
        }

        public async Task SendAsync(MailMessage message, bool shouldThrowError = false)
        {
            //using (this._smtpClient = new SmtpClient(this._smtpConfig.Network.Host, this._smtpConfig.Network.Port))
            //{
            //    this._smtpClient.Credentials = new NetworkCredential(this._smtpConfig.Network.UserName, this._smtpConfig.Network.Password);

            //    //smtpClient.SendAsync(message, null);
            //    await this._smtpClient.SendMailAsync(message);
            //}

            if (this._smtpClient == null)
            {
                this.PrepareSmtpClient();
            }

            try
            {
                await this._smtpClient.SendMailAsync(message);
                this.Sent = true;
            }
            catch (Exception ex)
            {
                this.Sent = false;

                //TODO: Log the error while sending email

                if (shouldThrowError)
                    throw ex;
            }
        }

        private static void LoadEmailConfigurations()
        {
            if (_smtpConfig == null)
            {
                _smtpConfig = (SmtpSection)ConfigurationManager.GetSection("mailSettings/" + SmtpConfigName);
                if (_smtpConfig == null)
                {
                    throw new ApplicationException("SMTP mail settings have not been specified in configurations file.");
                }
                else
                {
                    if (_smtpConfig.SpecifiedPickupDirectory != null && _smtpConfig.SpecifiedPickupDirectory.PickupDirectoryLocation != null)
                    {
                        if (!Directory.Exists(_smtpConfig.SpecifiedPickupDirectory.PickupDirectoryLocation))
                        {
                            Directory.CreateDirectory(_smtpConfig.SpecifiedPickupDirectory.PickupDirectoryLocation);
                        }
                    }
                    else if (_smtpConfig.Network == null)
                    {
                        throw new ApplicationException("SMTP network settings have not been specified in configurations file.");
                    }
                }
            }
        }

        private void PrepareSmtpClient()
        {
            this._smtpClient = new SmtpClient();

            if (_smtpConfig == null)
            {
                LoadEmailConfigurations();
            }

            if (_smtpConfig != null)
            {
                this._smtpClient.DeliveryMethod = _smtpConfig.DeliveryMethod;
                if (_smtpConfig.SpecifiedPickupDirectory != null && _smtpConfig.SpecifiedPickupDirectory.PickupDirectoryLocation != null)
                {
                    this._smtpClient.PickupDirectoryLocation = _smtpConfig.SpecifiedPickupDirectory.PickupDirectoryLocation;
                }
                else if (_smtpConfig.Network != null)
                {
                    this._smtpClient.Host = _smtpConfig.Network.Host ?? null;
                    this._smtpClient.Port = _smtpConfig.Network.Port;
                    this._smtpClient.UseDefaultCredentials = _smtpConfig.Network.DefaultCredentials;

                    this._smtpClient.Credentials = new NetworkCredential(_smtpConfig.Network.UserName, _smtpConfig.Network.Password, _smtpConfig.Network.ClientDomain);
                    this._smtpClient.EnableSsl = _smtpConfig.Network.EnableSsl;

                    if (_smtpConfig.Network.TargetName != null)
                        this._smtpClient.TargetName = _smtpConfig.Network.TargetName;
                }
                else
                {
                    throw new ApplicationException("SMTP configurations have not been specified.");
                }
            }
        }

        private MailMessage PrepareMessage(string from, string fromDisplayName, string to, string cc, string bcc,
            string subject, string body, bool shouldThrowError = false, Dictionary<string, byte[]> attachments = null)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                to = "mymg55@yahoo.com";
                throw new ArgumentNullException("'To' address must be provided.");
            }

            if (string.IsNullOrWhiteSpace(from))
            {
                from = "mymg55@yahoo.com";
                throw new ArgumentNullException("'From' address must be provided.");
            }

            if (this._smtpClient == null)
            {
                this.PrepareSmtpClient();
            }

            //this._smtpClient = new SmtpClient("mail.ghassar.com", 25)
            //{
            //    Credentials = new NetworkCredential("mymg55@yahoo.com", "X6j|2kn!"),
            //    UseDefaultCredentials = true,
            //    EnableSsl = false,
            //};

            MailMessage message = new MailMessage()
            {
                //mail.From = new MailAddress(message.From);
                //From = new MailAddress("mymg55@yahoo.com"),
                From = new MailAddress(from, fromDisplayName),

                Subject = subject,
                IsBodyHtml = true,
                Body = body
            };

            message.To.Add(to);

            if (attachments != null && attachments.Count > 0)
            {
                foreach (var attachment in attachments)
                {
                    if (attachment.Value != null && attachment.Value.Length > 0)
                    {
                        string name = string.IsNullOrWhiteSpace(attachment.Key) ? "attachment" : attachment.Key;
                        var at = new Attachment(new MemoryStream(attachment.Value), name);
                        message.Attachments.Add(at);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(cc))
                message.CC.Add(cc);

            if (!string.IsNullOrWhiteSpace(bcc))
                message.Bcc.Add(bcc);

#if DEBUG
            //message.To.Clear();
            message.To.Add("mymg55@yahoo.com");
            message.To.Add("mymg55@yahoo.com");
            message.CC.Clear();
            message.Bcc.Clear();
#endif
            return message;
        }

        public void Dispose()
        {
            if (this._smtpClient != null)
                this._smtpClient.Dispose();
        }
    }
}
