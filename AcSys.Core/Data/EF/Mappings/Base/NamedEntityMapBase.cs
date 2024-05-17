using AcSys.Core.Data.Model.Base;

namespace AcSys.Core.Data.EF.Mappings.Base
{
    public class NamedEntityMapBase<T> : EntityMapBase<T>
                where T : NamedEntityBase
    {
        public NamedEntityMapBase()
        {
            Property(o => o.Name)
                .IsRequired();
        }
    }
}
