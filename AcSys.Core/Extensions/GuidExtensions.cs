using System;

namespace AcSys.Core.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsEmpty(this Guid e)
        {
            return e == Guid.Empty;
        }

        public static bool IsNotEmpty(this Guid e)
        {
            return !IsEmpty(e);
        }

        public static bool IsNullOrEmpty(this Guid? e)
        {
            return !e.HasValue || e.Value == Guid.Empty;
        }

        public static bool IsNotNullOrEmpty(this Guid? e)
        {
            return !IsNullOrEmpty(e);
        }
    }
}
