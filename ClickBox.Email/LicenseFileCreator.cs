// --------------------------------------------------------------------------------------------------
//  <copyright file="LicenseFileCreator.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2016 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Email
{
    using System;

    using ClickBox.Mail;

    public class LicenseFileCreator
    {
        public string Create(IHaveDataForMail msg)
        {
            var s = $"\"UserName\": \"{msg.To}\", \"AccountType\": \"{msg.AccountType}\", \"ProductName\": \"{msg.ProductName}\", \"ExpiryDate\": \"{msg.ExpiryDate.ToString("dd-MM-yyyy")}\"";
            s = "{" + s + "}";
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}