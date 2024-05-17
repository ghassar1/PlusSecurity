using AcSys.Core.Data.EF.Mappings.Base;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Mappings
{
    public class NotificationMap : EntityMapBase<Notification>
    {
        public NotificationMap()
        {

            HasMany(e => e.Views)
                .WithRequired(e => e.Notification);

            HasMany(e => e.Recipients)
                .WithMany(e => e.Notifications);

            //HasRequired(e => e.Sender)
            //    .WithMany(e => e.AuthoredNotifications);

            HasOptional(e => e.Sender)
                .WithMany(e => e.AuthoredNotifications);

            //Property(e => e.Source)
            //    .IsRequired();

            Property(e => e.Type)
                .IsRequired();

            Property(e => e.SentAt)
                .IsRequired();

            Property(e => e.Text)
                .IsRequired()
                ;//.HasMaxLength(2500);

            Property(e => e.Title)
                .IsRequired()
                ;//.HasMaxLength(250);
        }
    }
}
