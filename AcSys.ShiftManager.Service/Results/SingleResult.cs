namespace AcSys.ShiftManager.Service.Results
{
    public class SingleResult<T> : ServiceResult, ISingleResult<T>
    {
        public T Items { get; set; }
    }
}
