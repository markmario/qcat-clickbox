namespace ClickBox.Web.Controllers
{
    using System;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using ClickBox.Web.Models;
    using ClickBox.Web.TableStorage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class ProductController : Controller
    {
        private readonly CloudTableClient client;
        #region Constructors and Destructors

        public ProductController(CloudTableClient client) {
            this.client = client;
        }

        #endregion

        public async Task<ActionResult> Index()
        {
            //var toRet = this.session.Query<Product>().ToList();
            var toRet = await client.GetEntitiesAsync<Product>();
            var model = new { Products = toRet };
            return this.View(model);
        }

        // GET: /Product/Details/5
        public ActionResult Details(int? id)
        {
            return new ContentResult() { Content = "Product Details HTTP GET" + id };
        }

        public ActionResult NewProductId()
        {
            var prodId = Guid.NewGuid();
            return this.Json(prodId, JsonRequestBehavior.AllowGet);
        }

        // GET: /Product/Create
        public ActionResult Create()
        {
            return new ContentResult() { Content = "Create Product HTTP GET" };
        }

        // POST: /Product/Create
        [HttpPost]
        public async Task<ActionResult> Create(Product newProduct)
        {
            try
            {
                // TODO: Add insert logic here
                if (string.IsNullOrWhiteSpace(newProduct.PrivateKey))
                {
                    var keyGen = RSA.Create();
                    newProduct.PublicKey = keyGen.ToXmlString(false);
                    newProduct.PrivateKey = keyGen.ToXmlString(true);
                    // Rhino.Licensing.   
                }

                var newQCatProduct = new Product()
                                         {
                                             Id = newProduct.Id, 
                                             Name = newProduct.Name, 
                                             PrivateKey = newProduct.PrivateKey, 
                                             PublicKey = newProduct.PublicKey,
                                             RowKey = newProduct.Name
                                         };

                await client.InsertStorageEntityAsync(newQCatProduct);
                var toRet = await client.GetEntitiesAsync<Product>();
                var model = new { Products = toRet };
                return this.Json(model);
            }
            catch
            {
                return this.Json("Product creation failed!");
            }
        }

        // POST: /Product/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(Product editProduct)
        {
            try
            {
                var edited = await client.GetEntityByPropertyFilterAsync<Product>("Id",editProduct.Id);
                edited.Name = editProduct.Name;
                edited.PrivateKey = editProduct.PrivateKey;
                edited.PublicKey = editProduct.PublicKey;
                await client.UpdateEntityAsync(editProduct);
                var toRet = await client.GetEntityByPartitionAndRowKeyAsync<Product>(editProduct.Name);
                var model = new { Products = toRet };
                return this.Json(model);
            }
            catch
            {
                return this.Json("Product edit failed!");
            }
        }

        // GET: /Product/Delete/5
        public ActionResult Delete(int id)
        {
            return new ContentResult() { Content = "Delete Product GET" };
        }

        // POST: /Product/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return new ContentResult() { Content = "Delete Product HTTP POST" };
            }
        }
    }
}
