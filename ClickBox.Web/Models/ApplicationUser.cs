namespace ClickBox.Web.Models
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using ElCamino.AspNet.Identity.AzureTable.Model;

    using Microsoft.AspNet.Identity;

    public class ApplicationUser : IdentityUser, IUser<string>, IUser
    {
        #region Public Methods and Operators

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<ApplicationUser> manager, 
            string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

            // Add custom user claims here
            return userIdentity;
        }

        #endregion
    }
}