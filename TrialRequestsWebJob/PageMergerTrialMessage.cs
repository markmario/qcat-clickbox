// --------------------------------------------------------------------------------------------------
//  <copyright file="PageMergerTrialMessage.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace TrialRequestsWebJob
{
    public class PageMergerTrialMessage
    {
        //{"AccountName":"Simon Dude","AccountEmail":"utzbach@gmail.com",
        //"AccountOrganisation":"NodeEngine","AccountPassword":"2981e9e3-687f-461d-840a-a2e703d8b3ba",
        //"AccountRequestMessage":"test from live site"} 
        public string AccountName { get; set; }
        public string AccountEmail { get; set; }
        public string AccountOrganisation { get; set; }
        public string AccountPassword { get; set; }
        public string AccountRequestMessage { get; set; }
    }
}