using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Data.Messages;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Repos.Messages
{
    public class MessageViewRepository : GenericRepository<ApplicationDbContext, MessageView>, IMessageViewRepository
    {
        public ApplicationDbContext Context { get; set; }

        public MessageViewRepository(ApplicationDbContext context)
            : base(context)
        {
            Context = context;
        }
    }
}
