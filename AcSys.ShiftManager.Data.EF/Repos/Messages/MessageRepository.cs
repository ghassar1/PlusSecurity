using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Data.Messages;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Repos.Messages
{
    public class MessageRepository : GenericRepository<ApplicationDbContext, Message>, IMessageRepository
    {
        public ApplicationDbContext Context { get; set; }

        public MessageRepository(ApplicationDbContext context)
            :base(context)
        {
            Context = context;
        }
    }
}
