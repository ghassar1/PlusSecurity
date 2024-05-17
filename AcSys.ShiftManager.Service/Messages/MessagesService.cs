using AcSys.Core.Data.Model.Base;
using AcSys.Core.Data.Querying;
using AcSys.Core.Email;
using AcSys.Core.Extensions;
using AcSys.Core.ObjectMapping;
using AcSys.ShiftManager.Data.EF.Identity;
using AcSys.ShiftManager.Data.Messages;
using AcSys.ShiftManager.Data.UnitOfWork;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Model.Helpers;
using AcSys.ShiftManager.Service.Base;
using AcSys.ShiftManager.Service.Results;
using AcSys.ShiftManager.Service.Users;
using Autofac.Extras.NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace AcSys.ShiftManager.Service.Messages
{
    public class MessagesService : ApplicationServiceBase, IMessagesService
    {

        #region Public Constructors

        public MessagesService(
            IUnitOfWork unitOfWork,
            ApplicationRoleManager roleManager,
            ApplicationUserManager userManager,
            IMessageRepository repo,
            IMessageViewRepository messageViewRepo,
            ILogger logger,
            IEmailService emailService
            ) : base(unitOfWork, roleManager, userManager, logger, emailService)

        {
            //_db = db;
            MessageRepo = repo;
            MessageViewRepo = messageViewRepo;

            // wrong
            //Logger = LogManager.GetCurrentClassLogger();

            //Logger = logger;

            //Logger.Trace("Sample trace message");
            //Logger.Debug("Sample debug message");
            //Logger.Info("Sample informational message");
            //Logger.Warn("Sample warning message");
            //Logger.Error("Sample error message");
            //Logger.Fatal("Sample fatal error message");

            //// alternatively you can call the Log() method and pass log level as the parameter.
            //Logger.Log(LogLevel.Info, "Sample informational message");

            //throw new ApplicationException("Me");
        }

        #endregion Public Constructors

        #region Private Properties

        IMessageRepository MessageRepo { get; set; }
        IMessageViewRepository MessageViewRepo { get; set; }

        #endregion Private Properties

        #region Public Methods

        public async Task<Guid> CreateMessage(MessageDto dto)
        {
            //if (!ModelState.IsValid) return BadRequest(ModelState);

            Message message = ObjectMapper.Map<MessageDto, Message>(dto);
            message.SentAt = DateTime.Now;

            message.Sender = LoggedInUser;

            List<User> recipients = new List<User>();
            if (dto.Recipients != null && dto.Recipients.Count > 0)
                foreach (UserBasicDetailsDto recipientDto in dto.Recipients)
                {
                    User recipient = await UserManager.FindByNameAsync(recipientDto.Username);

                    if (recipient == null) BadRequest("Invalid user name specified.");
                    recipients.Add(recipient);
                }
            message.Recipients.Clear();
            foreach (User recipient in recipients)
                message.Recipients.Add(recipient);

            MessageRepo.Add(message);

            await UnitOfWork.SaveChangesAsync();

            LoggedInUser.AddLog(Enums.SubjectType.Message, Enums.ActivityType.Sent, "Subject: {0}".FormatWith(message.Subject), message);
            await UnitOfWork.SaveChangesAsync();

            //return CreatedAtRoute("DefaultApi", new { id = message.Id }, message);
            return message.Id;
        }

        public async Task DeleteMessage(Guid id)
        {
            Message message = await MessageRepo.FindAsync(id);
            if (message == null) NotFound();

            //MessageRepo.Delete(message);
            message.Delete();

            LoggedInUser.AddLog(Enums.SubjectType.Message, Enums.ActivityType.Deleted, "Subject: {0}".FormatWith(message.Subject), message);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<MessageDto> GetMessage(Guid id)
        {
            Message message = await MessageRepo.FindAsync(id);
            if (message == null) NotFound();

            // check if logged in user can view the message
            CanUserViewMessage(LoggedInUser, message);

            MessageDto dto = ObjectMapper.Map<Message, MessageDto>(message);

            LoggedInUser.AddLog(Enums.ActivityType.Viewed, message);

            await UnitOfWork.SaveChangesAsync();

            return dto;
        }

        public async Task<List<MessageDto>> GetMessages()
        {
            var messages = await MessageRepo.GetAllAsync();
            //List<MessageDto> dtos = ObjectMapper.Map<List<Message>, List<MessageDto>>(messages);

            List<MessageDto> dtos = new List<MessageDto>();
            foreach (Message message in messages)
            {
                MessageDto dto = ObjectMapper.Map<Message, MessageDto>(message);

                MessageView view = message.Views.FirstOrDefault(o => o.User.Id == LoggedInUser.Id);
                if (view == null)
                {
                    dto.IsViewed = false;
                    dto.ViewedAt = null;
                }
                else
                {
                    dto.IsViewed = true;
                    dto.ViewedAt = view.ViewedAt;
                }
            }
            return dtos;
        }

        public async Task<IListResult<Message, MessageDto>> GetMyMessages(FindMyInBoxMessagesQuery query)
        {
            //var searchResults = await MessageRepo.FindAsync(o => o.Recipients.Any(r => r.Id == LoggedInUser.Id), o => o.SentAt, true);

            //ISearchResults<Message> searchResults = await MessageRepo.Find(query.ToSpec(LoggedInUser.Id));
            query.UserId = LoggedInUser.Id;
            ISearchResult<Message> searchResults = await MessageRepo.Find(query);

            IListResult<Message, MessageDto> results = new ListResult<Message, MessageDto>(query, searchResults,
                (message, dto) =>
                {
                    MessageView view = message.Views.FirstOrDefault(o => o.User.Id == LoggedInUser.Id);
                    if (view == null)
                    {
                        dto.IsViewed = false;
                        dto.ViewedAt = null;
                    }
                    else
                    {
                        dto.IsViewed = true;
                        dto.ViewedAt = view.ViewedAt;
                    }
                });

            LoggedInUser.AddLog(Enums.SubjectType.Message,
                query.SearchCriteria.IsNullOrWhiteSpace() ? Enums.ActivityType.Viewed : Enums.ActivityType.Searched,
                query.SearchCriteria.IsNullOrWhiteSpace() ? string.Empty : "Searched for: {0}".FormatWith(query.SearchCriteria));

            await UnitOfWork.SaveChangesAsync();

            return results;
        }

        public async Task<List<MessageDto>> GetMyNewMessages()
        {
            var messages = await MessageRepo.FindAsync(o => ((o.Recipients.Any(r => r.Id == LoggedInUser.Id))
                && !o.Views.Any(u => u.User.Id == LoggedInUser.Id)));

            List<MessageDto> dtos = ObjectMapper.Map<List<Message>, List<MessageDto>>(messages);
            return dtos;
        }

        public async Task<List<UserBasicDetailsDto>> GetRecipients()
        {
            var users = await UserManager.Users.ToListAsync();
            var userDtos = ObjectMapper.Map<List<User>, List<UserBasicDetailsDto>>(users);
            return userDtos;
        }

        public async Task<IListResult<Message, MessageDto>> GetSentMessages(FindMySentMessagesQuery query)
        {
            //var searchResults= await MessageRepo.FindAsync(o => o.Sender.Id == LoggedInUser.Id);
            //ISearchResults<Message> searchResults = await MessageRepo.Find(query.ToSpec(LoggedInUser.Id));

            query.UserId = LoggedInUser.Id;
            ISearchResult<Message> searchResults = await MessageRepo.Find(query);

            IListResult<Message, MessageDto> results = new ListResult<Message, MessageDto>(query, searchResults);
            
            //List<MessageDto> dtos = ObjectMapper.Map<List<Message>, List<MessageDto>>(searchResults.Records);
            //List<MessageDto> dtos = new List<MessageDto>();
            //foreach (Message message in messages)
            //{
            //    MessageDto dto = ObjectMapper.Map<Message, MessageDto>(message);
            //    foreach(MessageView view in message.Views)
            //    {
            //        UserBasicDto viewer = ObjectMapper.Map<User, UserBasicDto>(view.User);
            //        dto.Viewers.Add(viewer);
            //    }
            //    results.Data.Add(dto);
            //}
            return results;
        }

        public async Task MarkAsRead(Guid id)
        {
            Message message = await MessageRepo.FindAsync(id);
            if (message == null) NotFound();

            CanUserViewMessage(LoggedInUser, message);

            var viewed = await MessageViewRepo.AnyAsync(o => o.Message.Id == id && o.User.Id == LoggedInUser.Id);
            if (viewed) return;

            MessageView view = new MessageView()
            {
                Message = message,
                User = LoggedInUser,
                ViewedAt = DateTime.Now,
            };
            MessageViewRepo.Add(view);

            LoggedInUser.AddLog(Enums.ActivityType.Read, message);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsUnread(Guid id)
        {
            Message message = await MessageRepo.FindAsync(id);
            if (message == null) NotFound();

            CanUserViewMessage(LoggedInUser, message);

            CanUserMarkAsUnread(LoggedInUser, message);

            List<MessageView> views = message.Views.Where(o => o.User.Id == LoggedInUser.Id).ToList();
            if (views != null && views.Count > 0)
            {
                MessageViewRepo.Delete(views);

                LoggedInUser.AddLog(Enums.ActivityType.MarkedAsUnread, message);

                await UnitOfWork.SaveChangesAsync();
            }
        }

        void CanUserMarkAsUnread(User user, Message message)
        {
            if (message.Recipients != null && message.Recipients.Count > 0
                            && message.Sender != null && message.Sender.Id != user.Id
                            && !message.Recipients.Any(r => r.Id == user.Id))
                BadRequest("Cannot mark notification as unread. The notification does not belong to you.");
        }

        public async Task UpdateMessage(Guid id, MessageDto dto)
        {
            //if (id != dto.Id) return BadRequest();

            Message message = await MessageRepo.FindAsync(id);
            if (message == null) NotFound();

            LoggedInUserShouldBe(message.Sender);

            ObjectMapper.Map<MessageDto, Message>(dto, message);

            List<User> recipients = new List<User>();
            if (dto.Recipients != null && dto.Recipients.Count > 0)
                foreach (UserBasicDetailsDto userDto in dto.Recipients)
                {
                    User recipient = await UserManager.FindByNameAsync(userDto.Username);

                    if (recipient == null) BadRequest("Invalid user name specified.");
                    recipients.Add(recipient);
                }
            message.Recipients.Clear();
            foreach (User recipient in recipients)
                message.Recipients.Add(recipient);

            MessageRepo.Update(message);

            LoggedInUser.AddLog(Enums.ActivityType.Updated, message);

            try
            {
                await UnitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await MessageExists(id)))
                    NotFound();
                else
                    throw;
            }
        }

        #endregion Public Methods

        #region Private Methods

        void CanUserViewMessage(User user, Message message)
        {
            if (message.Sender.Id != user.Id
                && message.Recipients != null
                && message.Recipients.Count > 0
                && !message.Recipients.Any(o => o.Id == user.Id))
                Unauthorized();
        }

        async Task<bool> MessageExists(Guid id)
        {
            //return db.Messages.Count(e => e.Id == id) > 0;
            return (await MessageRepo.CountAsync(e => e.Id == id)) > 0;
        }

        #endregion Private Methods

    }
}