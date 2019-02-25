namespace ClickBox.Messages
{
    using Util;
    using System;

    public class AccountCreationMessage : IAzureQueueMessage
    {
        public Guid Id => new Guid(this.AccountPassword);

        public string AccountProductName { get; set; }

        public string AccountEmail { get; set; }

        public string AccountName { get; set; }

        public string AccountOrganisation { get; set; }

        public string AccountPassword { get; set; }

        public string Password
        {
            get
            {
                return Id.ToShortString();
            }
        }

        public string AccountRequestMessage { get; set; }

        public string AccountLicenseType { get; set; }

        public override string ToString()
        {
            return "{AccountName:"+ AccountName + ", AccountEmail:" + AccountEmail +","+
                    "AccountOrganisation:" + AccountOrganisation + ", AccountPassword:"+ AccountPassword +", "+
                    "AccountRequestMessage:" + AccountRequestMessage + ", AccountProductName:" + AccountProductName +"}";
        }

        public bool PaymentReceived { get; set; }

        public string QueueName => "create-account";
    }
}