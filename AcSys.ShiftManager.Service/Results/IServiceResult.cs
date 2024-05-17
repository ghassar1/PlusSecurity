namespace AcSys.ShiftManager.Service.Results
{
    public interface IServiceResult
    {
        bool Success { get; set; }
        string Message { get; set; }
    }
}
