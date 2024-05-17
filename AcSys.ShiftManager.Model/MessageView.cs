using System;
using AcSys.Core.Data.Model.Base;

namespace AcSys.ShiftManager.Model
{
    public class MessageView : EntityBase
    {
        public virtual Message Message { get; set; }
        public virtual User User { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
