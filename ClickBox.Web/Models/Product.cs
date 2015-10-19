// --------------------------------------------------------------------------------------------------
//  <copyright file="Product.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Models
{
    using System.Web.Mvc;

    using ClickBox.Web.TableStorage;

    using Microsoft.WindowsAzure.Storage.Table;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    [Bind(Exclude = "Timestamp, TableName, RowKey, PartitionKey, ETag")]
    public class Product : TableEntity, IContainTableReference
    {
        private string name;

        public Product()
        {
            this.PartitionKey = 1.ToString();
        }

        public Product(string rowKeyName)
        {
            this.RowKey = rowKeyName;
            this.name = rowKeyName;
            this.PartitionKey = 1.ToString();
        }

        #region Public Properties

        public string Id { get; set; }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.RowKey = value;
            }
        }

        public string PrivateKey { get; set; }

        public string PublicKey { get; set; }

        #endregion

        public override string ToString()
        {
            return this.RowKey + "\t\t" + this.Id + "]";
        }

        public string TableName
        {
            get{return "Products";}
        }

        public string Description { get; set; }
        public string Image { get; set; }

        public double Price { get; set; }
    }
}