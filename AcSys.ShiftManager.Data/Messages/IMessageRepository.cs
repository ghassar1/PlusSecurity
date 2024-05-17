using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.Messages
{
    public interface IMessageRepository : IGenericRepository<Message>, IRepository
    {
        
    }
}
