using AcSys.Core.Data.EF.Mappings.Base;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Mappings
{
    public class MessageViewMap : EntityMapBase<MessageView>
    {
        public MessageViewMap()
        {

            this.HasRequired(e => e.Message)
                .WithMany(e => e.Views);

            this.HasRequired(e => e.User)
                .WithMany(e => e.MessageViews);

            this.Property(e => e.ViewedAt)
                .IsRequired();
        }
    }
}
