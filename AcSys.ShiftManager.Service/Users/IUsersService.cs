using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AcSys.ShiftManager.Data.Users;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Service.Base;
using AcSys.ShiftManager.Service.Results;

namespace AcSys.ShiftManager.Service.Users
{
    public interface IUsersService : IApplicationService
    {
        Task<List<RoleDto>> GetRoles();
        Task<RoleDto> GetRole(Guid id);

        UserDto Me();

        Task<IListResult<User, UserDto>> GetUsers(FindUsersQuery query);

        Task<UserDto> GetUser(Guid id);

        Task UpdateUser(Guid id, UserDto dto);

        Task<Guid> CreateUser(UserDto dto);

        Task ActivateUser(Guid id);

        Task DeactivateUser(Guid id);

        Task DeleteUser(Guid id);

        Task<List<UserNotificationDto>> GetUserNotifications();
    }
}
