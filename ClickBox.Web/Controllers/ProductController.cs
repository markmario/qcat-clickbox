using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClickBox.Web.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/

        public ActionResult Index()
        {
            return new ContentResult()
                 { Content = "Product HTTP GET (index)" };
        }

        //
        // GET: /Product/Details/5

        public ActionResult Details(int id)
        {
            return new ContentResult()
                 { Content = "Product Details" };
        }

        //
        // GET: /Product/Create

        public ActionResult Create()
        {
            return new ContentResult()
                 { Content = "Create Product HTTP GET" };
        }

        //
        // POST: /Product/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {return new ContentResult()
                 { Content = "Create Product HTTP POST" };
            }
        }

        //
        // GET: /Product/Edit/5

        public ActionResult Edit(int id)
        {
            return new ContentResult()
                 { Content = "Edit Product HTTP GET" };
        }

        //
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
                return new ContentResult()
                 { Content = "Edit Product POST" };
            }
        }

        //
        // GET: /Product/Delete/5

        public ActionResult Delete(int id)
        {
            return new ContentResult()
                 { Content = "Delete Product GET" };
        }

        //
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
                return new ContentResult()
                 { Content = "Delete Product HTTP POST" };
            }
        }
    }
}
