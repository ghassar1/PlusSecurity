using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AcSys.Core.Data.Repository;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Data.Users;
using AcSys.ShiftManager.Model;

namespace AcSys.ShiftManager.Data.EF.Repos.Users
{
    public class UserRepository : GenericRepository<ApplicationDbContext, User>, IUserRepository
    {
        public ApplicationDbContext Context { get; set; }

        public DateTime Created { get; set; }

        public IQueryable<User> Users
        {
            get
            {
                return GetAsQueryable();
            }
        }

        public UserRepository(ApplicationDbContext context)
            : base(context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context), "DbContext must be provided.");

            Context = context;
            Created = DateTime.Now;
        }

        #region Private Methods

        void CheckParamForNull(object param, string paramName)
        {
            if (param == null)
                throw new ArgumentNullException(string.Format("{0} must be provided.", paramName));
        }

        void CheckStringParamForNullOrEmpty(string param, string paramName)
        {
            if (param == null)
                throw new ArgumentNullException(string.Format("{0} must be provided.", paramName));
        }

        Role GetAppRole(string roleName)
        {
            var role = Context.Roles.FirstOrDefault(r => r.Name.ToUpper() == roleName.ToUpper());
            if (role == null)
                throw new InvalidOperationException("Role does not exist.");

            return role;
        }

        #endregion

        #region IUserStore

        public async Task<User> FindByNameAsync(string username)
        {
            CheckStringParamForNullOrEmpty(username, "Username");

            try
            {
                var user = Context.Users.Where(e => e.UserName.ToLower() == username.ToLower()).FirstOrDefaultAsync();
                return await user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> FindByIdAsync(Guid userId)
        {
            CheckParamForNull(userId, "User Id");

            try
            {
                var user = Context.Users.Where(e => e.Id == userId).FirstOrDefaultAsync();
                return await user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteAsync(User user)
        {
            CheckParamForNull(user, "User");

            //Context.Users.Remove(user);

            user.RemoveRoles();
            //Context.UserActivations.RemoveRange(user.UserActivations);

            base.Delete(user);

            await Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            CheckParamForNull(user, "User");

            Context.Users.Attach(user);
            Context.Entry(user).State = EntityState.Modified;

            await Context.SaveChangesAsync();
        }

        public async Task CreateAsync(User user)
        {
            CheckParamForNull(user, "User");

            Context.Users.Add(user);
            await Context.SaveChangesAsync();
        }

        #endregion

        #region IUserPassowrdStore

        public async Task<bool> HasPasswordAsync(User user)
        {
            return await Task.FromResult<bool>(string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            CheckParamForNull(user, "User");

            return Task.FromResult<string>(user.PasswordHash);
        }

        public async Task SetPasswordHashAsync(User user, string passwordHash)
        {
            CheckParamForNull(user, "User");
            CheckStringParamForNullOrEmpty(passwordHash, "Password Hash");

            Context.Users.Attach(user);

            user.PasswordHash = passwordHash;

            //Context.Entry(user).State = EntityState.Modified;

            await UpdateAsync(user);
        }

        #endregion

        #region IUserRoleStore

        public virtual async Task<bool> IsInRoleAsync(User user, string roleName)
        {
            CheckParamForNull(user, "User");
            CheckStringParamForNullOrEmpty(roleName, "Role Name");

            return await Task.FromResult<bool>(user.HasRole(roleName));
        }

        public virtual Task<IList<string>> GetRolesAsync(User user)
        {
            CheckParamForNull(user, "User");

            return Task.FromResult<IList<string>>(user.GetRoleNames());
        }

        public async Task AddToRoleAsync(User user, string roleName)
        {
            CheckParamForNull(user, "User");
            CheckStringParamForNullOrEmpty(roleName, "Role Name");

            var role = GetAppRole(roleName);
            user.AddRole(role);
            
            await UpdateAsync(user);
        }

        public async Task RemoveFromRoleAsync(User user, string roleName)
        {
            CheckParamForNull(user, "User");
            CheckStringParamForNullOrEmpty(roleName, "Role Name");

            user.RemoveRole(roleName);
            await UpdateAsync(user);
        }

        #endregion

        #region IUserSecurityStampStore

        public Task<string> GetSecurityStampAsync(User user)
        {
            CheckParamForNull(user, "User");

            return Task.FromResult<string>(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(User user, string stamp)
        {
            CheckParamForNull(user, "User");
            CheckStringParamForNullOrEmpty(stamp, "Security Stamp");

            user.SecurityStamp = stamp;
            return Task.FromResult<int>(0);
        }

        #endregion

        #region IUserEmailStore

        public Task<User> FindByEmailAsync(string email)
        {
            CheckStringParamForNullOrEmpty(email, "Email");

            return Context.Users.Where(e => e.Email.ToLower() == email.ToLower())
                .FirstOrDefaultAsync();
        }

        public Task<string> GetEmailAsync(User user)
        {
            CheckParamForNull(user, "User");

            return Task.FromResult<string>(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user)
        {
            CheckParamForNull(user, "User");

            return Task.FromResult<bool>(user.EmailConfirmed);
        }

        public Task SetEmailAsync(User user, string email)
        {
            CheckParamForNull(user, "User");
            CheckStringParamForNullOrEmpty(email, "Email");

            user.Email = email;
            return Task.FromResult<int>(0);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed)
        {
            CheckParamForNull(user, "User");

            user.EmailConfirmed = confirmed;
            return Task.FromResult<int>(0);
        }

        #endregion

        public Task AddClaimAsync(User user, Claim claim)
        {
            CheckParamForNull(user, "User");
            CheckParamForNull(claim, "Claim");

            user.Claims.Add(new UserClaim { ClaimType = claim.Type, ClaimValue = claim.Value });
            return Task.FromResult(0);
        }

        public async Task<IList<Claim>> GetClaimsAsync(User user)
        {
            CheckParamForNull(user, "User");

            return await Task.FromResult(user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList());
        }

        public async Task RemoveClaimAsync(User user, Claim claim)
        {
            CheckParamForNull(user, "User");
            CheckParamForNull(claim, "Claim");

            IEnumerable<UserClaim> claims;
            var claimValue = claim.Value;
            var claimType = claim.Type;
            claims = user.Claims.Where(uc => uc.ClaimValue == claimValue && uc.ClaimType == claimType).ToList();

            foreach (var c in claims)
            {
                user.Claims.Remove(c);
            }

            //await Task.FromResult(0);
            await Context.SaveChangesAsync();
        }
    }
}
