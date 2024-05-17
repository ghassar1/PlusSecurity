using System;

namespace AcSys.ShiftManager.Service.Common
{
    public interface IDto : ICloneable
    {
        void Validate();
    }
}
