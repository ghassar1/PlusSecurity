using System;
using System.Linq;
using System.Linq.Expressions;
using AcSys.Core.Data.Querying;
using AcSys.Core.Data.Specifications;
using AcSys.Core.Extensions;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Model.Helpers;

namespace AcSys.ShiftManager.Data.Users
{
    public class FindUsersQuery : SearchQuery<User> //PagingQuery<User>
    {
        //public string RoleFilter { get; set; }
        public string[] IncludeRoles { get; set; }
        public string[] ExcludeRoles { get; set; }

        public bool FilterUsersInNoGroup { get; set; }
        //public string[] EmployeeGroupNames { get; set; }
        public Guid[] EmployeeGroupIds { get; set; }

        public override ISpecification<User> ToSpec()
        {
            ISpecification<User> spec = new Specification<User>();

            //if (RoleFilter.IsNotNullOrWhiteSpace())
            //{
            //    spec = spec.And(o => o.UserRoles.Select(ur => ur.Role.Name).Contains(RoleFilter));
            //}

            if (Status.HasValue)
            {
                spec = spec.And(o => o.EntityStatus == Status.Value);
            }

            if (IncludeRoles != null && IncludeRoles.Length > 0)
            {
                spec = spec.And(o => o.UserRoles.Select(ur => ur.Role).Any(r => IncludeRoles.Contains(r.Name)));
            }

            if (ExcludeRoles != null && ExcludeRoles.Length > 0)
            {
                spec = spec.And(o => o.UserRoles.Select(ur => ur.Role).Any(r => !ExcludeRoles.Contains(r.Name)));
            }

            //if (EmployeeGroupNames != null && EmployeeGroupNames.Count() > 0)
            //{
            //    spec = spec.And(o => o.EmployeeGroup != null && EmployeeGroupNames.Contains(o.EmployeeGroup.Name));
            //}

            if (FilterUsersInNoGroup)
            {
                spec = spec.And(o => o.EmployeeGroup == null);
            }
            else if (EmployeeGroupIds != null && EmployeeGroupIds.Count() > 0)
            {
                spec = spec.And(o => o.EmployeeGroup != null && EmployeeGroupIds.Contains(o.EmployeeGroup.Id));
            }

            ISpecification<User> criteriaSpec = null;
            if (SearchCriteria.IsNotNullOrWhiteSpace())
            {
                string criteria = SearchCriteria.ToUpper();
                criteriaSpec = new Specification<User>(o => (o.FirstName.Trim() + " " + o.LastName.Trim()).ToUpper().Contains(criteria));
                criteriaSpec = criteriaSpec.Or(o => o.Email.Trim().ToUpper().Contains(criteria));
                criteriaSpec = criteriaSpec.Or(o => o.UserName.Trim().ToUpper().Contains(criteria));
                criteriaSpec = criteriaSpec.Or(o => !string.IsNullOrEmpty(o.PhoneNumber.Trim()) && o.PhoneNumber.Trim().ToUpper().Contains(criteria));
                criteriaSpec = criteriaSpec.Or(o => !string.IsNullOrEmpty(o.Mobile.Trim()) && o.Mobile.Trim().ToUpper().Contains(criteria));

                if (RoleIsIncluded(AppConstants.RoleNames.Employee) || RoleIsNotExcluded(AppConstants.RoleNames.Employee))
                {
                    criteriaSpec = criteriaSpec.Or(o => o.EmployeeGroup != null && o.EmployeeGroup.Name.Trim().ToUpper().Contains(criteria));
                }
                spec = spec.And(criteriaSpec);
            }
            
            Expression<Func<User, string>> stringSortExp = null;
            //if (SortColumn.IsNotNullOrWhiteSpace()) { }
            switch (SortColumn.ToUpper())
            {
                case "FIRSTNAME":
                    stringSortExp = o => o.FirstName;
                    break;

                case "LASTNAME":
                    stringSortExp = o => o.LastName;
                    break;

                case "EMAIL":
                    stringSortExp = o => o.Email;
                    break;

                case "USERNAME":
                    stringSortExp = o => o.UserName;
                    break;

                default:
                    stringSortExp = o => o.FirstName;
                    break;
            }

            if (stringSortExp != null)
            {
                spec = SortType == SortType.Ascending
                    ? spec.OrderBy(stringSortExp)
                    : spec.OrderByDescending(stringSortExp);
            }

            if (PageSize > 0)
            {
                spec = spec.Skip((PageNo - 1) * PageSize).Take(PageSize);
            }

            return spec;
        }

        bool RoleIsIncluded(string role)
        {
            return IncludeRoles != null && IncludeRoles.Length > 0 && IncludeRoles.Contains(role);
        }

        bool RoleIsNotIncluded(string role)
        {
            return !RoleIsIncluded(role);
        }

        bool RoleIsExcluded(string role)
        {
            return ExcludeRoles != null && ExcludeRoles.Length > 0 && ExcludeRoles.Contains(role);
        }

        bool RoleIsNotExcluded(string role)
        {
            return !RoleIsExcluded(role);
        }
    }
}
