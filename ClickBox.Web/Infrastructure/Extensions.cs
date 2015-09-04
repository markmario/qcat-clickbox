// --------------------------------------------------------------------------------------------------
//  <copyright file="Extensions.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Infrastructure
{
    using Models;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using TableStorage;
    using System.Threading.Tasks;

    public static class Extensions
    {
        public static CloudTableClient Client { get; set; }

        //static Extensions()
        //{
        //    GetProductList();
        //}
        #region Public Methods and Operators

        public static List<SelectListItem> EnumToSelectList(Type enumType)
        {
            return
                Enum.GetValues(enumType)
                    .Cast<int>()
                    .Select(i => new SelectListItem { Value = i.ToString(), Text = Enum.GetName(enumType, i), })
                    .ToList();
        }

        public static List<SelectListItem> GetProductList()
        {
            Client = MvcApplication.TableStore.CreateCloudTableClient();
            var p =  Client.GetEntities<Product>();
            var dropList = p.Select(i => new SelectListItem { Value = i.Name, Text = i.Name }).ToList();
            return dropList;
        }


        #endregion
    }
}