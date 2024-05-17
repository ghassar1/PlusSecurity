using System;
using System.Threading.Tasks;
using AcSys.Core.Data.Querying;
using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.Shifts
{
    public interface IShiftRepository : IGenericRepository<Shift>
    {
        //Task<IListResult<T>> Find(FindShiftsQuery query);
    }
}
