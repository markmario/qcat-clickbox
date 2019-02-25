using System;

namespace ClickBox.Web.Models
{
    public class PersistedUserAccount : UserAccount
    {
        public new string AccountType { get; set; }
    }
}