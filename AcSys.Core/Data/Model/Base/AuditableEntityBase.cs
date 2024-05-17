using System;

namespace AcSys.Core.Data.Model.Base
{
    public abstract class AuditableEntityBase : EntityBase//, IAuditableEntity
    {
        public DateTime? CreatedDate { get; set; }
        //public virtual User CreatedByUser { get; set; }
        public string CreatedByUser { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public string UpdatedByUser { get; set; }
        //public virtual User UpdatedByUser { get; set; }
    }
}
