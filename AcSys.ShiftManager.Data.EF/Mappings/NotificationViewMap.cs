using AcSys.Core.Data.EF.Mappings.Base;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Mappings
{
    public class NotificationViewMap : EntityMapBase<NotificationView>
    {
        public NotificationViewMap()
        {

            this.HasRequired(e => e.Notification)
                .WithMany(e => e.Views);

            this.HasRequired(e => e.User)
                .WithMany(e => e.NotificationViews);

            this.Property(e => e.ViewedAt)
                .IsRequired();
        }
    }
}
