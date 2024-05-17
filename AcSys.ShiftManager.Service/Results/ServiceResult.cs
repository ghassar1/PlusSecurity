namespace AcSys.ShiftManager.Service.Results
{
    public class ServiceResult : IServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
