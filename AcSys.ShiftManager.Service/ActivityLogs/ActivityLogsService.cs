using AcSys.Core.Data.Querying;
using AcSys.Core.Data.Repository;
using AcSys.Core.Email;
using AcSys.Core.ObjectMapping;
using AcSys.ShiftManager.Data.ActivityLogs;
using AcSys.ShiftManager.Data.EF.Identity;
using AcSys.ShiftManager.Data.UnitOfWork;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Service.Base;
using AcSys.ShiftManager.Service.Results;
using Autofac.Extras.NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AcSys.ShiftManager.Service.ActivityLogs
{
    public class ActivityLogsService : ApplicationServiceBase, IActivityLogsService
    {
        IGenericRepository<ActivityLog> Repo { get; set; }

        public ActivityLogsService(IUnitOfWork unitOfWork,
            ApplicationRoleManager roleManager,
            ApplicationUserManager userManager,
            IGenericRepository<ActivityLog> repo,
            ILogger logger,
            IEmailService emailService)
             : base(unitOfWork, roleManager, userManager, logger, emailService)
        {
            Repo = repo;
        }

        public async Task<IListResult<ActivityLog, ActivityLogListItemDto>> Get(FindLogsQuery query)
        {
            ISearchResult<ActivityLog> searchResults = await Repo.Find(query);
            ListResult<ActivityLog, ActivityLogListItemDto> result = new ListResult<ActivityLog, ActivityLogListItemDto>(query, searchResults);
            return result;
        }

        public async Task<ActivityLogDetailsDto> Get(Guid id)
        {
            ActivityLog activityLog = await Repo.FindAsync(id, true);
            ActivityLogDetailsDto dto = ObjectMapper.Map<ActivityLog, ActivityLogDetailsDto>(activityLog);
            return dto;
        }

        public async Task Delete(Guid id)
        {
            ActivityLog activityLog = await Repo.FindAsync(id, true);

            //if (activityLog.HasChildRecords())
            //    BadRequest("Activity log cannot be deleted.");

            Repo.Delete(activityLog);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(List<Guid> ids)
        {
            Repo.Delete(ids);
            await UnitOfWork.SaveChangesAsync();
        }
    }
}
