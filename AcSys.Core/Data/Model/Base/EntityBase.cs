using System;
using AcSys.Core.Extensions;

namespace AcSys.Core.Data.Model.Base
{
    public abstract class EntityBase : IEntity
    {
        public EntityBase()
        {
            EntityStatus = EntityStatus.Active;
            IsLocked = false;
        }

        public Guid Id { get; set; }

        public byte[] Timestamp { get; set; }

        //public string ETag { get; set; }

        public EntityStatus EntityStatus { get; set; }

        public bool IsLocked { get; set; }

        public void Lock()
        {
            IsLocked = true;
        }

        public void Unlock()
        {
            IsLocked = true;
        }

        public void Activate()
        {
            EntityStatus = EntityStatus.Active;
        }

        public void Deactivate()
        {
            EntityStatus = EntityStatus.Deactivated;
        }

        public void Delete()
        {
            EntityStatus = EntityStatus.Deleted;
        }

        public void Archive()
        {
            EntityStatus = EntityStatus.Archived;
        }

        public bool IsArchived()
        {
            return EntityStatus == EntityStatus.Archived;
        }

        public bool IsNotArchived()
        {
            return !IsArchived();
        }

        public bool IsActive()
        {
            return EntityStatus == EntityStatus.Active;
        }

        public bool IsNotActive()
        {
            return !IsActive();
        }

        public bool IsDeactivated()
        {
            return EntityStatus == EntityStatus.Inactive;
        }

        public bool IsNotDeactivated()
        {
            return !IsDeactivated();
        }

        public bool IsDeleted()
        {
            return EntityStatus == EntityStatus.Deleted;
        }

        public bool IsNotDeleted()
        {
            return !IsDeleted();
        }

        public virtual void Validate()
        {
        }

        public virtual bool HasDescription()
        {
            return ToDescription().IsNotNullOrWhiteSpace();
        }

        public virtual string ToDescription()
        {
            return string.Empty;
        }

        public virtual bool HasChildRecords()
        {
            return false;
        }
    }
}
