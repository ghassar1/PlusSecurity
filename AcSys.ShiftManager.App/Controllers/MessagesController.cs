using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AcSys.ShiftManager.Data.Messages;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Model.Helpers;
using AcSys.ShiftManager.Service.Messages;
using AcSys.ShiftManager.Service.Results;
using AcSys.ShiftManager.Service.Users;
using Autofac.Extras.NLog;

namespace AcSys.ShiftManager.App.Controllers
{
    [Authorize]
    [RoutePrefix("api/Messages")]
    public class MessagesController : ApiControllerBase
    {
        IMessagesService Service { get; set; }

        public MessagesController(IMessagesService service, ILogger logger)
            : base(service, logger)
        {
            Service = service;
        }

        // GET: api/Messages/Mine/New
        [HttpGet]
        [Route("Mine/New")]
        [ResponseType(typeof(Task<List<MessageDto>>))]
        public async Task<IHttpActionResult> GetMyNewMessages()
        {
            List<MessageDto> dtos = await Service.GetMyNewMessages();
            return Ok(dtos);
        }

        // GET: api/Messages/All
        [HttpGet]
        [Route("All")]
        [Authorize(Roles = 
            AppConstants.RoleNames.SuperAdmin + ", " + 
            AppConstants.RoleNames.SuperAdmin)]
        [ResponseType(typeof(Task<List<MessageDto>>))]
        public async Task<IHttpActionResult> GetMyMessages()
        {
            List<MessageDto> dtos = await Service.GetMessages();
            return Ok(dtos);
        }

        // GET: api/Messages
        [HttpGet]
        [ResponseType(typeof(Task<IListResult<Message, MessageDto>>))]
        public async Task<IHttpActionResult> GetMessages([FromUri]FindMyInBoxMessagesQuery query)
        {
            IListResult<Message, MessageDto> result = await Service.GetMyMessages(query);
            return Ok(result);
        }

        // GET: api/Messages/Sent
        [HttpGet]
        [Route("Sent")]
        [ResponseType(typeof(Task<IListResult<Message, MessageDto>>))]
        public async Task<IHttpActionResult> GetSentMessages([FromUri]FindMySentMessagesQuery query)
        {
            IListResult<Message, MessageDto> result = await Service.GetSentMessages(query);
            return Ok(result);
        }

        // GET: api/Messages/5
        [ResponseType(typeof(MessageDto))]
        public async Task<IHttpActionResult> GetMessage(Guid id)
        {
            MessageDto dto = await Service.GetMessage(id);
            return Ok(dto);
        }

        // PUT: api/Messages/5/read
        [HttpPut]
        [Route("{id}/read")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> MarkAsRead(Guid id)
        {
            await Service.MarkAsRead(id);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Messages/5/unread
        [HttpPut]
        [Route("{id}/unread")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> MarkAsUnread(Guid id)
        {
            await Service.MarkAsUnread(id);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Messages/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMessage(Guid id, MessageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await Service.UpdateMessage(id, dto);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Messages
        [HttpPost]
        [ResponseType(typeof(Guid))]        //Message
        public async Task<IHttpActionResult> PostMessage(MessageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Guid id = await Service.CreateMessage(dto);

            //return CreatedAtRoute("DefaultApi", new { id = message.Id }, message);
            return Ok(id);
        }

        // DELETE: api/Messages/5
        [HttpDelete]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteMessage(Guid id)
        {
            await Service.DeleteMessage(id);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: api/Messages/Recipients
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
