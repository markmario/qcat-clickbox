// --------------------------------------------------------------------------------------------------
//  <copyright file="TopCatDbConfiguration.cs" company="QCAT Pty Ltd.">
//    Copyright (c) 2015 QCAT Pty Ltd. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------------------------------
namespace QCat
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Xml.Linq;

    public class TopCatDbConfiguration
    {
        // static holder for instance, need to use lambda to construct since constructor private
        #region Static Fields

        private static readonly Lazy<TopCatDbConfiguration> LazyInstance =
            new Lazy<TopCatDbConfiguration>(() => new TopCatDbConfiguration());

        #endregion

        #region Constructors and Destructors

        private TopCatDbConfiguration()
        {
            if (this.ConnectionString == null)
            {
                var fi = new FileInfo(@"..\config\topcat.config");
                if (!fi.Exists)
                {
                    var path1 = fi.FullName;
                    fi = new FileInfo(@"topcat.config");
                    if (!fi.Exists)
                    {
                        throw new ConfigurationErrorsException(
                            "Cannot find topcat.config in : " + path1 + " or " + fi.FullName);
                    }
                }

                XDocument document = XDocument.Load(fi.FullName);
                XElement xElement = document.Element("TopCatDb");
                if (xElement != null)
                {
                    this.ConnectionString = xElement.Value;
                }
            }
        }

        #endregion

        #region Public Properties

        public static TopCatDbConfiguration Instance
        {
            get
            {
                return LazyInstance.Value;
            }
        }

        public string ConnectionString { get; private set; }

        #endregion
    }
}