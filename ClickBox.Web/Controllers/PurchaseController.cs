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
    [RequireHttps(Order = 1)]
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
            var toRet = await client.GetEntityByPropertyFilterAsync<Product>("Id", productId);
            var model = new { Product = toRet };
            return View(model);
        }
    }
}