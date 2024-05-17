using System.Linq;
using AcSys.Core.Extensions;
using AcSys.ShiftManager.Service.Messages;
using AcSys.ShiftManager.Service.Notifications;
using AcSys.ShiftManager.Service.Users;
using AcSys.ShiftManager.Model;
using AutoMapper;
using AcSys.ShiftManager.Service.ActivityLogs;
using AcSys.ShiftManager.Service.EmployeeGroups;
using AcSys.ShiftManager.Service.Shifts;
using AcSys.ShiftManager.Service.Common;

namespace AcSys.ShiftManager.App
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<EntityDto, EntityBase>().ReverseMap();

            CreateMap<Role, RoleDto>()
                .ReverseMap()
                .ForMember(dest => dest.UserRoles, opts => opts.Ignore());

            CreateMap<User, EntityDto>();
            CreateMap<User, NamedEntityDto>()
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.GetFullName()));

            CreateMap<User, UserBasicDetailsDto>()
                .ForMember(dest => dest.EntityStatusDesc, opts => opts.MapFrom(src => src.EntityStatus.ToDescription()))
                .ReverseMap()
                .ForMember(dest => dest.UserRoles, opts => opts.Ignore());

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opts => opts.MapFrom(src => src.UserRoles.Select(r => r.Role).FirstOrDefault()))
                .ForMember(dest => dest.EntityStatusDesc, opts => opts.MapFrom(src => src.EntityStatus.ToDescription()))
                .ForMember(dest => dest.Password, opts => opts.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.UserRoles, opts => opts.Ignore())
                .ForMember(dest => dest.PasswordHash, opts => opts.Ignore())
                .ForMember(dest => dest.EntityStatus, opts => opts.Ignore())
                //.ForMember(dest => dest.City, opts => opts.Ignore())
                .ForMember(dest => dest.EmployeeGroup, opts => opts.Ignore())
                .ForMember(dest => dest.EmailConfirmed, opts => opts.Ignore())
                .ForMember(dest => dest.Id, opts => opts.Ignore());

            CreateMap<EmployeeGroup, EmployeeGroupDto>()
                .ForMember(dest => dest.NoOfEmployees, opts => opts.MapFrom(src => src.Employees.Count()))
                .ForMember(dest => dest.EntityStatusDesc, opts => opts.MapFrom(src => src.EntityStatus.ToDescription()))
                .ReverseMap();

            //CreateMap<User, LoggedInUserDto>()
            //    .ForMember(dest => dest.Role, opts => opts.MapFrom(src => src.Roles.FirstOrDefault()))
            //    .ReverseMap();

            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Recipients, opts => opts.Ignore())
                .ForMember(dest => dest.TypeDesc, opts => opts.MapFrom(src => src.Type.ToDescription()))
                //.ForMember(dest => dest.SourceDesc, opts => opts.MapFrom(src => src.Source.ToDescription()))
                .ReverseMap()
                .ForMember(dest => dest.Recipients, opts => opts.Ignore());

            CreateMap<Notification, UserNotificationDto>()
                .ForMember(dest => dest.IsMessage, opts => opts.MapFrom(src => false))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
                .ForMember(dest => dest.Sender, opts => opts.MapFrom(src => src.Sender.ToString()));

            CreateMap<Message, MessageDto>()
                //.ForMember(dest => dest.Recipients, opts => opts.Ignore())
                .ForMember(dest => dest.Viewers, opts => opts.MapFrom(src => src.Views.Select(o => o.User)))
                .ReverseMap()
                ;//.ForMember(dest => dest.Recipients, opts => opts.Ignore());

            CreateMap<Message, UserNotificationDto>()
                .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Subject))
                .ForMember(dest => dest.IsMessage, opts => opts.MapFrom(src => true))
                .ForMember(dest => dest.Sender, opts => opts.MapFrom(src => src.Sender.ToString()));

            CreateMap<ActivityLog, ActivityLogListItemDto>()
                .ForMember(dest => dest.TypeDesc, opts => opts.MapFrom(src => src.Type.ToDescription()))
                .ForMember(dest => dest.SubjectTypeDesc, opts => opts.MapFrom(src => src.SubjectType.ToDescription()))
                .ReverseMap();

            CreateMap<ActivityLog, ActivityLogDetailsDto>()
                .ForMember(dest => dest.TypeDesc, opts => opts.MapFrom(src => src.Type.ToDescription()))
                .ForMember(dest => dest.SubjectTypeDesc, opts => opts.MapFrom(src => src.SubjectType.ToDescription()))
                .ReverseMap();

            CreateMap<CreateShiftDto, Shift>()
                .ForMember(dest => dest.Employee, opts => opts.Ignore())
                .ForMember(dest => dest.ClockInTime, opts => opts.Ignore())
                .ForMember(dest => dest.ClockOutTime, opts => opts.Ignore());

            CreateMap<Shift, ShiftDto>()
                .ForMember(dest => dest.IsOpen, opts => opts.MapFrom(src => src.Employee == null))
                .ForMember(dest => dest.StatusDesc, opts => opts.MapFrom(src => src.Status.ToDescription()))
                .ForMember(dest => dest.StartStatusDesc, opts => opts.MapFrom(src => src.StartStatus.ToDescription()))
                .ForMember(dest => dest.EndStatusDesc, opts => opts.MapFrom(src => src.EndStatus.ToDescription()));
            //.ForMember(dest => dest.ClockInTime, opts => opts.MapFrom(src => src.ClockInTime.Value))
            //.ForMember(dest => dest.ClockOutTime, opts => opts.MapFrom(src => src.ClockOutTime.Value));

            CreateMap<Shift, ShiftBasicDetailsDto>()
                .ForMember(dest => dest.IsOpen, opts => opts.MapFrom(src => src.Employee == null))
                .ForMember(dest => dest.StatusDesc, opts => opts.MapFrom(src => src.Status.ToDescription()))
                .ForMember(dest => dest.StartStatusDesc, opts => opts.MapFrom(src => src.StartStatus.ToDescription()))
                .ForMember(dest => dest.EndStatusDesc, opts => opts.MapFrom(src => src.EndStatus.ToDescription()));
                //.ForMember(dest => dest.ClockInTime, opts => opts.MapFrom(src => src.ClockInTime.Value))
                //.ForMember(dest => dest.ClockOutTime, opts => opts.MapFrom(src => src.ClockOutTime.Value));

            //CreateMap<Shift, ShiftDetailsDto>()
            //    .ForMember(dest => dest.IsOpen, opts => opts.MapFrom(src => src.Employee == null));

            CreateMap<Shift, UpdateShiftDto>();

            CreateMap<Shift, AttendanceReportRowDto>()
                .ForMember(dest => dest.Attendance, opts => opts.MapFrom(src => src.Status.ToDescription()))
                .ForMember(dest => dest.Date, opts => opts.MapFrom(src => src.StartTime.Date))
                .ForMember(dest => dest.EmployeeId, opts => opts.MapFrom(src => src.Employee.Id))
                .ForMember(dest => dest.EmployeeName, opts => opts.MapFrom(src => src.Employee.GetFullName()))
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Timings, opts => opts.MapFrom(src => src.ClockInTime.HasValue && src.ClockOutTime.HasValue ? src.ClockInTime.Value.ToString("hh:mm tt") + " - " + src.ClockOutTime.Value.ToString("hh:mm tt") : "Not Attended"))
                .ForMember(dest => dest.LateMins, opts => opts.MapFrom(src => src.LateMins > 0 ? src.LateMins : 0));

            //Mapper.AssertConfigurationIsValid();
        }
    }
}
