using ClickBox.Web.Models;
using ClickBox.Web.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ClickBox.Web.Controllers
{
    public class PurchaseController : Controller
    {
        private CloudTableClient client;

        public PurchaseController(CloudTableClient client)
        {
            this.client = client;
        }

        // GET: Purchase
        public async Task<ActionResult> Index(string productId)
        {
            //var toRet = await client.GetEntitiesAsync<Product>();
            var toRet = new Product() {
                RowKey = productId,
                Name = "PageMaker",
                Description = "Splits multi page PDF Files for Load Files"
            };
            var model = new { Product = toRet };
            return View(model);
        }
    }
}