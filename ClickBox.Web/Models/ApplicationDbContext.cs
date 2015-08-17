namespace ClickBox.Web.Models
{
    using ElCamino.AspNet.Identity.AzureTable;
    using ElCamino.AspNet.Identity.AzureTable.Model;

    public class ApplicationDbContext : IdentityCloudContext<ApplicationUser>
    {
        #region Constructors and Destructors

        public ApplicationDbContext()
            : base(new IdentityConfiguration() { StorageConnectionString = MvcApplication.TableStoreConnectionString })
        {
        }

        #endregion

        #region Public Methods and Operators

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        #endregion
    }
}