using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AcSys.ShiftManager.Data.Notifications;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Service.Base;
using AcSys.ShiftManager.Service.Results;
using AcSys.ShiftManager.Service.Users;

namespace AcSys.ShiftManager.Service.Notifications
{
    public interface INotificationsService : IApplicationService
    {
        Task<List<NotificationDto>> GetMyNewNotifications();
        Task<IListResult<Notification, NotificationDto>> GetMyNotifications(FindMyNotificationsQuery query);

        Task<IListResult<Notification, NotificationDto>> GetNotifications(FindNotificationsQuery query);

        Task<NotificationDto> GetNotification(Guid id);

        Task MarkAsRead(Guid id);

        Task MarkAsUnread(Guid id);

        Task UpdateNotification(Guid id, NotificationDto dto);

        Task<Guid> CreateNotification(NotificationDto dto);

        Task DeleteNotification(Guid id);

        Task<List<UserBasicDetailsDto>> GetRecipients();
    }
}
