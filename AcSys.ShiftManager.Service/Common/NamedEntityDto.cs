using AcSys.Core.Extensions;

namespace AcSys.ShiftManager.Service.Common
{
    public class NamedEntityDto : EntityDto
    {
        public string Name { get; set; }

        public bool HasName { get { return Name.IsNotNullOrWhiteSpace(); } }

        public bool HasNoName { get { return Name.IsNullOrWhiteSpace(); } }
    }
}
