using System;

namespace AcSys.ShiftManager.Service.Common
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException()
        {

        }

        public ForbiddenException(string message)
            : base(message)
        {
            
        }
    }
}
