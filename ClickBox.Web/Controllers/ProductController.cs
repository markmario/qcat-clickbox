namespace ClickBox.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Web.Mvc;

    using ClickBox.Web.Models;

    using Raven.Client;

    public class ProductController : Controller
    {
        private readonly IDocumentSession session;

        #region Constructors and Destructors

        public ProductController(IDocumentSession session)
        {
            this.session = session;
        }

        #endregion

        // GET: /Product/

        // [AcceptVerbs(HttpVerbs.Get)]
        // public ActionResult Get([DataSourceRequest] DataSourceRequest request)
        // {
        // // var toRet = _ctx.Applications.OrderBy(o => o.Id).ToDataSourceResult(request);
        // var toRet = this._session.Query<Product>().ToDataSourceResult(request);
        // return this.Json(toRet, JsonRequestBehavior.AllowGet);
        // }

        public ActionResult Index()
        {
            var toRet = this.session.Query<Product>().ToList();
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
        public ActionResult Create(Product newProduct)
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
                                             PublicKey = newProduct.PublicKey
                                         };
                this.session.Store(newQCatProduct);
                var toRet = this.session.Query<Product>().ToList();
                var model = new { Products = toRet };
                return this.Json(model);
            }
            catch
            {
                return this.Json("Product failed!");
            }
        }

        // GET: /Product/Edit/5
        public ActionResult Edit(int id)
        {
            return new ContentResult() { Content = "Edit Product HTTP GET" };
        }

        // POST: /Product/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return new ContentResult() { Content = "Edit Product POST" };
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
