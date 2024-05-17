using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AcSys.ShiftManager.Data.Messages;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Service.Base;
using AcSys.ShiftManager.Service.Results;
using AcSys.ShiftManager.Service.Users;

namespace AcSys.ShiftManager.Service.Messages
{
    public interface IMessagesService : IApplicationService
    {
        Task<List<MessageDto>> GetMyNewMessages();

        Task<IListResult<Message, MessageDto>> GetMyMessages(FindMyInBoxMessagesQuery query);

        Task<List<MessageDto>> GetMessages();

        Task<IListResult<Message, MessageDto>> GetSentMessages(FindMySentMessagesQuery query);

        Task<MessageDto> GetMessage(Guid id);

        Task MarkAsRead(Guid id);

        Task MarkAsUnread(Guid id);

        Task UpdateMessage(Guid id, MessageDto dto);

        Task<Guid> CreateMessage(MessageDto dto);

        Task DeleteMessage(Guid id);

        Task<List<UserBasicDetailsDto>> GetRecipients();
    }
}
