using System.Collections.Generic;

namespace AcSys.ShiftManager.Service.Shifts
{
    public class RotaDto
    {
        public List<ShiftBasicDetailsDto> OpenShifts { get; set; }
        public List<RotaItemDto> Items { get; set; }
    }
}
