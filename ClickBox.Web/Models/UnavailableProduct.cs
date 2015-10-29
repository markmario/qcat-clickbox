using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickBox.Web.Models
{
    public class UnavailableProduct
    {
        public int HttpCode { get; set; }
        public string ErrorMessage { get; set; }
        public string HeaderMessage { get { return "Oops, the product you requested is not available"; } }
        public string ContactUsLink { get { return "http://www.qcat.com.au/contact.html"; } }
    }
}