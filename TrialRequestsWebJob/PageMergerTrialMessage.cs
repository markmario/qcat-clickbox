// --------------------------------------------------------------------------------------------------
//  <copyright file="PageMergerTrialMessage.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace TrialRequestsWebJob
{
    using ClickBox.Util;
    using System;

    public class PageMergerTrialMessage
    {
        public Guid Id
        {
            get
            {
                return new Guid(this.AccountPassword);
            }
        }

        public string AccountEmail { get; set; }

        public string AccountName { get; set; }

        public string AccountOrganisation { get; set; }

        public string AccountPassword { get; set; }

        public string Password
        {
            get
            {
                return this.Id.ToShortString();
            }
        }

        public string AccountRequestMessage { get; set; }
        public override string ToString()
        {
            return "{AccountName:"+ AccountName + ", AccountEmail:" + AccountEmail +","+
                    "AccountOrganisation:" + AccountOrganisation + ", AccountPassword:"+ AccountPassword +", "+
                    "AccountRequestMessage:" + AccountRequestMessage + "}";
        }
    }
}