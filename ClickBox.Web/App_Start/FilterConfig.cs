// --------------------------------------------------------------------------------------------------
//  <copyright file="FilterConfig.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web
{
    using System.Web.Mvc;

    public class FilterConfig
    {
        #region Public Methods and Operators

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // filters.Add(new TokenValidationAttribute());
        }

        #endregion
    }
}