namespace AcSys.Core.Data.Querying
{
    public interface IResult<T>
    {
        T Record { get; set; }
    }
}
