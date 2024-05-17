using AcSys.Core.Data.EF.Mappings.Base;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Mappings
{
    public class MessageMap : EntityMapBase<Message>
    {
        public MessageMap()
        {

            this.HasMany(e => e.Views)
                .WithRequired(e => e.Message);

            this.HasMany(e => e.Recipients)
                .WithMany(e => e.IncomingMessages);

            this.HasRequired(e => e.Sender)
                .WithMany(e => e.OutgoingMessages);

            this.Property(e => e.SentAt)
                .IsRequired();

            this.Property(e => e.Subject)
                .IsRequired()
                ;//.HasMaxLength(250);

            this.Property(e => e.Text)
                .IsRequired()
                ;//.HasMaxLength(2500);
        }
    }
}
