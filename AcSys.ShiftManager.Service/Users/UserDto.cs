using System;
using AcSys.ShiftManager.Service.Common;
using AcSys.ShiftManager.Service.EmployeeGroups;

namespace AcSys.ShiftManager.Service.Users
{
    public class UserDto : EntityDto
    {
        public UserDto()
        {
            Role = new RoleDto();
        }

        public string Email { get; set; }
        public string UserName { get; set; }

        public bool MustChangePassword { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string WorkNumber { get; set; }
        public string Location { get; set; }
        public bool HasDrivingLicense { get; set; }

        public string Mobile { get; set; }
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }

        

        public DateTime DateOfBirth { get; set; }

        public RoleDto Role { get; set; }

        public EmployeeGroupDto EmployeeGroup { get; set; }
    }
}
