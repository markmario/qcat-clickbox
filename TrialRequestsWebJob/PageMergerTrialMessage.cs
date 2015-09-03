// --------------------------------------------------------------------------------------------------
//  <copyright file="PageMergerTrialMessage.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace TrialRequestsWebJob
{
    public class PageMergerTrialMessage
    {
        public string AccountName { get; set; }
        public string AccountEmail { get; set; }
        public string AccountOrganisation { get; set; }
        public string AccountPassword { get; set; }
        public string AccountRequestMessage { get; set; }
        public override string ToString()
        {
            return "{AccountName:"+ AccountName + ", AccountEmail:" + AccountEmail +","+
                    "AccountOrganisation:" + AccountOrganisation + ", AccountPassword:"+ AccountPassword +", "+
                    "AccountRequestMessage:" + AccountRequestMessage + "}";
        }
    }
}