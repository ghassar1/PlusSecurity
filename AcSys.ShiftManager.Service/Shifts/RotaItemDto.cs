using System.Collections.Generic;
using AcSys.ShiftManager.Service.Common;

namespace AcSys.ShiftManager.Service.Shifts
{
    public class RotaItemDto
    {
        public RotaItemDto()
        {
            Shifts = new List<ShiftBasicDetailsDto>();
        }

        public NamedEntityDto Employee { get; set; }
        public List<ShiftBasicDetailsDto> Shifts { get; set; }
    }
}
