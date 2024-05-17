using AcSys.Core.Data.EF.Mappings.Base;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Mappings
{
    public class ShiftMap : EntityMapBase<Shift>
    {
        public ShiftMap()
        {
            HasOptional(e => e.Employee)
                .WithMany(a => a.Shifts);

            //Property(e => e.IsOpen)
            //    .IsRequired();

            Property(e => e.Title)
                .IsRequired();

            Property(e => e.TotalBreakMins)
                .IsRequired();

            //Property(e => e.Date)
            //    .IsRequired();

            Property(e => e.StartTime)
                .IsRequired()
                ;//.HasColumnType("datetime2");

            Property(e => e.EndTime)
                .IsRequired()
                ;//.HasColumnType("datetime2");

            Property(e => e.ClockInTime)
                .IsOptional()
                ;//.HasColumnType("datetime2");

            Property(e => e.ClockOutTime)
                .IsOptional()
                ;//.HasColumnType("datetime2");
        }
    }
}
