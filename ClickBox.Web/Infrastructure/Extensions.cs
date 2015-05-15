// --------------------------------------------------------------------------------------------------
//  <copyright file="Extensions.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace ClickBox.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public static class Extensions
    {
        #region Public Methods and Operators

        public static List<SelectListItem> EnumToSelectList(Type enumType)
        {
            return
                Enum.GetValues(enumType)
                    .Cast<int>()
                    .Select(i => new SelectListItem { Value = i.ToString(), Text = Enum.GetName(enumType, i), })
                    .ToList();
        }

        #endregion
    }
}