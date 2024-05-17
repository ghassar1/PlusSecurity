using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AcSys.Core.Data.Model.Base;
using AcSys.Core.Email;
using AcSys.Core.ObjectMapping;
using AcSys.ShiftManager.Data.EF.Identity;
using AcSys.ShiftManager.Data.UnitOfWork;
using AcSys.ShiftManager.Data.Users;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Model.Helpers;
using AcSys.ShiftManager.Service.Base;
using AcSys.ShiftManager.Service.Results;
using Autofac.Extras.NLog;

namespace AcSys.ShiftManager.Service.EmployeeGroups
{
    public class EmployeeGroupsService : ApplicationServiceBase, IEmployeeGroupsService
    {
        protected IEmployeeGroupRepository  Repo { get; set; }
        
        public EmployeeGroupsService(
            IUnitOfWork unitOfWork,
            ApplicationRoleManager roleManager,
            ApplicationUserManager userManager,
            IEmployeeGroupRepository repo,
            ILogger logger,
            IEmailService emailService
            ) : base(unitOfWork, roleManager, userManager, logger, emailService)
        {
            Repo = repo;
        }

        public async Task<IListResult<EmployeeGroupDto>> Get()
        {
            //var employeeGroups = await Repo.GetAllAsync();
            var employeeGroups = await Repo.FindAsync(o => o.EntityStatus == EntityStatus.Active);
            var dtos = ObjectMapper.Map<List<EmployeeGroup>, List<EmployeeGroupDto>>(employeeGroups);

            IListResult<EmployeeGroupDto> result = new ListResult<EmployeeGroupDto>(dtos);

            LoggedInUser.AddLog(Enums.SubjectType.EmployeeGroup, Enums.ActivityType.Viewed);
            await UnitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<EmployeeGroupDto> Get(Guid id)
        {
            EmployeeGroup employeeGroup = await Repo.FindAsync(id, true);
            if (employeeGroup == null) NotFound();

            var dto = ObjectMapper.Map<EmployeeGroup, EmployeeGroupDto>(employeeGroup);

            LoggedInUser.AddLog(Enums.ActivityType.Viewed, employeeGroup);
            await UnitOfWork.SaveChangesAsync();

            return dto;
        }

        public async Task Update(Guid id, EmployeeGroupDto dto)
        {
            EmployeeGroup employeeGroup = await Repo.FindAsync(id);
            if (employeeGroup == null) NotFound();

            employeeGroup.Name = dto.Name;

            Repo.Update(employeeGroup);

            LoggedInUser.AddLog(Enums.ActivityType.Updated, employeeGroup);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<Guid> Create(EmployeeGroupDto dto)
        {
            EmployeeGroup employeeGroup = new EmployeeGroup()
            {
                Name = dto.Name
            };

            Repo.Add(employeeGroup);
            await UnitOfWork.SaveChangesAsync();

            LoggedInUser.AddLog(Enums.ActivityType.Created, employeeGroup);
            await UnitOfWork.SaveChangesAsync();

            return employeeGroup.Id;
        }

        public async Task Delete(Guid id)
        {
            EmployeeGroup employeeGroup = await Repo.FindAsync(id);
            if (employeeGroup == null) NotFound();

            //if (employeeGroup.HasChildRecords())
            //    BadRequest("Cannot delete employee group. The group has been assigned to some employees.");

            //employeeGroup.Delete();
            Repo.Delete(employeeGroup);

            LoggedInUser.AddLog(Enums.ActivityType.Deleted, employeeGroup);

            await UnitOfWork.SaveChangesAsync();
        }
    }
}
