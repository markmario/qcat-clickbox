// --------------------------------------------------------------------------------------------------
//  <copyright file="Product.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Models
{
    using ClickBox.Web.TableStorage;

    using Microsoft.WindowsAzure.Storage.Table;

    public class Product : TableEntity, IContainTableReference
    {
        private string name;

        public Product()
        {
            this.PartitionKey = TableStorageUtil.GetPartitionPrefix() + 1;
        }

        public Product(string rowKeyName)
        {
            this.RowKey = rowKeyName;
            this.name = rowKeyName;
            this.PartitionKey = TableStorageUtil.GetPartitionPrefix() + 1;
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
    }
}