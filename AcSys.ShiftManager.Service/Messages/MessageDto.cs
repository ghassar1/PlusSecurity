using System;
using System.Collections.Generic;
using AcSys.ShiftManager.Service.Common;
using AcSys.ShiftManager.Service.Users;

namespace AcSys.ShiftManager.Service.Messages
{
    public class MessageDto : EntityDto
    {
        public MessageDto()
        {
            this.Recipients = new List<UserBasicDetailsDto>();
            this.Viewers = new List<UserBasicDetailsDto>();
    }

        public UserDto Sender { get; set; }

        public string Subject { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }

        public bool IsViewed { get; set; }

        public DateTime? ViewedAt { get; set; }

        public List<UserBasicDetailsDto> Recipients { get; set; }
        public List<UserBasicDetailsDto> Viewers { get; set; }
    }
}
