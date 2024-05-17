using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AcSys.Core.Data.Model.Base;

namespace AcSys.Core.Data.EF.Mappings.Base
{
    public class EntityMapBase<T> : EntityTypeConfiguration<T>
        where T : EntityBase
    {
        public EntityMapBase()
        {
            // Primary Key
            HasKey(e => e.Id)
                .Property(e => e.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Timestamp (Row Version)
            Property(e => e.Timestamp)
                .IsRequired()
                .IsRowVersion();

            Property(e => e.EntityStatus)
                .IsRequired();
        }
    }
}
