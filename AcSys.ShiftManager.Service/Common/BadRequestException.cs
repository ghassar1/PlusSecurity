using System;

namespace AcSys.ShiftManager.Service.Common
{
    public class BadRequestException : ApplicationException
    {
        public BadRequestException()
        {

        }

        public BadRequestException(string message)
            : base(message)
        {

        }

        public BadRequestException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
