namespace AcSys.Core.Data.Model.Base
{
    public interface INamedEntity : IEntity
    {
        string Name { get; set; }
    }
}
