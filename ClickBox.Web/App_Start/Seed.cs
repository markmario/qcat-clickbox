// --------------------------------------------------------------------------------------------------
//  <copyright file="Seed.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------

 //[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Seed), "It")]

namespace ClickBox.Web.App_Start
{
    using ClickBox.Web.Models;

    public static class Seed
    {
        #region Public Methods and Operators

        public static void It()
        {
            // var session = DependencyResolver.Current.GetService<IDocumentSession>();
            using (var session = MvcApplication.DocumentStore.OpenSession())
            {
                //session.Store(
                //    new Product());

                //session.SaveChanges();
            }
        }

        #endregion
    }
}
