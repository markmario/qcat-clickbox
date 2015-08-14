namespace ClickBox.Web.Models
{
    using System.Collections.Generic;

    public class ExternalLoginViewModel
    {
        #region Public Properties

        public string Name { get; set; }

        public string State { get; set; }

        public string Url { get; set; }

        #endregion
    }

    public class ManageInfoViewModel
    {
        #region Public Properties

        public string Email { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }

        public string LocalLoginProvider { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        #endregion
    }

    public class UserInfoViewModel
    {
        #region Public Properties

        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }

        #endregion
    }

    public class UserLoginInfoViewModel
    {
        #region Public Properties

        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        #endregion
    }
}