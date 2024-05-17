namespace AcSys.ShiftManager.Service.Results
{
    public interface ISingleResult<T> : IServiceResult
    {
        T Items { get; set; }
    }
}
