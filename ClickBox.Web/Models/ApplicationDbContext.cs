namespace ClickBox.Web.Models
{
    using ElCamino.AspNet.Identity.AzureTable;

    public class ApplicationDbContext : IdentityCloudContext<ApplicationUser>
    {
        #region Constructors and Destructors

        public ApplicationDbContext()
            : base()
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