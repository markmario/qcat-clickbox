// --------------------------------------------------------------------------------------------------
//  <copyright file="PostLicenseRequestFailure.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Models
{
    using System;

    using Microsoft.WindowsAzure.Storage.Table;

    public class PostLicenseRequestFailure : TableEntity, IContainTableReference
    {
        public string Request { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
        public string TableName => "PostLicenseRequestFailure";
        public PostLicenseRequestFailure()
        {
            PartitionKey = 5.ToString();
        }
    }
}