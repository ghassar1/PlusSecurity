using AcSys.ShiftManager.Service.Common;

namespace AcSys.ShiftManager.Service.Users
{
    public class UserBasicDetailsDto : EntityDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
