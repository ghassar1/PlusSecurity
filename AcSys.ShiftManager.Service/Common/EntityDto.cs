using System;
using AcSys.Core.Data.Model.Base;
using AcSys.Core.Extensions;

namespace AcSys.ShiftManager.Service.Common
{
    public class EntityDto : Dto
    {
        public EntityDto()
        {
            EntityStatus = EntityStatus.Active;
        }

        public Guid? Id { get; set; }
        //public bool? IsLocked { get; set; }

        public EntityStatus EntityStatus { get; set; }
        public string EntityStatusDesc{ get; set; }

        public bool HasId { get { return Id.IsNotNullOrEmpty(); } }

        public bool HasNoId { get { return Id.IsNullOrEmpty(); } }
    }
}
