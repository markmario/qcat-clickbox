// --------------------------------------------------------------------------------------------------
//  <copyright file="Product.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Models
{
    public class Product
    {
        // public  RSA Key;
        #region Public Properties

        public string Id { get; set; }

        public string Name { get; set; }

        public string PrivateKey { get; set; }

        public string PublicKey { get; set; }

        #endregion
    }
}