using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AcSys.ShiftManager.Data.Notifications;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Service.Notifications;
using AcSys.ShiftManager.Service.Results;
using AcSys.ShiftManager.Service.Users;
using Autofac.Extras.NLog;

namespace AcSys.ShiftManager.App.Controllers
{
    [Authorize]
    [RoutePrefix("api/Notifications")]
    public class NotificationsController : ApiControllerBase
    {
        INotificationsService Service = null;
        
        public NotificationsController(INotificationsService service, ILogger logger)
            : base(service, logger)
        {
            Service = service;
        }

        // GET: api/Notifications/Mine/New
        [HttpGet]
        [Route("Mine/New")]
        [ResponseType(typeof(Task<List<NotificationDto>>))]
        public async Task<IHttpActionResult> GetMyNewNotifications()
        {
            List<NotificationDto> dtos = await Service.GetMyNewNotifications();
            return Ok(dtos);
        }

        // GET: api/Notifications/Mine
        [HttpGet]
        [Route("Mine")]
        [ResponseType(typeof(Task<IListResult<Notification, NotificationDto>>))]
        public async Task<IHttpActionResult> GetMyNotifications([FromUri]FindMyNotificationsQuery query)
        {
            //throw new ApplicationException("Error fetching notifications!");
            IListResult<Notification, NotificationDto> result = await Service.GetMyNotifications(query);
            return Ok(result);
        }

        // GET: api/Notifications
        [HttpGet]
        [ResponseType(typeof(Task<IListResult<Notification, NotificationDto>>))]
        public async Task<IHttpActionResult> GetNotifications([FromUri]FindNotificationsQuery query)
        {
            IListResult<Notification, NotificationDto> result = await Service.GetNotifications(query);
            return Ok(result);
        }

        // GET: api/Notifications/5
        [ResponseType(typeof(NotificationDto))]
        public async Task<IHttpActionResult> GetNotification(Guid id)
        {
            NotificationDto dto = await Service.GetNotification(id);
            return Ok(dto);
        }

        // PUT: api/Notifications/5/read
        [HttpPut]
        [Route("{id}/read")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> MarkAsRead(Guid id)
        {
            await Service.MarkAsRead(id);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Notifications/5/unread
        [HttpPut]
        [Route("{id}/unread")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> MarkAsUnread(Guid id)
        {
            await Service.MarkAsUnread(id);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Notifications/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNotification(Guid id, NotificationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await Service.UpdateNotification(id, dto);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Notifications
        [HttpPost]
        [ResponseType(typeof(Guid))]        //Notification
        public async Task<IHttpActionResult> PostNotification(NotificationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Guid id = await Service.CreateNotification(dto);

            //return CreatedAtRoute("DefaultApi", new { id = notification.Id }, notification);
            return Ok(id);
        }

        // DELETE: api/Notifications/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteNotification(Guid id)
        {
            await Service.DeleteNotification(id);
            return Ok();
        }

        // GET: api/Notifications/Recipients
        [HttpGet]
        [Route("Recipients")]
        [ResponseType(typeof(Task<List<UserBasicDetailsDto>>))]
        public async Task<IHttpActionResult> GetRecipients()
        {
            List<UserBasicDetailsDto> dtos = await Service.GetRecipients();
            return Ok(dtos);
        }
    }
}
