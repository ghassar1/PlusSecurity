using System.ComponentModel;

namespace AcSys.Core.Data.Model.Base
{
    public enum EntityStatus
    {
        [Description("Inactive")]
        Inactive = 0,

        [Description("Active")]
        Active = 1,

        [Description("Deactivated")]
        Deactivated = 2,

        [Description("Deleted")]
        Deleted = 3,

        [Description("Archived")]
        Archived = 4
    }
}
