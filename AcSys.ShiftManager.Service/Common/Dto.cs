namespace AcSys.ShiftManager.Service.Common
{
    public abstract class Dto : IDto
    {
        public object Clone()
        {
            return MemberwiseClone();
        }

        public virtual void Validate()
        {
        }
    }
}
