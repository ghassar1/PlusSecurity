using System;

namespace AcSys.Core.Data.Model.Base
{
    public interface IEntity
    {
        Guid Id { get; set; }

        byte[] Timestamp { get; set; }

        /// ETags (a.k.a Sequence IDs) are used for optimistic concurrency control. 
        /// ETag is a key that changes each time the object is updated. Whenever the an update occurs 
        /// the current ETag must be included in the update request or the update will fail. 
        /// This ensures that the changes are based on the latest copy of the data and 
        /// prevents one user from overwriting the changes of another.
        /// https://blog.4psa.com/rest-best-practices-managing-concurrent-updates/
        /// http://control.cyriouswiki.com/file/view/2013-08-08+WAPIC+Specifications.pdf
        //string ETag { get; set; }

        EntityStatus EntityStatus { get; set; }

        bool IsLocked { get; set; }

        void Lock();
        void Unlock();

        void Activate();
        void Deactivate();

        bool HasChildRecords();
        void Delete();

        //void Archive();
        //bool IsArchived();
        //bool IsNotArchived();

        bool IsActive();
        bool IsNotActive();

        bool IsDeactivated();
        bool IsNotDeactivated();

        bool IsDeleted();
        bool IsNotDeleted();

        void Validate();

        bool HasDescription();
        string ToDescription();
    }
}
